using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;
// using NotificationService.Infrastructure.Messaging;
// using RabbitMQ.Client;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("NotificationService.API")));

        // Add Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Services
        services.AddScoped<INotificationDeliveryInfrastructure, NotificationDeliveryService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        
        // Configure HttpClient for PushNotificationService
        services.AddHttpClient<IPushNotificationService, PushNotificationService>();
        
        services.AddScoped<NotificationService.Domain.Services.INotificationTemplateService, NotificationService.Domain.Services.NotificationTemplateService>();

        // Add Utility Services
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Add Event Publisher - completely disabled for testing
        services.AddScoped<IEventPublisher, NoOpEventPublisher>();

        return services;
    }
}