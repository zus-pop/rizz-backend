using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Interfaces;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IEventPublisher _eventPublisher;

    public TestController(ILogger<TestController> logger, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        _logger.LogInformation("Health check endpoint called");
        return Ok(new
        {
            Status = "Healthy",
            Service = "NotificationService",
            Timestamp = DateTime.UtcNow,
            EventPublisher = _eventPublisher.GetType().Name,
            Message = "Service is running without RabbitMQ connection issues!"
        });
    }

    [HttpPost("test-post")]
    public async Task<IActionResult> TestPost([FromBody] TestRequest request)
    {
        _logger.LogInformation("Test POST endpoint called with message: {Message}", request.Message);
        
        try
        {
            // Test that we can handle POST operations without RabbitMQ issues
            await Task.Delay(100); // Simulate some processing
            
            // Test our NoOp event publisher (this should not cause RabbitMQ connection errors)
            await _eventPublisher.PublishAsync(new TestEvent { Message = request.Message });
            
            return Ok(new
            {
                Success = true,
                Message = $"POST operation successful! Received: {request.Message}",
                Timestamp = DateTime.UtcNow,
                EventPublisherType = _eventPublisher.GetType().Name,
                Note = "Event publishing completed without RabbitMQ connection issues"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test POST operation");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}

public class TestRequest
{
    public string Message { get; set; } = string.Empty;
}

public class TestEvent
{
    public string Message { get; set; } = string.Empty;
}