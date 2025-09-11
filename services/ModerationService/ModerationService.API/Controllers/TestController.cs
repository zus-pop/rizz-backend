using Microsoft.AspNetCore.Mvc;
using Common.Application.Controllers;
using Common.Application.Models;

namespace ModerationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : BaseController
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        var healthData = new
        {
            status = "healthy",
            service = "ModerationService",
            timestamp = DateTime.UtcNow
        };

        return HandleResult(ApiResponse<object>.SuccessResult(healthData));
    }

    [HttpGet]
    public IActionResult Test()
    {
        var testData = new
        {
            message = "ModerationService is working!",
            timestamp = DateTime.UtcNow,
            service = "ModerationService"
        };

        return HandleResult(ApiResponse<object>.SuccessResult(testData));
    }
}
