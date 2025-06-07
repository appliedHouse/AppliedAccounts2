using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace AppliedAccounts.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class DatabasePatches
    {
        private readonly RequestDelegate _next;

        public DatabasePatches(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DatabasePatchesExtensions
    {
        public static IApplicationBuilder UseDatabasePatches(this IApplicationBuilder builder)
        {
            var _UserFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SQLiteDB", "AppliedUser2.db");
            var _Connection = new SQLiteConnection($"Data Source={_UserFile}");
            {
                // Set the connection string properties if needed
                // e.g., Password = "your_password"
            }
            ;
            return builder.UseMiddleware<DatabasePatches>();
        }
    }
}
