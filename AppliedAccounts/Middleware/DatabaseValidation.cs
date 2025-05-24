namespace AppliedAccounts.Middleware
{
    public class DatabaseValidation
    {

        private readonly RequestDelegate _next;
        private static bool _isDatabaseValid = false; // Cached validation result
        private static bool _validationPerformed = false; // Ensure validation runs only once

        public DatabaseValidation(RequestDelegate next)
        {
            _next = next;
        }



        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // If validation has already been performed, skip it
            if (!_validationPerformed)
            {
                _validationPerformed = true; // Mark validation as performed

                string dbPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "SQLiteDB",
                    "AppliedUsers2.db");

                // Check if the database file exists
                _isDatabaseValid = File.Exists(dbPath);
            }

            // If the database is invalid, redirect only once
            if (!_isDatabaseValid && !context.Request.Path.StartsWithSegments("/Error"))
            {

                context.Response.Redirect("/Error");
                return;
            }

            // Continue to the next middleware
            await _next(context);
        }
    }
}
