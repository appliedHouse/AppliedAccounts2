using Microsoft.Data.Sqlite;
using System.Data;
using System.Net.Quic;

namespace AppliedAccounts.Services
{
    public static class AppliedUsersService
    {
        // Path is set once at startup — see Initialize() below
        private static string _usersDbFile = "";

        public static string UsersDBFile => _usersDbFile;
        public static bool IsReady => GetStatus();

        private static bool GetStatus()
        {
            var _Connection = CreateConnection();
            return _Connection == null ? false : true;
        }

        public static void Initialize(IWebHostEnvironment env)
        {
            _usersDbFile = Path.Combine(env.ContentRootPath, "wwwroot", "System", "AppliedUsers2.db");

        }

        public static SqliteConnection? CreateConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(_usersDbFile))
                    throw new InvalidOperationException(
                        "AppliedUsersService.Initialize() was not called at startup.");

                var conn = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = _usersDbFile,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Cache = SqliteCacheMode.Shared,
                    Pooling = true,
                    DefaultTimeout = 15
                }.ToString());

                conn.Open();
                return conn;
            }
            catch (Exception)
            {
                _usersDbFile = string.Empty;
            }
            return null;
        }

        public static SqliteConnection? ClientConnection(string _ClientID)
        {
            if(string.IsNullOrEmpty(_ClientID)) { return null; }

            DataRow? _Row = GetUserRow(_ClientID);
            if (_Row != null)
            {
                
                var _File = _Row.Field<string>("DataFile"); if(string.IsNullOrEmpty(_File)) { return null; }
                var _DataFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SQLiteDB", _File);
                var conn = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = _DataFile,
                    Mode = SqliteOpenMode.ReadWrite,
                    Cache = SqliteCacheMode.Shared,
                    Pooling = true,
                    DefaultTimeout = 15
                }.ToString());

                conn.Open();
                return conn;
            }
            return null;
        }

        public static DataRow? GetUserRow(string UserID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserID)) { return null; }

                using var _connection = CreateConnection();
                if (_connection != null)
                {
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = "SELECT * FROM [Users] WHERE UserID = @UserID LIMIT 1";

                    var p = cmd.CreateParameter();
                    p.ParameterName = "@UserID";
                    p.SqliteType = SqliteType.Text;   // explicit affinity
                    p.Value = UserID;
                    cmd.Parameters.Add(p);

                    using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    var table = new DataTable();
                    table.Load(reader);

                    return table.Rows.Count > 0 ? table.Rows[0] : null;
                }
            }
            catch (Exception)
            {
                _usersDbFile = string.Empty;
            }
            return null;
        }

        /// <summary>
        /// Optional convenience: run a scalar query without exposing the
        /// connection to the caller.
        /// </summary>
        public static T? QuerySingle<T>(
            string sql,
            Func<SqliteDataReader, T> map,
            params (string name, object? value)[] args)
        {
            using var conn = CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            foreach (var (n, v) in args)
                cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
            using var r = cmd.ExecuteReader();
            return r.Read() ? map(r) : default;
        }
    }
}