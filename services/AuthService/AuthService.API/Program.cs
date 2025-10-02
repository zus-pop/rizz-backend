using AuthService.Infrastructure.Data;
using AuthService.API;
using AuthService.API.Middleware;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var key = jwtSection["Key"] ?? "default_secret_key_for_development_minimum_32_characters_long";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"] ?? "AuthService",
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"] ?? "AuthService",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Authentication Service API",
        Version = "v1",
        Description = "API for user authentication, registration, and OTP verification"
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

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

try {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    // Test basic connection first
    var canConnect = await context.Database.CanConnectAsync();
    if (canConnect) {
        // Skip migration for now since tables are manually created
        // await context.Database.MigrateAsync();
        Console.WriteLine("Database connection successful - tables already exist");
        
        // Seed sample data with proper BCrypt hashes
        await AuthSampleDataSeeder.SeedSampleDataAsync(context);
        Console.WriteLine("Sample data seeded successfully");
    } else {
        Console.WriteLine("Cannot connect to database - will continue without DB");
    }
}
catch (Exception ex) {
    Console.WriteLine($"Database setup failed: {ex.Message}");
}

// Firebase initialization
try {
    var firebaseConfig = builder.Configuration.GetSection("Firebase");
    var firebaseProjectId = firebaseConfig["ProjectId"];
    
    if (!string.IsNullOrEmpty(firebaseProjectId)) {
        if (FirebaseApp.DefaultInstance == null) {
            GoogleCredential credential;
            
            // Try credentials JSON first, then file path, then default
            var credentialsJson = firebaseConfig["CredentialsJson"];
            var credentialsPath = firebaseConfig["CredentialsPath"];
            
            if (!string.IsNullOrEmpty(credentialsJson)) {
                credential = GoogleCredential.FromJson(credentialsJson);
                Console.WriteLine("Firebase initialized with JSON credentials");
            } else if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath)) {
                credential = GoogleCredential.FromFile(credentialsPath);
                Console.WriteLine($"Firebase initialized with credentials file: {credentialsPath}");
            } else {
                credential = GoogleCredential.GetApplicationDefault();
                Console.WriteLine("Firebase initialized with application default credentials");
            }
            
            FirebaseApp.Create(new AppOptions() {
                Credential = credential,
                ProjectId = firebaseProjectId
            });
            Console.WriteLine($"Firebase initialized successfully with project ID: {firebaseProjectId}");
        }
    } else {
        Console.WriteLine("Warning: Firebase ProjectId not configured. Firebase authentication features will be limited.");
    }
} catch (Exception ex) {
    Console.WriteLine($"Firebase initialization warning: {ex.Message}");
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://0.0.0.0:8080");
app.Run();
