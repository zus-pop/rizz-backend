using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
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
                service = "AuthService API",
                version = "1.0.0",
                status = "running",
                endpoints = new[]
                {
                    "POST /api/auth/register",
                    "POST /api/auth/login",
                    "POST /api/auth/send-otp",
                    "POST /api/auth/verify-otp",
                    "GET /api/auth/health"
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}
