using Microsoft.Data.Sqlite;

namespace AppliedDB
{
    public static class SqliteConnect
    {
        public static SqliteConnection? GetSqliteConnection(string DBFile)
        {
            try
            {
                if (!File.Exists(DBFile))
                {
                    return null;
                }
                SqliteConnectionStringBuilder _ConnectionString = new()
                {
                    DataSource = DBFile,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Cache = SqliteCacheMode.Default,
                    ForeignKeys = true
                };
                SqliteConnection _Connection = new(_ConnectionString.ToString());
                _Connection.Open();
                return _Connection;
            }
            catch
            {
                return null;
            }
        }

    }
}
