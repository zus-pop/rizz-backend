using Microsoft.AspNetCore.Mvc;

namespace MatchService.API.Controllers
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
                service = "MatchService API",
                version = "1.0.0",
                status = "running",
                endpoints = new[]
                {
                    "GET / - Service info",
                    "GET /api/swipes/health - Health check",
                    "GET /api/swipes - Get all swipes",
                    "GET /api/swipes/user/{userId} - Get swipes for user",
                    "GET /api/swipes/matches - Get all matches",
                    "GET /api/swipes/matches/user/{userId} - Get matches for user",
                    "POST /api/swipes - Create a swipe (and potentially match)"
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}
