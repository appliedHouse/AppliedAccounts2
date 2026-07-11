using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Menus;


namespace AppliedAccounts.Services.Menus
{
    public interface IMenuService
    {
        List<MenuItem> GetMenus();
        List<MenuItem> GetMenusByParent(int parentId);
        MenuItem GetMenuById(int id);
        Task RefreshMenusAsync();
        bool IsLoaded { get; }
        DateTime LastLoaded { get; }
        string DatabasePath { get; }
        bool DatabaseExists { get; }
    }

    public class MenuService : IMenuService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MenuService> _logger;
        private readonly IMenuDatabaseInitializer _dbInitializer;
        private const string CACHE_KEY = "ApplicationMenus";
        private const string CACHE_TIME_KEY = "MenuLastLoaded";
        private const string DB_PATH = "./wwwroot/System/MenusDB.db";

        public MenuService(
            IMemoryCache cache,
            ILogger<MenuService> logger,
            IMenuDatabaseInitializer dbInitializer)
        {
            _cache = cache;
            _logger = logger;
            _dbInitializer = dbInitializer;

            // Ensure database exists on service creation
            EnsureDatabase();
        }

        public bool IsLoaded => _cache.TryGetValue(CACHE_KEY, out _);
        public DateTime LastLoaded => _cache.TryGetValue(CACHE_TIME_KEY, out DateTime time) ? time : DateTime.MinValue;
        public string DatabasePath => DB_PATH;
        public bool DatabaseExists => _dbInitializer.DatabaseExists();

        private void EnsureDatabase()
        {
            try
            {
                _dbInitializer.EnsureDatabaseExists();
                _logger.LogInformation("Database check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure database exists");
                // Continue with fallback menus
            }
        }

        public List<MenuItem> GetMenus()
        {
            if (!_cache.TryGetValue(CACHE_KEY, out List<MenuItem> menus))
            {
                menus = LoadMenusFromDatabase();
                SetCache(menus);
            }
            return menus ?? new List<MenuItem>();
        }

        public List<MenuItem> GetMenusByParent(int parentId)
        {
            var allMenus = GetMenus();
            return allMenus.Where(m => m.ParentID == parentId).ToList();
        }

        public MenuItem GetMenuById(int id)
        {
            var allMenus = GetMenus();
            return allMenus.FirstOrDefault(m => m.ID == id)!;
        }

        public async Task RefreshMenusAsync()
        {
            try
            {
                // Check if database exists, if not create it
                _dbInitializer.EnsureDatabaseExists();

                var menus = await Task.Run(() => LoadMenusFromDatabase());
                SetCache(menus);
                _logger.LogInformation("Menus refreshed at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh menus");
                throw;
            }
        }

        private List<MenuItem> LoadMenusFromDatabase()
        {
            // If database doesn't exist, create and populate it
            if (!_dbInitializer.DatabaseExists())
            {
                _logger.LogWarning("Database not found during load. Initializing...");
                _dbInitializer.InitializeDatabase();
            }

            try
            {
                var menus = new List<MenuItem>();

                using var connection = new SqliteConnection($"Data Source={DB_PATH}");
                connection.Open();

                var commandText = "SELECT * FROM Menus ORDER BY Level, ParentID, ID";
                using var command = new SqliteCommand(commandText, connection);
                using var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);

                foreach (DataRow row in dataTable.Rows)
                {
                    menus.Add(new MenuItem
                    {
                        ID = row.Field<int>("ID"),
                        Title = row.Field<string>("Title") ?? "",
                        Active = row.Field<int>("Active") == 1,
                        Icon = row.Field<string>("Icon") ?? "",
                        Level = row.Field<int>("Level"),
                        ParentID = row.Field<int>("ParentID"),
                        NavigateTo = row.Field<string>("NavigateTo") ?? ""
                    });
                }

                _logger.LogInformation("Loaded {Count} menus from database", menus.Count);

                // If no data in database, populate it
                if (menus.Count == 0)
                {
                    _logger.LogWarning("Database has no menu data. Populating...");
                    _dbInitializer.InitializeDatabase();
                    return LoadMenusFromDatabase(); // Recursive call after population
                }

                return menus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load menus from database, using fallback");
                var fallbackMenus = MenusFromDB.Get2();
                _logger.LogWarning("Using {Count} fallback menus", fallbackMenus.Count);
                return fallbackMenus;
            }
        }

        private void SetCache(List<MenuItem> menus)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24))
                .SetAbsoluteExpiration(TimeSpan.FromDays(7))
                .SetPriority(CacheItemPriority.High);

            _cache.Set(CACHE_KEY, menus, cacheOptions);
            _cache.Set(CACHE_TIME_KEY, DateTime.Now, cacheOptions);
        }
    }
}