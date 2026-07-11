// Services/MenuDatabaseInitializer.cs
using Menus;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AppliedAccounts.Services.Menus
{
    public interface IMenuDatabaseInitializer
    {
        void EnsureDatabaseExists();
        void InitializeDatabase();
        bool DatabaseExists();
        void RecreateDatabase();
        bool TableHasData();
    }

    public class MenuDatabaseInitializer : IMenuDatabaseInitializer
    {
        private readonly ILogger<MenuDatabaseInitializer> _logger;
        private readonly string _dbPath = "./wwwroot/System/MenusDB.db";
        private readonly string _connectionString;

        public MenuDatabaseInitializer(ILogger<MenuDatabaseInitializer> logger)
        {
            _logger = logger;
            _connectionString = $"Data Source={_dbPath}";
        }

        public bool DatabaseExists()
        {
            return File.Exists(_dbPath);
        }

        public void EnsureDatabaseExists()
        {
            if (!DatabaseExists())
            {
                InitializeDatabase(); // This creates the table
            }
            else
            {
                _logger.LogInformation("Database already exists at: {DbPath}", _dbPath);

                // PROBLEM: This runs even if the database file exists but the table doesn't
                if (!TableHasData()) // TableHasData tries to query Menus table
                {
                    _logger.LogWarning("Database exists but Menus table is empty. Populating...");
                    PopulateDatabase();
                }
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                // Create database and tables
                CreateDatabaseSchema();

                // Populate with data from Get2()
                PopulateDatabase();

                _logger.LogInformation("Database initialized successfully at: {DbPath}", _dbPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                throw;
            }
        }

        public void RecreateDatabase()
        {
            try
            {
                if (DatabaseExists())
                {
                    File.Delete(_dbPath);
                    _logger.LogInformation("Deleted existing database");
                }

                InitializeDatabase();
                _logger.LogInformation("Database recreated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recreating database");
                throw;
            }
        }

        public bool TableHasData()
        {
            try
            {
                if (!DatabaseExists())
                    return false;

                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                var query = "SELECT COUNT(*) FROM Menus";
                using var command = new SqliteCommand(query, connection);
                var count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if table has data");
                return false;
            }
        }

        private void CreateDatabaseSchema()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Menus (
                    ID INTEGER PRIMARY KEY,
                    Title TEXT NOT NULL,
                    Active INTEGER NOT NULL,
                    Icon TEXT,
                    Level INTEGER NOT NULL,
                    ParentID INTEGER NOT NULL,
                    NavigateTo TEXT
                );

                CREATE INDEX IF NOT EXISTS idx_menus_parent ON Menus(ParentID);
                CREATE INDEX IF NOT EXISTS idx_menus_level ON Menus(Level);
                CREATE INDEX IF NOT EXISTS idx_menus_active ON Menus(Active);
            ";

            using var command = new SqliteCommand(createTableSql, connection);
            command.ExecuteNonQuery();

            _logger.LogInformation("Database schema created successfully");
        }

        private void PopulateDatabase()
        {
            // Get hardcoded menus from Get2()
            var menus = MenusFromDB.Get2();

            if (menus == null || !menus.Any())
            {
                _logger.LogWarning("No menus to populate from Get2()");
                return;
            }

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Clear existing data
            using var clearCommand = new SqliteCommand("DELETE FROM Menus", connection);
            clearCommand.ExecuteNonQuery();

            // Insert menus
            var insertSql = @"
                INSERT INTO Menus (ID, Title, Active, Icon, Level, ParentID, NavigateTo)
                VALUES (@ID, @Title, @Active, @Icon, @Level, @ParentID, @NavigateTo)
            ";

            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var menu in menus)
                {
                    using var insertCommand = new SqliteCommand(insertSql, connection, transaction);
                    insertCommand.Parameters.AddWithValue("@ID", menu.ID);
                    insertCommand.Parameters.AddWithValue("@Title", menu.Title);
                    insertCommand.Parameters.AddWithValue("@Active", menu.Active ? 1 : 0);
                    insertCommand.Parameters.AddWithValue("@Icon", menu.Icon ?? "");
                    insertCommand.Parameters.AddWithValue("@Level", menu.Level);
                    insertCommand.Parameters.AddWithValue("@ParentID", menu.ParentID);
                    insertCommand.Parameters.AddWithValue("@NavigateTo", menu.NavigateTo ?? "");

                    insertCommand.ExecuteNonQuery();
                }

                transaction.Commit();
                _logger.LogInformation("Populated {Count} menus from Get2()", menus.Count);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error populating database");
                throw;
            }
        }
    }
}