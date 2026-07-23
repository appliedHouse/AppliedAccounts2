using AppliedGlobals;
using Microsoft.Data.Sqlite;
using System.Text;

namespace AppliedDB.CreateDB
{
    public class UpdateDB
    {
        public DataSource Source { get; set; }
        public string DBFile { get; set; }
        public SqliteConnection MyConnection { get; set; }
        public StringBuilder Log { get; set; } = new StringBuilder();

        public UpdateDB(string dbFile, AppValues.AppPath appPaths) 
        {
            Source = new(appPaths);
            MyConnection = Source.MyConnection;
        }

        public async Task UpdateDatabaseAsync()
        {
            if(MyConnection == null)
            {
                Log.AppendLine("No connection to the database. Update aborted.");
                return;
            }

            if(MyConnection.State != System.Data.ConnectionState.Open)
            {
                MyConnection.Open();
                Log.AppendLine("Database connection opened.");
            }

            bool IsBook1Created = false;
            bool IsBook2Created = false;
                        
            string[] fileNames = TableNames.GetTableNames();
            foreach(string tableName in fileNames)
            {

                if (!TableIsExist(tableName))
                {
                    string createTableQuery = TableQueries.GetTableQuery(tableName);
                    if (!string.IsNullOrEmpty(createTableQuery))
                    {
                        try
                        {
                            using SqliteCommand command = new SqliteCommand(createTableQuery, MyConnection);
                            command.ExecuteNonQuery();

                            // If we get here, the table was created successfully
                            if (tableName.Equals("Book")) { IsBook1Created = true; }
                            if (tableName.Equals("Book2")) { IsBook2Created = true; }
                            Log.AppendLine($"Table {tableName} created successfully.");
                        }
                        catch (SqliteException ex)
                        {
                            Log.AppendLine($"Failed to create table {tableName}: {ex.Message}");
                            // Optionally rethrow or handle the exception
                        }
                    }
                    else
                    {
                        Log.AppendLine($"No query found for table {tableName}.");
                    }

                    if(IsBook1Created && IsBook2Created)
                    {
                        Log.AppendLine("DataTable Book created in update process");
                        DataMigration dataMigration = new(Source);
                        await dataMigration.Cash2BookAsync();
                        Log.AppendLine("Data Migrated from CashBook to Book");
                        await dataMigration.Bank2BookAsync();
                        Log.AppendLine("Data Migrated from BankBook to Book");
                    }
                }
            }

        }

        public bool TableIsExist(string tableName)
        {
            string _Query = $"SELECT COUNT(*) FROM [sqlite_master] WHERE Type in ('table','view') AND name = '{tableName}'";

            using SqliteCommand _Command = new(_Query, MyConnection);

            int _result = Convert.ToInt32(_Command.ExecuteScalar());
            return _result > 0;
        }
    }
}
