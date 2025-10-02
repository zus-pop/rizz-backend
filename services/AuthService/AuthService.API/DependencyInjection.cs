using AuthService.Application.Handlers;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly("AuthService.API")));
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));
        return services;
    }
}
