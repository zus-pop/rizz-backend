using Microsoft.EntityFrameworkCore;
using AiInsightsService.Infrastructure.Data;
using AiInsightsService.API;

var builder = WebApplication.CreateBuilder(args);

// Register Clean Architecture dependencies
builder.Services.AddAiInsightsService(builder.Configuration);

// Add Controllers
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AiInsightsDbContext>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "AI Insights Service API", 
        Version = "v1",
        Description = "API for managing AI-generated user insights and compatibility analysis"
    });
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

var app = builder.Build();

// Log the connection string for debugging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

// Ensure database is created and migrations are applied
await EnsureDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

app.UseAuthorization();

// Map health checks
app.MapHealthChecks("/health");

app.MapControllers();

// Configure to listen on all interfaces
app.Urls.Add("http://0.0.0.0:8080");

app.Run();

static async Task EnsureDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AiInsightsDbContext>();
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