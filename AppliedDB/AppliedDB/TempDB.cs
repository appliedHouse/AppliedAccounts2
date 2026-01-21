using Microsoft.Data.Sqlite;
using System.Data;

namespace AppliedDB
{
    public class TempDB
    {
        private SqliteConnection MyConnection { get; set; }
        public string TempDBFile { get; set; }
        public string TableName { get; set; }
        public DataTable TempTable { get; set; }
        public string ErrorMessage { get; set; }

        public TempDB(string _TempDBFile)
        {
            TempDBFile = _TempDBFile;
            string _DBPath = Path.Combine(Connections.GetTempDBPath(), TempDBFile);
            MyConnection = new SqliteConnection($"Data Source={_DBPath}");
        }

        public async Task<DataTable> GetTempTableAsync(string tableName)
        {
            ErrorMessage = string.Empty;
            var dt = new DataTable();


            try
            {
                if (MyConnection.State != ConnectionState.Open)
                    await MyConnection.OpenAsync();

                string query = $"SELECT * FROM [{tableName}]";

                using var cmd = new SqliteCommand(query, MyConnection);
                using var reader = await cmd.ExecuteReaderAsync();

                dt.Load(reader);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return new DataTable(); // return empty on error
            }
            finally
            {
                if (MyConnection.State == ConnectionState.Open)
                    await MyConnection.CloseAsync();
            }

            return dt;
        }

    }
}
