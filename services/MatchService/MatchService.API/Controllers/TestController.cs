using Microsoft.AspNetCore.Mvc;
using Common.Application.Controllers;
using Common.Application.Models;

namespace MatchService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : BaseController
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            var response = ApiResponse<object>.SuccessResult(new { 
                status = "healthy", 
                service = "MatchService", 
                timestamp = DateTime.UtcNow 
            });
            return HandleResult(response);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet]
        public IActionResult TestGet()
        {
            var response = ApiResponse<object>.SuccessResult(new { 
                message = "MatchService GET endpoint working", 
                timestamp = DateTime.UtcNow 
            });
            return HandleResult(response);
        }

        [HttpPost]
        public IActionResult TestPost([FromBody] object data)
        {
            var response = ApiResponse<object>.SuccessResult(new { 
                message = "MatchService POST endpoint working", 
                receivedData = data, 
                timestamp = DateTime.UtcNow 
            });
            return HandleResult(response);
        }
    }
}
