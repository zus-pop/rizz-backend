using FluentValidation;
using MediatR;
using UserService.Application.Services;
using UserService.Application.Validators;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace UserService.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Database
            services.AddDbContext<UserDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Postgres");
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    npgsqlOptions.UseNetTopologySuite(); // Enable PostGIS spatial data support
                    npgsqlOptions.MigrationsAssembly("UserService.Infrastructure"); // Specify migrations assembly
                });
            });

            // Add Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IPreferenceRepository, PreferenceRepository>();

            // Add Services
            services.AddScoped<IFileService, FileService>();

            // Add MediatR
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(typeof(UserService.Application.Commands.CreateUserCommand).Assembly);
            });

            // Add AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Add FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

            // Add Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<UserDbContext>("database");

            return services;
        }
    }
}