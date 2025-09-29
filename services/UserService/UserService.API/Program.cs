using UserService.API.Extensions;
using UserService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "User Service API", 
        Version = "v2.0",
        Description = "API for managing user profiles, preferences, and location data - Clean Architecture + DDD + CQRS"
    });
});

// Add UserService specific services
builder.Services.AddUserServices(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Database migration with retry logic
for (int retry = 0; retry < 5; retry++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Starting database migration for UserService...");
        
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
        await context.Database.MigrateAsync();
        
        // Seed sample data
        logger.LogInformation("Seeding sample data...");
        await SampleDataSeeder.SeedSampleDataAsync(context);
        
        logger.LogInformation("UserService database migration completed successfully");
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API v2.0");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseRouting();
app.UseAuthorization();

// Health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                exception = x.Value.Exception?.Message,
                duration = x.Value.Duration
            }),
            totalDuration = report.TotalDuration
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapControllers();

// Configure to listen on all interfaces
app.Urls.Add("http://0.0.0.0:8080");

app.Run();