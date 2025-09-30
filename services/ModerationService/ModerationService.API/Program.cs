using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ModerationService.Application;
using ModerationService.Infrastructure;
using ModerationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Moderation Service API", 
        Version = "v1",
        Description = "API for content moderation, user blocking, and reporting"
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
    .AddDbContextCheck<ModerationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Moderation Service API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Database initialization
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
            var context = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();
            
            Console.WriteLine($"Attempting to connect to database (attempt {i + 1}/{maxRetries})...");
            // Comment out migration since tables already exist
            // await context.Database.MigrateAsync();
            await context.Database.CanConnectAsync(); // Just check connection
            Console.WriteLine("Database connection verified successfully!");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection attempt {i + 1} failed: {ex.Message}");
            
            if (i == maxRetries - 1)
            {
                Console.WriteLine("Max retries reached. Database connection failed.");
                // Don't throw in development to allow the service to start without DB
                if (!services.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    throw;
                }
                Console.WriteLine("Development mode: continuing without database...");
                return;
            }
            
            Console.WriteLine($"Retrying in {delayBetweenRetriesMs / 1000} seconds...");
            await Task.Delay(delayBetweenRetriesMs);
        }
    }
}
