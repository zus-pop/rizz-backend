using AiInsightsService.Application.Handlers;
using AiInsightsService.Domain.Interfaces;
using AiInsightsService.Infrastructure.Data;
using AiInsightsService.Infrastructure.Repositories;
using AiInsightsService.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiInsightsService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAiInsightsService(this IServiceCollection services, IConfiguration config)
    {
        // Database
        services.AddDbContext<AiInsightsDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly("AiInsightsService.API")));

        // Repositories
        services.AddScoped<IAiInsightRepository, AiInsightRepository>();

        // Domain Services
        services.AddScoped<IAiAnalysisService, MockAiAnalysisService>();

        // Background Services (commented out for testing without RabbitMQ)
        // services.AddHostedService<AiRabbitMqListener>();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrUpdateAiInsightCommandHandler).Assembly));

        return services;
    }
}