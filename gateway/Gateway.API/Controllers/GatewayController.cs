using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            service = "Gateway",
            status = "running",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            features = new[]
            {
                "YARP Reverse Proxy",
                "JWT Authentication",
                "Swagger Aggregation", 
                "Health Checks",
                "CORS Support"
            }
        });
    }

    [HttpGet("routes")]
    public IActionResult GetRoutes()
    {
        var routes = new
        {
            services = new[]
            {
                new { name = "AuthService", path = "/api/auth/*", port = 5002, swagger = "/api-docs/auth.json" },
                new { name = "UserService", path = "/api/users/*", port = 5008, swagger = "/api-docs/user.json" },
                new { name = "MatchService", path = "/api/matches/*", port = 5003, swagger = "/api-docs/match.json" },
                new { name = "MessagingService", path = "/api/messages/*", port = 5004, swagger = "/api-docs/messaging.json" },
                new { name = "NotificationService", path = "/api/notifications/*", port = 5009, swagger = "/api-docs/notification.json" },
                new { name = "ModerationService", path = "/api/moderation/*", port = 5005, swagger = "/api-docs/moderation.json" },
                new { name = "PurchaseService", path = "/api/purchases/*", port = 5006, swagger = "/api-docs/purchase.json" },
                new { name = "PushService", path = "/api/push/*", port = 5007, swagger = "/api-docs/push.json" },
                new { name = "AiInsightsService", path = "/api/ai-insights/*", port = 5001, swagger = "/api-docs/aiinsights.json" }
            },
            documentation = new
            {
                swagger_ui = "/",
                health_check = "/health",
                detailed_health = "/health/detailed",
                health_ui = "/health-ui"
            }
        };

        return Ok(routes);
    }
}