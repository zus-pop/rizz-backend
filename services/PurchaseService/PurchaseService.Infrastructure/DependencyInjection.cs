using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PurchaseService.Application.Repositories;
using PurchaseService.Domain.Services;
using PurchaseService.Infrastructure.Persistence;
using PurchaseService.Infrastructure.Repositories;
using PurchaseService.Infrastructure.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PurchaseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<PurchaseDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(PurchaseDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
            
            // Enable sensitive data logging in development
            var enableSensitiveDataLogging = configuration.GetSection("Logging")["EnableSensitiveDataLogging"];
            if (bool.TryParse(enableSensitiveDataLogging, out var result) && result)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Repositories
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        // Domain Services - Use fully qualified names to avoid ambiguity
        services.AddScoped<IPaymentProcessor, Infrastructure.Services.PaymentProcessor>();
        services.AddScoped<ISubscriptionCalculator, Infrastructure.Services.SubscriptionCalculator>();

        // Logging
        services.AddSingleton<ILogger>(provider =>
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        });

        return services;
    }

    public static async Task<IServiceProvider> MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PurchaseDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        
        try
        {
            logger.Information("Starting database migration for PurchaseService...");
            
            // Test connection first
            logger.Information("Testing database connection...");
            await context.Database.CanConnectAsync();
            logger.Information("Database connection successful");
            
            // Check if database exists
            logger.Information("Checking if database exists...");
            var canConnect = await context.Database.CanConnectAsync();
            if (canConnect)
            {
                logger.Information("Database exists and is accessible");
            }
            else
            {
                logger.Warning("Database does not exist or is not accessible");
            }
            
            // Apply migrations
            logger.Information("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.Information("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while migrating the database: {ErrorMessage}", ex.Message);
            throw;
        }

        return serviceProvider;
    }
}