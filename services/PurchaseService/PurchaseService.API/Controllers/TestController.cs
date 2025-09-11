using Microsoft.AspNetCore.Mvc;
using Common.Application.Controllers;
using Common.Application.Models;

namespace PurchaseService.API.Controllers;

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
            service = "PurchaseService",
            timestamp = DateTime.UtcNow
        };

        return HandleResult(ApiResponse<object>.SuccessResult(healthData));
    }

    [HttpGet]
    public IActionResult Test()
    {
        var testData = new
        {
            message = "PurchaseService is working!",
            timestamp = DateTime.UtcNow,
            service = "PurchaseService"
        };

        return HandleResult(ApiResponse<object>.SuccessResult(testData));
    }
}
