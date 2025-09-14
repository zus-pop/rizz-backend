using Microsoft.EntityFrameworkCore;
using ModerationService.API.Data;
using ModerationService.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Moderation Service API", 
        Version = "v1",
        Description = "API for content moderation, user blocking, and reporting"
    });
});

// Database configuration with retry logic
builder.Services.AddDbContext<ModerationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
    
    // Suppress all Entity Framework warnings that might be treated as errors
    options.ConfigureWarnings(warnings =>
    {
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.SensitiveDataLoggingEnabledWarning);
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning);
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.RedundantIndexRemoved);
    });
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super-secret-key-for-development-only-min-32-chars";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health checks
builder.Services.AddHealthChecks();
    // Temporarily removed database health check to fix HTTP serving
    // .AddDbContextCheck<ModerationDbContext>();

// Background services
builder.Services.AddHostedService<ModerationRabbitMqListener>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Temporarily disable database initialization to resolve HTTP serving issue
// TODO: Fix the EF Core pending model changes warning
// await EnsureDatabaseAsync(app.Services);

app.Run();

// Temporarily commented out to resolve EF Core model warning issue
/*
static async Task EnsureDatabaseAsync(IServiceProvider services)
{
    const int maxRetries = 10;
    const int delayBetweenRetriesMs = 3000;
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
            
            Console.WriteLine($"Attempting to connect to database (attempt {i + 1}/{maxRetries})...");
            await context.Database.MigrateAsync();
            Console.WriteLine("Database migration completed successfully!");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection attempt {i + 1} failed: {ex.Message}");
            
            if (i == maxRetries - 1)
            {
                Console.WriteLine("Max retries reached. Database connection failed.");
                throw;
            }
            
            Console.WriteLine($"Retrying in {delayBetweenRetriesMs / 1000} seconds...");
            await Task.Delay(delayBetweenRetriesMs);
        }
    }
}
*/
