using AppliedAccounts.Authentication;
using Microsoft.Data.Sqlite;

namespace AppliedAccounts.Middleware
{
    public class UserDatabaseValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static bool _validationPerformed = false;
        private static readonly Lock _lock = new(); // thread-safe check

        public UserDatabaseValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserAuthenticationStateProvider authProvider)
        {




            // Run validation only once per application startup
            if (!_validationPerformed)
            {

                if (string.IsNullOrEmpty(authProvider.AppUser.DataFile))
                //if (authProvider.Claims.Count() > 0)
                {
                    await _next(context);
                    return; // user not logged in yet
                }

                lock (_lock)
                {




                    if (!_validationPerformed)
                    {
                        try
                        {
                            var dataFile = authProvider.AppUser.DataFile;
                            if (string.IsNullOrEmpty(dataFile))
                                throw new Exception("User DataFile not assigned.");

                            var dbPath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                "SqliteDB",
                                dataFile
                            );

                            if (!File.Exists(dbPath))
                                throw new Exception($"User database file '{dataFile}' not found.");

                            // Try opening a connection
                            using var conn = new SqliteConnection($"Data Source={dbPath}");
                            conn.Open(); // Will throw if invalid/corrupt

                            _validationPerformed = true; // mark success
                        }
                        catch
                        {
                            // Redirect to error page if DB invalid
                            if (!context.Request.Path.StartsWithSegments("/Error"))
                            {
                                context.Response.Redirect("/Error");
                                return;
                            }
                        }
                    }
                }
            }

            // Continue pipeline
            await _next(context);
        }
    }

    // Extension method to register middleware
    public static class UserDatabaseValidationExtensions
    {
        public static IApplicationBuilder UseUserDatabaseValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserDatabaseValidationMiddleware>();
        }
    }
}
