using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PushService.Infrastructure.Data;
using PushService.Domain.Repositories;
using PushService.Infrastructure.Repositories;
using PushService.Infrastructure.Services;
using PushService.Application.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Push Service API", Version = "v2.0" });
    // Add JWT bearer definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {token}'"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] {}
        }
    });
});

// Database configuration
var connectionString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "postgres"};" +
      $"Database={Environment.GetEnvironmentVariable("POSTGRES_PUSH_DB") ?? "push_db"};" +
      $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres"};" +
      $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "123456"};" +
      $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"}";

builder.Services.AddDbContext<PushDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("PushService.Infrastructure");
    }));

// Repository registration
builder.Services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
builder.Services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();

// MediatR registration
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(PushService.Application.Commands.RegisterDeviceTokenCommand).Assembly);
});

// AutoMapper registration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PushDbContext>();

// Firebase initialization
try
{
    var firebaseProjectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");
    if (!string.IsNullOrEmpty(firebaseProjectId))
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = firebaseProjectId
            });
        }
    }
    else
    {
        Console.WriteLine("Warning: FIREBASE_PROJECT_ID not set. Firebase features will be limited.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Firebase initialization warning: {ex.Message}");
}

var app = builder.Build();

// Database migration
for (int retry = 0; retry < 5; retry++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PushDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Starting database migration for PushService...");
        
        // Check if database can be connected to
        var canConnect = await context.Database.CanConnectAsync();
        logger.LogInformation("Database connection test: {CanConnect}", canConnect);
        
        if (!canConnect)
        {
            logger.LogError("Cannot connect to database");
            throw new Exception("Database connection failed");
        }
        
        // Apply all migrations (this will create the database if it doesn't exist)
        logger.LogInformation("Applying database migrations...");
        
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception migrationEx) when (migrationEx.Message.Contains("already exists"))
        {
            logger.LogWarning("DeviceTokens table already exists. Dropping and recreating...");
            
            // Drop the existing table and recreate
            await context.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS \"DeviceTokens\"");
            await context.Database.MigrateAsync();
        }
        
        logger.LogInformation("PushService database migration completed successfully");
        break;
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed, attempt {Retry}/5", retry + 1);
        if (retry == 4) throw;
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Push Service API v2.0");
    });
}

app.UseRouting();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();