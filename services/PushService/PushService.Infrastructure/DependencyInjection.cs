using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PushService.Application.Services;
using PushService.Domain.Repositories;
using PushService.Infrastructure.Data;
using PushService.Infrastructure.Repositories;
using PushService.Infrastructure.Services;

namespace PushService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework
        var connectionString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? configuration.GetConnectionString("DefaultConnection")
            : $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "postgres"};" +
              $"Database={Environment.GetEnvironmentVariable("POSTGRES_PUSH_DB") ?? "push_db"};" +
              $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres"};" +
              $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "123456"};" +
              $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"}";

        services.AddDbContext<PushDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add Repositories
        services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();

        // Add Enhanced Services
        services.AddScoped<IEnhancedPushNotificationService, EnhancedPushNotificationService>();
        
        // Add Background Services
        services.AddHostedService<TokenMaintenanceBackgroundService>();

        return services;
    }
}