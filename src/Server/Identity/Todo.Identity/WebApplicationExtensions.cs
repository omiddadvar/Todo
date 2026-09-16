using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Todo.Identity.Infrastructure.Data;

namespace Todo.Identity;

public static class WebApplicationExtensions
{
    public static WebApplication UseWebAppExtensions(this WebApplication app)
    {
        app.UseWebAppSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("/openapi/{documentName}.json");
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Todo Identity API")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

                options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
            });
        }
        return app;
    }
    public static WebApplication UseWebAppSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            };
        });
        return app;
    }
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
