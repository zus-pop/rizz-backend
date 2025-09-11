using Microsoft.AspNetCore.Mvc;
using Common.Application.Controllers;
using Common.Application.Models;

namespace AiInsightsService.API.Controllers;

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
            service = "AiInsightsService",
            timestamp = DateTime.UtcNow
        };

        return HandleResult(ApiResponse<object>.SuccessResult(healthData));
    }

    [HttpGet]
    public IActionResult Test()
    {
        var testData = new
        {
            message = "AiInsightsService is working!",
            timestamp = DateTime.UtcNow,
            service = "AiInsightsService"
        };

        return HandleResult(ApiResponse<object>.SuccessResult(testData));
    }
}
