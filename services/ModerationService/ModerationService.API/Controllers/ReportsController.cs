using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ModerationService.Application.Commands;
using ModerationService.Application.Queries;
using ModerationService.Application.DTOs;

namespace ModerationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "ModerationService - Reports", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Get report by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReportDto>> GetReport(Guid id)
    {
        var report = await _mediator.Send(new GetReportByIdQuery(id));
        
        if (report == null)
            return NotFound();
            
        return Ok(report);
    }

    /// <summary>
    /// Get reports for a user
    /// </summary>
    [HttpGet("for-user/{reportedUserId}")]
    public async Task<ActionResult<List<ReportDto>>> GetReportsForUser(int reportedUserId)
    {
        var reports = await _mediator.Send(new GetReportsForUserQuery(reportedUserId));
        return Ok(reports);
    }

    /// <summary>
    /// Get reports by status
    /// </summary>
    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<List<ReportDto>>> GetReportsByStatus(string status)
    {
        var reports = await _mediator.Send(new GetReportsByStatusQuery(status));
        return Ok(reports);
    }

    /// <summary>
    /// Get pending reports
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<List<ReportDto>>> GetPendingReports()
    {
        var reports = await _mediator.Send(new GetPendingReportsQuery());
        return Ok(reports);
    }

    /// <summary>
    /// Create a new report
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReportDto>> CreateReport([FromBody] CreateReportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var command = new CreateReportCommand(request.ReporterId, request.ReportedUserId, request.Reason, request.Description);
            var report = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Review a report (dismiss, resolve, escalate)
    /// </summary>
    [HttpPut("{id}/review")]
    public async Task<IActionResult> ReviewReport(Guid id, [FromBody] ReviewReportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ReviewReportCommand(id, request.ReviewedByUserId, request.Action, request.ReviewNotes);
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    /// <summary>
    /// Escalate a report
    /// </summary>
    [HttpPut("{id}/escalate")]
    public async Task<IActionResult> EscalateReport(Guid id, [FromBody] EscalateReportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new EscalateReportCommand(id, request.EscalatedByUserId);
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    /// <summary>
    /// Get moderation case by ID
    /// </summary>
    [HttpGet("cases/{id}")]
    public async Task<ActionResult<ModerationCaseDto>> GetModerationCase(Guid id)
    {
        var moderationCase = await _mediator.Send(new GetModerationCaseByIdQuery(id));
        
        if (moderationCase == null)
            return NotFound();
            
        return Ok(moderationCase);
    }

    /// <summary>
    /// Get moderation cases for a user
    /// </summary>
    [HttpGet("cases/for-user/{targetUserId}")]
    public async Task<ActionResult<List<ModerationCaseDto>>> GetCasesForUser(int targetUserId)
    {
        var cases = await _mediator.Send(new GetCasesForUserQuery(targetUserId));
        return Ok(cases);
    }

    /// <summary>
    /// Get assigned moderation cases
    /// </summary>
    [HttpGet("cases/assigned/{assignedToUserId}")]
    public async Task<ActionResult<List<ModerationCaseDto>>> GetAssignedCases(int assignedToUserId)
    {
        var cases = await _mediator.Send(new GetAssignedCasesQuery(assignedToUserId));
        return Ok(cases);
    }
}

// Request DTOs
public class CreateReportRequest
{
    public int ReporterId { get; set; }
    public int ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ReviewReportRequest
{
    public int ReviewedByUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
}

public class EscalateReportRequest
{
    public int EscalatedByUserId { get; set; }
}