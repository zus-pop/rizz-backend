using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Swagger services for Gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Dating App YARP Gateway API", 
        Version = "v1",
        Description = "YARP-powered unified API Gateway for all microservices with health checks and load balancing"
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

// Add health checks for Gateway itself
builder.Services.AddHealthChecks();

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
    c.ConfigObject.DeepLinking = true;
    c.ConfigObject.TryItOutEnabled = true;
    c.ConfigObject.DisplayRequestDuration = true;
});

// Use CORS
app.UseCors("AllowAll");

// Add Gateway health check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            service = "Gateway",
            timestamp = DateTime.UtcNow,
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

// Add a Gateway status endpoint
app.MapGet("/gateway/status", () => new { 
    gateway = "YARP Gateway",
    version = "1.0.0",
    status = "healthy",
    timestamp = DateTime.UtcNow,
    proxy = "Microsoft YARP",
    services = new[] {
        "aiinsights", "auth", "match", "messaging", 
        "moderation", "purchase", "push", "user", "notification"
    }
});

app.UseRouting();
app.UseAuthorization();

// Map controllers for any Gateway-specific endpoints
app.MapControllers();

// Map the reverse proxy - YARP will handle all routing based on appsettings.json configuration
app.MapReverseProxy();

// Configure to listen on all interfaces
app.Urls.Add("http://0.0.0.0:8080");

app.Run();