using AppliedGlobals;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using static AppliedGlobals.AppValues;

namespace AppliedDB
{
    public class DatabaseConfig
    {
        public string UsersDb { get; set; } = "AppliedUsers2.db";
        public string MessagesDb { get; set; } = "Messages.db";
        public string LanguagesDb { get; set; } = "Languages.db";
        public string SystemDb { get; set; } = "System.db";
        public string MenusDb { get; set; } = "MenusDB.db";
        public string ClientDb { get; set; } = string.Empty;
        public string SessionDb { get; set; } = string.Empty;
    }

    public class Connections : IDisposable
    {
        private readonly ILogger<Connections>? _logger;
        private readonly List<SqliteConnection> _activeConnections = new();
        private bool _disposed;
        private readonly DatabaseConfig _config;

        // Instance properties
        public AppValues GlobalValues { get; set; }
        public AppPath AppPaths { get; private set; }
        public string BaseUrl { get; private set; } = string.Empty;
        public string RootPath { get; private set; } = "wwwroot";
        public string UsersPath { get; private set; } = "SQLiteDB";
        public string ClientPath { get; private set; } = "SQLiteDB";
        public string ImagesPath { get; private set; } = "Images";
        public string MessagePath { get; private set; } = "Messages";
        public string LanguagePath { get; private set; } = "Languages";
        public string ReportPath { get; private set; } = "Reports";
        public string PDFPath { get; private set; } = "PDFReports";
        public string SystemPath { get; private set; } = "System";
        public string SessionPath { get; private set; } = "Sessions";
        public string MenuPath { get; private set; } = "System";
        public string TempDBPath { get; private set; } = "SqliteTemp";
        public string ClientDBFile { get; private set; } = string.Empty;
        public string SessionDBFile { get; private set; } = "Session";
        public AppUserModel AppUserProfile { get; set; }

        // Constructors
        public Connections()
        {
            _config = new DatabaseConfig();
        }

        public Connections(AppValues appValues)
        {
            _config = new DatabaseConfig();
            GlobalValues = appValues;
            InitializeFromAppPaths(appValues.Paths);
            AppPaths = appValues.Paths;

        }

        public Connections(AppValues appValues, ILogger<Connections>? logger = null)
        {
            _logger = logger;
            _config = new DatabaseConfig();
            GlobalValues = appValues;
            AppPaths = appValues.Paths;
            InitializeFromAppPaths(appValues.Paths);
        }

        public Connections(AppPath appPaths, ILogger<Connections>? logger = null)
        {
            _logger = logger;
            _config = new DatabaseConfig();
            InitializeFromAppPaths(appPaths);
        }

        public Connections(AppPath appPaths, DatabaseConfig config, ILogger<Connections>? logger = null)
        {
            _logger = logger;
            _config = config ?? new DatabaseConfig();
            InitializeFromAppPaths(appPaths);
        }

        private void InitializeFromAppPaths(AppPath appPaths)
        {
            AppPaths = appPaths;
            BaseUrl = appPaths.BaseUri;
            RootPath = appPaths.RootPath;
            UsersPath = appPaths.UsersPath;
            ClientPath = appPaths.ClientPath;
            ImagesPath = appPaths.ImagesPath;
            MessagePath = appPaths.MessagesPath;
            LanguagePath = appPaths.LanguagesPath;
            ReportPath = appPaths.ReportPath;
            PDFPath = appPaths.PDFPath;
            SystemPath = appPaths.SystemPath;
            SessionPath = appPaths.SessionPath;
            TempDBPath = appPaths.DBTempPath;
            ClientDBFile = appPaths.DBFile;
            SessionDBFile = appPaths.DBFile;
        }

        #region Private Helper Methods

        private static string BuildConnectionString(string dbPath, SqliteOpenMode mode = SqliteOpenMode.ReadWrite)
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath,
                Mode = mode,
                Cache = SqliteCacheMode.Private,
                Pooling = true,
                DefaultTimeout = 30
            };
            return builder.ToString();
        }

        private SqliteConnection? CreateAndTrackConnection(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                _logger?.LogWarning("Database file not found: {DbPath}", dbPath);
                return null;
            }

            try
            {
                var connectionString = BuildConnectionString(dbPath);
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                _activeConnections.Add(connection);
                return connection;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create connection to database at {DbPath}", dbPath);
                return null;
            }
        }

        private static string GetAppPath()
        {
            return Directory.GetCurrentDirectory();
        }

       

        #endregion

        #region Instance Connection Methods

        public SqliteConnection? GetSqliteUsers()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, ClientPath, ClientDBFile);
            return CreateAndTrackConnection(dbPath);
        }

        public SqliteConnection? GetSqliteClient()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, ClientPath, ClientDBFile);
            return CreateAndTrackConnection(dbPath);
        }

        public SqliteConnection? GetSqliteLanguage()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, LanguagePath, _config.LanguagesDb);
            return CreateAndTrackConnection(dbPath);
        }

        public SqliteConnection? GetSqliteMessage()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, MessagePath, _config.MessagesDb);
            return CreateAndTrackConnection(dbPath);
        }

        public SqliteConnection? GetSqliteSystem()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, SystemPath, _config.SystemDb);
            return CreateAndTrackConnection(dbPath);
        }

        public SqliteConnection? GetSqliteSession()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, SessionPath, SessionDBFile);
            return CreateAndTrackConnection(dbPath);
        }

        public SqliteConnection? GetSqliteMenu()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, MenuPath, _config.MenusDb);
            return CreateAndTrackConnection(dbPath);
        }

        public string GetTempDBPath()
        {
            return Path.Combine(GetAppPath(), "wwwroot", "SqliteTemp");
        }

        public string GetExcelPath()
        {
            return Path.Combine(GetAppPath(), "wwwroot", "ExcelFiles");
        }

        #endregion

        #region Async Connection Methods

        public async Task<SqliteConnection?> GetSqliteUsersAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, ClientPath, ClientDBFile);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        public async Task<SqliteConnection?> GetSqliteClientAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, ClientPath, ClientDBFile);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        public async Task<SqliteConnection?> GetSqliteLanguageAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, LanguagePath, _config.LanguagesDb);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        public async Task<SqliteConnection?> GetSqliteMessageAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, MessagePath, _config.MessagesDb);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        public async Task<SqliteConnection?> GetSqliteSystemAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, SystemPath, _config.SystemDb);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        public async Task<SqliteConnection?> GetSqliteSessionAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, SessionPath, SessionDBFile);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        public async Task<SqliteConnection?> GetSqliteMenuAsync()
        {
            var dbPath = Path.Combine(GetAppPath(), RootPath, MenuPath, _config.MenusDb);
            return await CreateAndTrackConnectionAsync(dbPath);
        }

        private async Task<SqliteConnection?> CreateAndTrackConnectionAsync(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                _logger?.LogWarning("Database file not found: {DbPath}", dbPath);
                return null;
            }

            try
            {
                var connectionString = BuildConnectionString(dbPath);
                var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();
                _activeConnections.Add(connection);
                return connection;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create connection to database at {DbPath}", dbPath);
                return null;
            }
        }

        #endregion

        #region Execute With Connection Pattern (Recommended)

        public async Task<T> ExecuteWithConnectionAsync<T>(
            string dbPath,
            Func<SqliteConnection, Task<T>> action)
        {
            using var connection = new SqliteConnection(BuildConnectionString(dbPath));
            await connection.OpenAsync();
            return await action(connection);
        }

        public async Task ExecuteWithConnectionAsync(
            string dbPath,
            Func<SqliteConnection, Task> action)
        {
            using var connection = new SqliteConnection(BuildConnectionString(dbPath));
            await connection.OpenAsync();
            await action(connection);
        }

        public T ExecuteWithConnection<T>(
            string dbPath,
            Func<SqliteConnection, T> action)
        {
            using var connection = new SqliteConnection(BuildConnectionString(dbPath));
            connection.Open();
            return action(connection);
        }

        public void ExecuteWithConnection(
            string dbPath,
            Action<SqliteConnection> action)
        {
            using var connection = new SqliteConnection(BuildConnectionString(dbPath));
            connection.Open();
            action(connection);
        }

        #endregion

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                foreach (var connection in _activeConnections)
                {
                    try
                    {
                        connection?.Close();
                        connection?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error disposing connection");
                    }
                }
                _activeConnections.Clear();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        // Deprecated static method for backward compatibility
        internal static SqliteConnection? GetClientConnection(string dBFile)
        {
            var _FilePath = Path.Combine(GetAppPath(), "wwwroot", "SQLiteDB", dBFile);
            SqliteConnection _Connection = new($"Data Source={_FilePath}");
            return _Connection;
            
        }

        // Deprecated static method for backward compatibility
        internal static SqliteConnection GetSqliteConnectionbyString(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        internal static SqliteConnection? GetUsersConnection()
        {
            var Config = new DatabaseConfig();
            var _FilePath = Path.Combine(GetAppPath(), "wwwroot", "SQLiteDB", Config.UsersDb);
            SqliteConnection _Connection = new($"Data Source={_FilePath}");
            return _Connection;
        }

        #endregion
    }

    // Extension methods for easier usage
    public static class ConnectionsExtensions
    {
        public static async Task<T> ExecuteWithConnectionAsync<T>(
            this Connections connections,
            string dbPath,
            Func<SqliteConnection, Task<T>> action)
        {
            return await connections.ExecuteWithConnectionAsync(dbPath, action);
        }

        public static async Task ExecuteWithConnectionAsync(
            this Connections connections,
            string dbPath,
            Func<SqliteConnection, Task> action)
        {
            await connections.ExecuteWithConnectionAsync(dbPath, action);
        }
    }
}