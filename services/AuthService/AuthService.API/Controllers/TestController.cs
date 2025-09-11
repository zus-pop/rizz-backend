using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "AuthService", timestamp = DateTime.UtcNow });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet("simple")]
        public IActionResult Simple()
        {
            return Ok(new { message = "AuthService is running", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public IActionResult TestGet()
        {
            return Ok(new { message = "GET endpoint is working", timestamp = DateTime.UtcNow });
        }

        [HttpPost]
        public IActionResult Test([FromBody] object data)
        {
            return Ok(new { message = "POST endpoint is working", receivedData = data, timestamp = DateTime.UtcNow });
        }
    }
}
