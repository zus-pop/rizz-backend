using Microsoft.AspNetCore.Mvc;

namespace PushService.API.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                service = "PushService.API",
                version = "1.0.0",
                status = "running",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "GET /api/devicetokens - Get all device tokens",
                    "GET /api/devicetokens/{id} - Get specific device token",
                    "POST /api/devicetokens - Register device token",
                    "PUT /api/devicetokens/{id} - Update device token",
                    "DELETE /api/devicetokens/{id} - Remove device token",
                    "POST /api/push/send - Send push notification",
                    "GET /health - Health check"
                }
            });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                service = "PushService.API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}