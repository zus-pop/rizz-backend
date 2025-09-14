using Microsoft.AspNetCore.Mvc;

namespace PurchaseService.API.Controllers
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
                service = "PurchaseService.API",
                version = "1.0.0",
                status = "running",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "GET /api/purchases - Get all purchases (with filters)",
                    "GET /api/purchases/{id} - Get specific purchase",
                    "POST /api/purchases - Create new purchase",
                    "PUT /api/purchases/{id}/complete - Complete purchase",
                    "PUT /api/purchases/{id}/cancel - Cancel purchase",
                    "GET /api/purchases/user/{userId} - Get user purchases",
                    "GET /api/purchases/user/{userId}/active - Get active user purchases",
                    "GET /api/purchases/user/{userId}/stats - Get user purchase statistics",
                    "DELETE /api/purchases/{id} - Delete purchase",
                    "GET /api/purchases/stats - Get overall purchase statistics",
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
                service = "PurchaseService.API",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
