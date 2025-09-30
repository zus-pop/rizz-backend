using Microsoft.Extensions.DependencyInjection;
using PushService.Application.Services;

namespace PushService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add Application services
        services.AddScoped<IDeviceTokenService, DeviceTokenService>();
        
        return services;
    }
}