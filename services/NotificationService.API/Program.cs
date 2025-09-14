using Microsoft.EntityFrameworkCore;
using NotificationService.API.Data;
using NotificationService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddDbContext<NotificationDbContext>(opt =>
{
    opt.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
    opt.EnableSensitiveDataLogging();
    opt.EnableDetailedErrors();
});

builder.Services.AddHostedService<RabbitMqListener>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<NotificationDbContext>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Notification Service API", 
        Version = "v1",
        Description = "API for managing user notifications and messaging"
    });
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Log the connection string for debugging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

// Ensure database is created and migrations are applied
await EnsureDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static async Task EnsureDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Wait for database to be available with retry logic
    var maxRetries = 10;
    var retryDelay = TimeSpan.FromSeconds(5);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
            
            // Test database connectivity
            await context.Database.CanConnectAsync();
            logger.LogInformation("Database connection successful");
            
            // Apply migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations completed successfully");
            break;
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            logger.LogWarning(ex, "Database connection failed (attempt {Attempt}/{MaxRetries}): {Error}. Retrying in {Delay} seconds...", 
                i + 1, maxRetries, ex.Message, retryDelay.TotalSeconds);
            await Task.Delay(retryDelay);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to database after {MaxRetries} attempts", maxRetries);
            throw; // Re-throw after all retries exhausted
        }
    }
}
