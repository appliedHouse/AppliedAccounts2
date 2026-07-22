using Microsoft.Data.Sqlite;
using System.Text;

namespace AppliedDB.CreateDB
{
    public class CreateClientDB
    {
        public SqliteConnection MyConnection { get; set; }
        public StringBuilder Log { get; set; } = new StringBuilder();

        #region Constructor
        public CreateClientDB(string connectionString)
        {
            MyConnection = new SqliteConnection(connectionString);
        }
        #endregion


        #region Create Database file
        // give File name with full path if you want to create in specific location, otherwise it will create in current directory
        public async Task<bool> CreateDatabase(string databaseName)         
        {
            try
            {
                if(databaseName.Contains(".db"))
                {
                    databaseName = databaseName.Replace(".db", "");
                }
               
                // Option 1: Create an empty database file first (if you want to be explicit)
                // SQLite will create it anyway, but this makes it clear
                if (!File.Exists(databaseName))
                {
                    // Create an empty file
                    File.Create(databaseName).Close();
                    Log.AppendLine($"Database file {databaseName}' created.");
                }

                // Now create the connection
                string connectionString = $"Data Source={databaseName};";
                MyConnection = new SqliteConnection(connectionString);

                // Open the connection
                await MyConnection.OpenAsync();

                if (MyConnection.State == System.Data.ConnectionState.Open)
                {
                    Log.AppendLine($"Connected to database '{databaseName}'.");

                    // Create tables
                    var result = await CreateTables();

                    if (result == "Success")
                    {
                        Log.AppendLine("All tables created successfully.");
                        return true;
                    }
                    else
                    {
                        Log.AppendLine($"Table creation failed: {result}");
                        return false;
                    }
                }
                else
                {
                    Log.AppendLine("Failed to open database connection.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.AppendLine($"Error creating database: {ex.Message}");
                return false;
            }
        }
        #endregion


        #region Create Data Tables
        private async Task<string> CreateTables()
        {
            string status = "Success";
            string[] tableNames = TableNames.GetTableNames();
            foreach(var tableName in tableNames)
            {
                string createTableQuery = TableQueries.GetTableQuery(tableName);
                if (!string.IsNullOrEmpty(createTableQuery))
                {
                    SqliteCommand command = new SqliteCommand(createTableQuery, MyConnection);
                    var _result = await command.ExecuteNonQueryAsync();
                    if (_result > 0)
                    {
                        Log.AppendLine($"Table {tableName} created successfully.");
                        
                    }
                    else
                    {
                        status = "Failed";
                        Log.AppendLine($"Failed to create table {tableName}.");
                        
                    }
                }
                else
                {
                    status = "QueryError";
                    Log.AppendLine($"No query found for table {tableName}.");
                    
                }
            }

            return status;
        }
        
        

        #endregion
    }
}


