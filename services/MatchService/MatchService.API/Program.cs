using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MatchService.Application;
using MatchService.Infrastructure;
using MatchService.Infrastructure.Data;
using MatchService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Match Service API", 
        Version = "v1",
        Description = "API for managing swipes, matches, and user interactions"
    });
});

// Configure Clean Architecture layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add RabbitMQ Service
builder.Services.AddSingleton<RabbitMqService>();

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
        var context = scope.ServiceProvider.GetRequiredService<MatchDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Starting database migration for MatchService...");
        await context.Database.MigrateAsync();
        
        logger.LogInformation("MatchService database migration completed successfully");
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Match Service API v1");
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
