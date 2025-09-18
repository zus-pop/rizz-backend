using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Queries;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationTemplatesController> _logger;

    public NotificationTemplatesController(IMediator mediator, ILogger<NotificationTemplatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetTemplates(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPagedNotificationTemplatesQuery(page, pageSize, filter);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetActiveTemplates(
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveNotificationTemplatesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NotificationTemplateDto>> GetTemplate(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationTemplateByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<NotificationTemplateDto>> GetTemplateByName(
        string name, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationTemplateByNameQuery(name);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet("by-type/{type}")]
    public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetTemplatesByType(
        string type, 
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationTemplatesByTypeQuery(type);
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationTemplateDto>> CreateTemplate(
        [FromBody] CreateNotificationTemplateDto createDto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateNotificationTemplateCommand(createDto);
        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetTemplate), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<NotificationTemplateDto>> UpdateTemplate(
        Guid id,
        [FromBody] UpdateNotificationTemplateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateNotificationTemplateCommand(id, updateDto);
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTemplate(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteNotificationTemplateCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult> ActivateTemplate(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new ActivateNotificationTemplateCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult> DeactivateTemplate(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeactivateNotificationTemplateCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}