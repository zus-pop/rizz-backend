using Microsoft.AspNetCore.Mvc;

namespace NotificationService.API.Controllers
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
                service = "NotificationService.API",
                version = "1.0.0",
                status = "running",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "GET /api/notification/user/{userId} - Get user notifications",
                    "GET /api/notification/{id} - Get specific notification",
                    "POST /api/notification - Create notification",
                    "PUT /api/notification/{id}/mark-read - Mark notification as read",
                    "PUT /api/notification/user/{userId}/mark-all-read - Mark all user notifications as read",
                    "DELETE /api/notification/{id} - Delete notification",
                    "DELETE /api/notification/user/{userId} - Delete user notifications",
                    "GET /api/notification/user/{userId}/stats - Get notification statistics",
                    "GET /api/notification/types - Get notification types",
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
                service = "NotificationService.API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
