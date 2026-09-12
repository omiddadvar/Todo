using Microsoft.EntityFrameworkCore;
using Todo.Identity.Infrastructure;

namespace Todo.Identity;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var pending = await dbContext.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                logger.LogInformation("Applying {Count} pending migration(s): {Migrations}",
                    pending.Count(), string.Join(", ", pending));
            }

            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database is up to date.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

        return app;
    }
}
