using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Data;

namespace NotificationService.API.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        
        try
        {
            app.Logger.LogInformation("Checking for pending migrations...");
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            
            if (pendingMigrations.Any())
            {
                app.Logger.LogInformation("Found {Count} pending migrations: {Migrations}", pendingMigrations.Count, string.Join(", ", pendingMigrations));
                await context.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                app.Logger.LogInformation("No pending migrations found");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while applying database migrations");
            throw;
        }
    }
}