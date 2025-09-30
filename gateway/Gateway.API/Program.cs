using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Gateway.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add HttpClient for Swagger proxy middleware
builder.Services.AddHttpClient();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Swagger services for Gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Dating App YARP Gateway API", 
        Version = "v1",
        Description = "YARP-powered unified API Gateway for all microservices with health checks and load balancing"
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

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

// Add health checks for Gateway itself and upstream services
builder.Services.AddHealthChecks()
    .AddCheck("gateway_health", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Gateway is running"))
    .AddUrlGroup(new Uri("http://aiinsights:8080/health"), "aiinsights")
    .AddUrlGroup(new Uri("http://authservice:8080/health"), "authservice")
    .AddUrlGroup(new Uri("http://matchservice:8080/health"), "matchservice")
    .AddUrlGroup(new Uri("http://messagingservice:8080/health"), "messagingservice")
    .AddUrlGroup(new Uri("http://moderationservice:8080/health"), "moderationservice")
    .AddUrlGroup(new Uri("http://purchaseservice:8080/health"), "purchaseservice")
    .AddUrlGroup(new Uri("http://pushservice:8080/health"), "pushservice")
    .AddUrlGroup(new Uri("http://userservice:8080/health"), "userservice")
    .AddUrlGroup(new Uri("http://notificationservice:8080/health"), "notificationservice");

// Add HealthChecks UI
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(30);
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.SetApiMaxActiveRequests(1);
    options.AddHealthCheckEndpoint("Gateway Health", "/health");
}).AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments for containerized development
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "YARP Gateway API v1");
    
    // Add endpoints for each microservice (proxied through YARP)
    c.SwaggerEndpoint("/api-docs/aiinsights.json", "AI Insights Service");
    c.SwaggerEndpoint("/api-docs/auth.json", "Auth Service");  
    c.SwaggerEndpoint("/api-docs/match.json", "Match Service");
    c.SwaggerEndpoint("/api-docs/messaging.json", "Messaging Service");
    c.SwaggerEndpoint("/api-docs/moderation.json", "Moderation Service");
    c.SwaggerEndpoint("/api-docs/purchase.json", "Purchase Service");
    c.SwaggerEndpoint("/api-docs/push.json", "Push Service");
    c.SwaggerEndpoint("/api-docs/user.json", "User Service");
    c.SwaggerEndpoint("/api-docs/notification.json", "Notification Service");
    
    c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
    
    // Configure Swagger UI for better experience
    c.DisplayRequestDuration();
    c.EnableTryItOutByDefault();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    c.DefaultModelsExpandDepth(-1);
});

// Use CORS
app.UseCors("AllowAll");

// Use Swagger proxy middleware for service documentation
app.UseMiddleware<SwaggerProxyMiddleware>();

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure Gateway health endpoint with detailed service status
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        proxy = "Microsoft YARP",
        services = new[] {
            "aiinsights", "auth", "match", "messaging", 
            "moderation", "purchase", "push", "user", "notification"
        }
    });
});

// Map health checks with detailed reporting
app.MapHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
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
                duration = x.Value.Duration,
                data = x.Value.Data
            }),
            totalDuration = report.TotalDuration
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Add HealthChecks UI
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-api";
});

app.UseRouting();

// Map controllers for any Gateway-specific endpoints
app.MapControllers();

// Map the reverse proxy - YARP will handle all routing based on appsettings.json configuration
app.MapReverseProxy();

// Configure to listen on all interfaces (use port 5000 for gateway)
app.Urls.Add("http://0.0.0.0:5000");

app.Run();