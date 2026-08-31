using AppliedGlobals;
using AppMessages;
using Microsoft.Data.Sqlite;
using System;

namespace AppliedDB.CreateDB
{
    public class DBPatches
    {
        public DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; } = new();
        public SqliteCommand MyCommand { get; set; }


        #region Constructor
        public DBPatches(DataSource _Source)
        {
            Source = _Source;
        }
        public DBPatches(AppValues.AppPath _appPaths)
        {
            Source = new DataSource(_appPaths);
        }
        #endregion

        #region Execute Data Pactches
        public async Task ExecutePatches()
        {
            ProjectPatch();
            CustomerAddress3();
            BillPayable2_AddUnit();
            BillReceivable2_AddUnit();
            InventoryPatches();
            DropViewIfExists("view_Receipts");
            AlterReceipt2Columns();

        }
        #endregion

        #region Query Executor
        private bool QueryExecutor(string query, string Message)
        {
            if (Source.MyConnection == null)
            {
                MsgClass.Warning(AppMessages.Enums.Messages.DataSourceIsNull);
                return false;
            }

            using (var command = new SqliteCommand(query, Source.MyConnection))
            {
                try
                {
                    command.ExecuteNonQuery();
                    MsgClass.Success(Message);
                    return true;
                }
                catch (Exception ex)
                {
                    MsgClass.Error(ex);
                    return false;
                }
            }
        }
        #endregion

        #region Patches
        private bool ProjectPatch()
        {
            var dataTable = Source.GetTable(Enums.Tables.Project);

            if (Source.MyConnection == null)
            {
                MsgClass.Danger("Database connection is not available.");
                return false;
            }

            bool shouldCloseConnection = false;

            var columns = new (string Name, string Type, string Default)[]
            {
                ("Client", "INT64", "0"),
                ("ActualCost", "DECIMAL", "0.00"),            // DECIMAL NOT NULL DEFAULT (0.00)
                ("Budget", "DECIMAL", "0.00"),                // DECIMAL NOT NULL DEFAULT (0.00)
                ("Location", "NVARCHAR", "NULL"),
                ("StartDate", "DATETIME", "NULL"),
                ("EndDate", "DATETIME", "NULL"),
                ("IsActive", "BOOLEAN", "True"),
                ("IsCompleted", "BOOLEAN", "False"),
                ("ProjectManager", "INT64", "0"),
                ("Terms", "NVARCHAR", "NULL")
            };

            try
            {
                foreach (var col in columns)
                {
                    if (!dataTable.Columns.Contains(col.Name))
                    {
                        string query = string.Empty;

                        if (col.Default == "NULL")
                        { query = $"ALTER TABLE Project ADD COLUMN {col.Name} {col.Type};"; } // nullable
                        else
                        { query = $"ALTER TABLE Project ADD COLUMN {col.Name} {col.Type} NOT NULL DEFAULT {col.Default};"; }
                        QueryExecutor(query, $"{col.Name} added successfully in Table Project");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MsgClass.Error(ex);
                return false;
            }
            finally
            {
                try
                {
                    if (shouldCloseConnection && Source.MyConnection?.State == System.Data.ConnectionState.Open)
                    {
                        Source.MyConnection.Close();
                    }
                }
                catch (Exception closeEx)
                {
                    MsgClass.Error(closeEx);
                }
            }
        }
        private bool CustomerAddress3()
        {
            // 1. Better naming - method name describes what it does
            var dataTable = Source.GetTable(Enums.Tables.Customers);

            // 2. Check if column exists first (more efficient than querying the DB)
            if (dataTable.Columns.Contains("Address3"))
            {
                return true; // Column already exists
            }

            // 3. Validate connection
            if (Source.MyConnection == null)
            {
                MsgClass.Danger("Database connection is not available.");
                return false;
            }

            // 4. Ensure proper resource management
            bool shouldCloseConnection = false;

            try
            {
                // Check and manage connection state
                if (Source.MyConnection.State != System.Data.ConnectionState.Open)
                {
                    Source.MyConnection.Open();
                    shouldCloseConnection = true; // Track that we opened it
                }

                // 5. Use parameterized/standard SQL (though no parameters here)
                // Consider making column length configurable
                const string commandText = @"ALTER TABLE [Customers] 
                                     ADD COLUMN [Address3] NVARCHAR(60);";

                using (var command = new SqliteCommand(commandText, Source.MyConnection))
                {
                    // 6. ALTER TABLE doesn't typically return affected rows in SQLite
                    // ExecuteNonQuery returns 0 for DDL statements in SQLite
                    command.ExecuteNonQuery();

                    // 7. Refresh the schema cache
                    if (dataTable.Columns.Contains("Address3") == false)
                    {
                        // Force refresh of schema
                        // Depending on your DataTable implementation, you might need:
                        // dataTable.Columns.Add("Address3", typeof(string));
                        // Or refresh from database
                    }

                    MsgClass.Success("Column 'Address3' added to [Customers] table successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MsgClass.Error(ex);
                return false;
            }
            finally
            {
                // 9. Clean up connection state if we opened it
                try
                {
                    if (shouldCloseConnection && Source.MyConnection?.State == System.Data.ConnectionState.Open)
                    {
                        Source.MyConnection.Close();
                    }
                }
                catch (Exception closeEx)
                {
                    MsgClass.Error(closeEx);
                }
            }
        }
        public bool BillReceivable2_AddUnit()
        {
            var _DataTable = Source.GetTable(AppliedDB.Enums.Tables.BillReceivable2);
            if (_DataTable.Columns.Contains("Unit")) return true; // Column already exists
            if (Source.MyConnection == null) { return false; }

            try
            {
                if (Source.MyConnection.State != System.Data.ConnectionState.Open) { Source.MyConnection.Open(); }
                var _CommandText = "ALTER TABLE [BillReceivable2] ADD COLUMN Unit INT;";
                var _Command = new SqliteCommand(_CommandText, Source.MyConnection);
                int _effected = _Command.ExecuteNonQuery();
                if (_effected > 0)
                {
                    MsgClass.Success("Column 'Unit' added to [BillReceivable2] table successfully.");
                    return true;
                }
                else
                {
                    MsgClass.Danger("Column 'Unit' NOT added to [BillReceivable2] table successfully.");
                    return false;
                }
            }
            catch (Exception error)
            {
                MsgClass.Error(error);
                return false;
            }
        }
        public bool BillPayable2_AddUnit()
        {
            var _DataTable = Source.GetTable(Enums.Tables.BillPayable2);
            if (_DataTable.Columns.Contains("Unit")) return true; // Column already exists
            if (Source.MyConnection == null) { return false; }

            try
            {
                if (Source.MyConnection.State != System.Data.ConnectionState.Open) { Source.MyConnection.Open(); }
                var _CommandText = "ALTER TABLE [BillPayable2] ADD COLUMN Unit INT;";
                var _Command = new SqliteCommand(_CommandText, Source.MyConnection);
                int _effected = _Command.ExecuteNonQuery();
                if (_effected > 0)
                {
                    MsgClass.Success("Column 'Unit' added to [BillPayable2] table successfully.");
                    return true;
                }
                else
                {
                    MsgClass.Danger("Column 'Unit' NOT added to BillPayable2 table successfully.");
                    return false;
                }
            }
            catch (Exception error)
            {
                MsgClass.Error(error);
                return false;
            }
        }
        public bool InventoryPatches()
        {
            string _Query1 = @"ALTER TABLE [Inventory] ADD COLUMN [Size] INT64;";
            string _Query2 = @"ALTER TABLE [Inventory] ADD COLUMN [Brand]  NVARCHAR(60);";
            string _Query3 = @"ALTER TABLE [Inventory] ADD COLUMN [Model]  NVARCHAR(60);";

            var _result1 = QueryExecutor(_Query1, "Add [Size] Column in [Inventory] Table.");
            var _result2 = QueryExecutor(_Query2, "Add [Brand] Column in [Inventory] Table.");
            var _result3 = QueryExecutor(_Query3, "Add [Model] Column in [Inventory] Table.");

            if (_result1 && _result2 && _result3) { return true; }
            return false;
        }

        /// <summary>
        /// Drops the view if it exists
        /// </summary>
        private bool DropViewIfExists(string viewName)
        {
            try
            {
                if (Source.MyConnection == null)
                {
                    MsgClass.Danger("Database connection is not available.");
                    return false;
                }

                string query = $"DROP VIEW IF EXISTS {viewName}";
                return QueryExecutor(query, $"View '{viewName}' dropped successfully.");
            }
            catch (Exception ex)
            {
                MsgClass.Error($"Error dropping view '{viewName}': {ex.Message}");
                return false;
            }
        }
        public bool AlterReceipt2Columns()
        {
            try
            {
                if (Source.MyConnection == null)
                {
                    MsgClass.Danger("Database connection is not available.");
                    return false;
                }

                // Try to alter Inv_No column
                try
                {
                    string alterQuery = "ALTER TABLE Receipt2 ALTER COLUMN Inv_No INT64";
                    QueryExecutor(alterQuery, "Altered Inv_No to INT64");
                }
                catch (Exception ex)
                {
                    MsgClass.Warning($"Could not alter Inv_No: {ex.Message}");
                }

                // Try to alter Account column
                try
                {
                    string alterQuery = "ALTER TABLE Receipt2 ALTER COLUMN Account INT64";
                    QueryExecutor(alterQuery, "Altered Account to INT64");
                }
                catch (Exception ex)
                {
                    MsgClass.Warning($"Could not alter Account: {ex.Message}");
                }

                return true;
            }
            catch (Exception ex)
            {
                MsgClass.Error(ex);
                return false;
            }
        }

        #endregion
    }
}

