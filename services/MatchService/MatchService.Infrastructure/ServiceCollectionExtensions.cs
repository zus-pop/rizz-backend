using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MatchService.Application.Interfaces;
using MatchService.Infrastructure.Data;
using MatchService.Infrastructure.Repositories;

namespace MatchService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<MatchDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("MatchService.Infrastructure");
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                });
                
                // Add logging and sensitive data logging for development
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Register repositories
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<ISwipeRepository, SwipeRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register health checks
            services.AddHealthChecks()
                .AddDbContextCheck<MatchDbContext>("matchdb");

            return services;
        }
    }
}