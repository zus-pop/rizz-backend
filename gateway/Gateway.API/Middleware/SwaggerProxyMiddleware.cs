using System.Text.Json;

namespace Gateway.API.Middleware;

public class SwaggerProxyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SwaggerProxyMiddleware> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly Dictionary<string, string> _serviceEndpoints = new()
    {
        { "/api-docs/aiinsights.json", "http://aiinsights:8080/swagger/v1/swagger.json" },
        { "/api-docs/auth.json", "http://authservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/match.json", "http://matchservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/messaging.json", "http://messagingservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/moderation.json", "http://moderationservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/purchase.json", "http://purchaseservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/push.json", "http://pushservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/user.json", "http://userservice:8080/swagger/v1/swagger.json" },
        { "/api-docs/notification.json", "http://notificationservice:8080/swagger/v1/swagger.json" }
    };

    public SwaggerProxyMiddleware(RequestDelegate next, ILogger<SwaggerProxyMiddleware> logger, IHttpClientFactory httpClientFactory)
    {
        _next = next;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path != null && _serviceEndpoints.ContainsKey(path))
        {
            await ProxySwaggerDoc(context, path);
            return;
        }

        await _next(context);
    }

    private async Task ProxySwaggerDoc(HttpContext context, string path)
    {
        try
        {
            var serviceUrl = _serviceEndpoints[path];
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            _logger.LogInformation("Proxying Swagger doc from {ServiceUrl} for path {Path}", serviceUrl, path);

            var response = await httpClient.GetAsync(serviceUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // Modify the swagger doc to include the gateway base path
                var swaggerDoc = JsonSerializer.Deserialize<JsonElement>(content);
                var modifiedContent = ModifySwaggerDoc(swaggerDoc, GetServiceName(path));
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(modifiedContent);
            }
            else
            {
                _logger.LogWarning("Failed to fetch Swagger doc from {ServiceUrl}. Status: {StatusCode}", serviceUrl, response.StatusCode);
                await Return404SwaggerDoc(context, GetServiceName(path));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proxying Swagger doc for path {Path}", path);
            await Return404SwaggerDoc(context, GetServiceName(path));
        }
    }

    private string ModifySwaggerDoc(JsonElement swaggerDoc, string serviceName)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            
            // Create a dictionary to modify the swagger doc
            var doc = JsonSerializer.Deserialize<Dictionary<string, object>>(swaggerDoc.GetRawText());
            
            if (doc != null)
            {
                // Update the title to include "via Gateway"
                if (doc.ContainsKey("info") && doc["info"] is JsonElement infoElement)
                {
                    var info = JsonSerializer.Deserialize<Dictionary<string, object>>(infoElement.GetRawText());
                    if (info != null && info.ContainsKey("title"))
                    {
                        info["title"] = $"{info["title"]} (via Gateway)";
                        doc["info"] = info;
                    }
                }
                
                // Update server URLs to point to the gateway
                doc["servers"] = new[]
                {
                    new { url = "/", description = "Gateway" }
                };
            }
            
            return JsonSerializer.Serialize(doc, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to modify Swagger doc for {ServiceName}", serviceName);
            return swaggerDoc.GetRawText();
        }
    }

    private async Task Return404SwaggerDoc(HttpContext context, string serviceName)
    {
        var errorDoc = new
        {
            openapi = "3.0.1",
            info = new
            {
                title = $"{serviceName} Service (Unavailable)",
                version = "1.0.0",
                description = $"The {serviceName} service is currently unavailable. Please try again later."
            },
            paths = new { },
            components = new { }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 200; // Return 200 so Swagger UI doesn't break
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDoc, new JsonSerializerOptions { WriteIndented = true }));
    }

    private string GetServiceName(string path)
    {
        return path switch
        {
            "/api-docs/aiinsights.json" => "AI Insights",
            "/api-docs/auth.json" => "Authentication",
            "/api-docs/match.json" => "Match",
            "/api-docs/messaging.json" => "Messaging",
            "/api-docs/moderation.json" => "Moderation",
            "/api-docs/purchase.json" => "Purchase",
            "/api-docs/push.json" => "Push",
            "/api-docs/user.json" => "User",
            "/api-docs/notification.json" => "Notification",
            _ => "Unknown"
        };
    }
}