using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers
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
                service = "UserService",
                version = "2.0.0",
                status = "running",
                timestamp = DateTime.UtcNow,
                message = "UserService is running with Clean Architecture + DDD + CQRS"
            });
        }
    }
}