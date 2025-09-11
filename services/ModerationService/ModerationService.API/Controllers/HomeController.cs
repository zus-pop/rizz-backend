using Microsoft.AspNetCore.Mvc;

namespace ModerationService.API.Controllers
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
                service = "ModerationService API",
                version = "1.0.0",
                status = "running",
                endpoints = new[]
                {
                    "GET / - Service info",
                    "GET /api/blocks/health - Health check for blocks",
                    "GET /api/reports/health - Health check for reports",
                    "GET /api/blocks - Get all blocks",
                    "GET /api/blocks/user/{userId} - Get users blocked by user",
                    "GET /api/blocks/blocked-by/{userId} - Get users who blocked this user",
                    "GET /api/blocks/check/{blockerId}/{blockedId} - Check if user is blocked",
                    "POST /api/blocks - Block a user",
                    "DELETE /api/blocks/{blockerId}/{blockedId} - Unblock a user",
                    "GET /api/reports - Get all reports",
                    "GET /api/reports/reported/{userId} - Get reports against user",
                    "GET /api/reports/reporter/{userId} - Get reports made by user",
                    "GET /api/reports/stats - Get report statistics",
                    "POST /api/reports - Report a user",
                    "DELETE /api/reports/{id} - Delete a report"
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}
