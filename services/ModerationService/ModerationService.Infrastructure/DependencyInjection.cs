using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModerationService.Application.Interfaces;
using ModerationService.Infrastructure.Data;
using ModerationService.Infrastructure.Repositories;

namespace ModerationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ModerationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IBlockRepository, BlockRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IModerationCaseRepository, ModerationCaseRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}