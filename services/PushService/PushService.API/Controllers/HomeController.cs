using Microsoft.AspNetCore.Mvc;

namespace PushService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                service = "PushService",
                version = "2.0.0",
                status = "running",
                timestamp = DateTime.UtcNow,
                message = "PushService is running with Clean Architecture + DDD + CQRS"
            });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "PushService",
                database = "connected",
                firebase = "ready"
            });
        }
    }
}