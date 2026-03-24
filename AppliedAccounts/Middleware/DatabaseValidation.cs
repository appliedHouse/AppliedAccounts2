using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Middleware
{
    public class DatabaseValidation
    {
        private readonly RequestDelegate _next;

        private static bool _isDatabaseValid = false;
        private static bool _validationPerformed = false;
        private static readonly Lock  _lock = new();

        public DatabaseValidation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Run validation only once (Thread Safe)
            if (!_validationPerformed)
            {
                lock (_lock)
                {
                    if (!_validationPerformed)
                    {
                        string dbPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "SqliteDB",
                            "AppliedUsers2.db"
                        );

                        try
                        {
                            // Check file exists
                            if (File.Exists(dbPath))
                            {
                                // Check SQLite connection
                                using var conn = new SqliteConnection($"Data Source={dbPath}");
                                conn.Open();

                                _isDatabaseValid = true;
                            }
                            else
                            {
                                _isDatabaseValid = false;
                            }
                        }
                        catch
                        {
                            _isDatabaseValid = false;
                        }

                        _validationPerformed = true;
                    }
                }
            }

            // Redirect to error page if DB invalid
            if (!_isDatabaseValid && !context.Request.Path.StartsWithSegments("/Error"))
            {
                context.Response.Redirect("/Error");
                return;
            }

            await _next(context);
        }
    }

    // Extension Method
    public static class DatabaseValidationExtensions
    {
        public static IApplicationBuilder UseDatabaseValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatabaseValidation>();
        }
    }
}
