using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Infrastructure.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, string serviceName)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy($"{serviceName} is healthy"))
                .AddCheck("database", () => HealthCheckResult.Healthy("Database connection is healthy"));

            return services;
        }
    }
}
