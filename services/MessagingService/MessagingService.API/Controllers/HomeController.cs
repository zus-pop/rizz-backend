using Microsoft.AspNetCore.Mvc;

namespace MessagingService.API.Controllers
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
                service = "MessagingService API",
                version = "1.0.0",
                status = "running",
                endpoints = new[]
                {
                    "GET / - Service info",
                    "GET /api/messages/health - Health check",
                    "GET /api/messages - Get all messages",
                    "GET /api/messages/match/{matchId} - Get messages for a match",
                    "GET /api/messages/user/{userId} - Get messages sent by user",
                    "GET /api/messages/unread/match/{matchId} - Get unread messages for a match",
                    "POST /api/messages - Send a message",
                    "PUT /api/messages/{id}/read - Mark message as read",
                    "DELETE /api/messages/{id} - Delete a message"
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}
