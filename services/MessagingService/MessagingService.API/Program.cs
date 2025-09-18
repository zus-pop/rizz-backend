using MessagingService.Application;
using MessagingService.Infrastructure;
using MessagingService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Messaging Service API", 
        Version = "v1",
        Description = "API for messaging and chat functionality"
    });
});

// Add Clean Architecture layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MessagingDbContext>();

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

// Ensure database is created and migrated
await EnsureDatabaseAsync(app.Services);

app.Run();

static async Task EnsureDatabaseAsync(IServiceProvider services)
{
    const int maxRetries = 10;
    const int delayBetweenRetriesMs = 3000;
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
            
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
