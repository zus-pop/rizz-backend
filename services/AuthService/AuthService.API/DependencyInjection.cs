using AuthService.Application.Handlers;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace AuthService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration config)
    {
        // Explicitly get connection string with fallback
        var connectionString = config.GetConnectionString("Postgres") 
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Port=5432;Database=auth_db;Username=postgres;Password=123456";
            
        Console.WriteLine($"[AuthService] Using connection string: {connectionString}");
            
        services.AddDbContext<AuthDbContext>(options => {
            options.UseNpgsql(connectionString, 
                b => b.MigrationsAssembly("AuthService.Infrastructure"));
            // // Add logging for database operations
            // options.EnableSensitiveDataLogging();
            // options.LogTo(Console.WriteLine);
        });
        
        // Repository registrations
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
        
        // Service registrations
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailService, EmailService>();
        
        // MediatR registration
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));
        
        // Rate limiting configuration
        services.AddRateLimiter(options =>
        {
            // General auth endpoints rate limiting
            options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 2;
            });
            
            // OTP endpoints rate limiting
            options.AddFixedWindowLimiter("OtpPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 3;
                limiterOptions.Window = TimeSpan.FromMinutes(5);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });
        });
        
        return services;
    }
}
