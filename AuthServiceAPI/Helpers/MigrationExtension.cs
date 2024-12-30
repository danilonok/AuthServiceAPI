using AuthServiceAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthServiceAPI.Helpers
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            try
            {
                // Only apply migrations if there are any pending
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Migration error: {ex}");
            }
        }

    }
}
