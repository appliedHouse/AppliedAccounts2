using Microsoft.Data.Sqlite;

public class ClientDatabase
{
    private string _connectionString;
    private string _dataFile;
    private readonly IWebHostEnvironment env;



    public string DataFile => _dataFile;
    public bool IsReady => !string.IsNullOrWhiteSpace(_dataFile);

    // Called ONCE at login by CurrentUser.LoadAsync — see below
    public ClientDatabase(IConfiguration cfg, IWebHostEnvironment _env)
    {
        // Set up later via Configure(); placeholder here
        _connectionString = "";
        _dataFile = "";
        env = _env;
    }

    public void Configure(string dataFile)
    {
        if (string.IsNullOrWhiteSpace(dataFile))
            throw new ArgumentException("DataFile is required.", nameof(dataFile));

        // SECURITY: DataFile comes from the DB, but sanitize anyway
        if (dataFile.Contains("..") || dataFile.Contains(Path.DirectorySeparatorChar)
            || dataFile.Contains(Path.AltDirectorySeparatorChar))
            throw new InvalidOperationException($"Invalid DataFile: {dataFile}");

        var fullPath = Path.Combine(env.ContentRootPath, "wwwroot", "SQLiteDB", dataFile);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Client DB not found: {fullPath}");

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            Mode = SqliteOpenMode.ReadWrite,
            Cache = SqliteCacheMode.Private,   // per-file, no cross-tenant leak
            Pooling = true,
            DefaultTimeout = 15
        }.ToString();

        // WAL per client DB, one-time
        using var _connection = new SqliteConnection(_connectionString);
        _connection.Open();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
        cmd.ExecuteNonQuery();
    }

    // Every operation gets a fresh, pooled connection. Thread-safe.
    public SqliteConnection CreateConnection()
    {
        if (!IsReady) throw new InvalidOperationException("Client DB not configured. Log in first.");
        var c = new SqliteConnection(_connectionString);
        c.Open();
        return c;
    }

    public T? QuerySingle<T>(string sql, Func<SqliteDataReader, T> map, params (string name, object? value)[] args)
    {
        using var conn = CreateConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (n, v) in args)
            cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
        using var r = cmd.ExecuteReader();
        return r.Read() ? map(r) : default;
    }

    public List<T> QueryList<T>(string sql, Func<SqliteDataReader, T> map, params (string name, object? value)[] args)
    {
        using var conn = CreateConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (n, v) in args)
            cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
        using var r = cmd.ExecuteReader();
        var list = new List<T>();
        while (r.Read()) list.Add(map(r));
        return list;
    }

    public int Execute(string sql, params (string name, object? value)[] args)
    {
        using var conn = CreateConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (n, v) in args)
            cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
        return cmd.ExecuteNonQuery();
    }
}