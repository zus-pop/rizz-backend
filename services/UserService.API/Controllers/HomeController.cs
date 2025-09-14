using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers
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
                service = "UserService.API",
                version = "1.0.0",
                status = "running",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "GET /api/users - Get all users (with filters)",
                    "GET /api/users/{id} - Get specific user",
                    "POST /api/users - Create new user",
                    "PUT /api/users/{id} - Update user",
                    "PUT /api/users/{id}/verify - Verify user",
                    "DELETE /api/users/{id} - Delete user",
                    "GET /api/users/stats - Get user statistics",
                    "GET /api/users/search - Search users",
                    "GET /api/profiles - Get all profiles",
                    "GET /api/profiles/{id} - Get specific profile",
                    "GET /api/profiles/user/{userId} - Get user profile",
                    "POST /api/profiles - Create profile",
                    "PUT /api/profiles/{id} - Update profile",
                    "DELETE /api/profiles/{id} - Delete profile",
                    "GET /api/profiles/search - Search profiles",
                    "GET /api/photos - Get all photos",
                    "GET /api/photos/{id} - Get specific photo",
                    "GET /api/photos/user/{userId} - Get user photos",
                    "POST /api/photos - Create photo",
                    "PUT /api/photos/{id} - Update photo",
                    "PUT /api/photos/{id}/set-profile-picture - Set as profile picture",
                    "DELETE /api/photos/{id} - Delete photo",
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
                service = "UserService.API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
