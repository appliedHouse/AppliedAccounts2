using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Middleware
{
    public class DatabasePatches
    {
        private readonly RequestDelegate _next;
        private readonly string _connectionString;

        public DatabasePatches(RequestDelegate next)
        {
            _next = next;

            var userFile = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "SqliteDB",
                "AppliedUser2.db"
            );

            _connectionString = $"Data Source={userFile}";
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                await conn.OpenAsync();   // ✅ Connection test
            }
            catch (Exception ex)
            {
                // ❌ Connection failed → show error page
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(
                    "Database connection failed. Please contact administrator."
                );
                return;
            }

            // ✅ Continue pipeline
            await _next(context);
        }
    }

    public static class DatabasePatchesExtensions
    {
        public static IApplicationBuilder UseDatabasePatches(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatabasePatches>();
        }
    }
}
