using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Queries;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPagedNotificationsQuery(page, pageSize, filter);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NotificationDto>> GetNotification(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationsByUserIdQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("user/{userId}/unread")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotifications(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetUnreadNotificationsByUserIdQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetPendingNotifications(
        CancellationToken cancellationToken = default)
    {
        var query = new GetPendingNotificationsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> CreateNotification(
        [FromBody] CreateNotificationDto createDto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateNotificationCommand(createDto);
        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetNotification), new { id = result.Id }, result);
    }

    [HttpPost("from-template")]
    public async Task<ActionResult<NotificationDto>> CreateFromTemplate(
        [FromBody] CreateFromTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateNotificationFromTemplateCommand(
            request.TemplateName, 
            request.UserId, 
            request.Variables);
        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetNotification), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<NotificationDto>> UpdateNotification(
        Guid id,
        [FromBody] UpdateNotificationDto updateDto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateNotificationCommand(id, updateDto);
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteNotification(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteNotificationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    [HttpPost("{id:guid}/mark-read")]
    public async Task<ActionResult> MarkAsRead(
        Guid id,
        [FromBody] MarkAsReadRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new MarkNotificationAsReadCommand(id, request.UserId);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    [HttpPost("user/{userId}/mark-all-read")]
    public async Task<ActionResult<int>> MarkAllAsRead(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var command = new MarkAllNotificationsAsReadCommand(userId);
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(new { MarkedCount = result });
    }

    [HttpPost("{id:guid}/send")]
    public async Task<ActionResult> SendNotification(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new SendNotificationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return BadRequest("Failed to send notification");
            
        return NoContent();
    }

    [HttpPost("send-pending")]
    public async Task<ActionResult<int>> SendPendingNotifications(CancellationToken cancellationToken = default)
    {
        var command = new SendPendingNotificationsCommand();
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(new { SentCount = result });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<NotificationStatsDto>> GetStats(CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationStatsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("user/{userId}/stats")]
    public async Task<ActionResult<NotificationStatsDto>> GetUserStats(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserNotificationStatsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }
}

// Request DTOs
public record CreateFromTemplateRequest(
    string TemplateName,
    string UserId,
    Dictionary<string, string> Variables);

public record MarkAsReadRequest(string UserId);