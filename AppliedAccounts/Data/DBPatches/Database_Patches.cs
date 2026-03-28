using AppMessages;
using Microsoft.Data.Sqlite;
using Enums = AppliedDB.Enums;

namespace AppliedAccounts.Data
{
    public class Database_Patches
    {
        public AppliedDB.DataSource Source { get; set; }
        public MessageClass MsgClass { get; set; } = new();
        public MessageClass ErrorMsgClass { get; set; } = new();
        public List<bool> IsPatchApplied { get; set; } = [];
        public Database_Patches(AppliedDB.DataSource source)
        {
            Source = source;
            IsPatchApplied.Add(BillReceivable2_AddUnit());
            IsPatchApplied.Add(BillPayable2_AddUnit());
            IsPatchApplied.Add(CustomerAddress3());
            IsPatchApplied.Add(ProjectPatch());
        }

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
                        QueryExecutor(query, $"{col.Name} added successfully in Table Project", Source.MyConnection);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsgClass.Error($"Failed to add columns to Project table: {ex.Message}");
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
                    ErrorMsgClass.Error($"Error closing connection: {closeEx.Message}");
                }
            }
        }

        private bool QueryExecutor(string query, string Message, SqliteConnection _Connection)
        {
            using (var command = new SqliteCommand(query, _Connection))
            {
                try
                {
                    command.ExecuteNonQuery();
                    MsgClass.Add(Message);
                    return true;
                }
                catch (Exception ex)
                {
                    MsgClass.Add($"Error: {ex.Message}");
                    return false;
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

                    MsgClass.Add("Column 'Address3' added to [Customers] table successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                // 8. Better error handling with more context
                string errorMessage = $"Failed to add Address3 column to Customers table: {ex.Message}";
                ErrorMsgClass.Error(errorMessage);

                // Log the full exception for debugging if needed
                // Logger.Error(ex, "AddCustomerAddress3Column failed");

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
                    // Log but don't throw - we want the original exception to propagate
                    ErrorMsgClass.Error($"Error closing connection: {closeEx.Message}");
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
                    MsgClass.Add("Column 'Unit' added to [BillReceivable2] table successfully.");
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
                ErrorMsgClass.Error(error.Message);
                return false;
            }
        }

        public bool BillPayable2_AddUnit()
        {
            var _DataTable = Source.GetTable(AppliedDB.Enums.Tables.BillPayable2);
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
                    MsgClass.Add("Column 'Unit' added to [BillPayable2] table successfully.");
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
                ErrorMsgClass.Error(error.Message);
                return false;
            }
        }
    }
}
