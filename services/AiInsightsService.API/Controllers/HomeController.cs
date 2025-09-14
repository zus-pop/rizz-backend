using Microsoft.AspNetCore.Mvc;

namespace AiInsightsService.API.Controllers
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
                service = "AiInsightsService API",
                version = "1.0.0",
                status = "running",
                endpoints = new[]
                {
                    "GET / - Service info",
                    "GET /api/aiinsights/health - Health check",
                    "GET /api/aiinsights - Get all insights",
                    "GET /api/aiinsights/{userId} - Get insight for user",
                    "POST /api/aiinsights/{userId} - Create/update insight for user",
                    "DELETE /api/aiinsights/{userId} - Delete insight for user"
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}
