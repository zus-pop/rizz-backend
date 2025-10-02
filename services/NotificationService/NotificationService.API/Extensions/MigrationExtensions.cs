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
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied successfully");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while applying database migrations");
            throw;
        }
    }
}