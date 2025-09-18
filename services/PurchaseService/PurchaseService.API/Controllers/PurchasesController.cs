using MediatR;
using Microsoft.AspNetCore.Mvc;
using PurchaseService.Application.Commands;
using PurchaseService.Application.DTOs;
using PurchaseService.Application.Queries;

namespace PurchaseService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(IMediator mediator, ILogger<PurchasesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new purchase
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> CreatePurchase([FromBody] CreatePurchaseCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPurchase), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase for user {UserId}", command.UserId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get purchase by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PurchaseDto>> GetPurchase(Guid id)
    {
        try
        {
            var query = new GetPurchaseByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase {PurchaseId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get purchases for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<PurchaseDto>>> GetUserPurchases(string userId)
    {
        try
        {
            var query = new GetUserPurchasesQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchases for user {UserId}", userId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get active subscriptions for a user
    /// </summary>
    [HttpGet("user/{userId}/subscriptions")]
    public async Task<ActionResult<List<PurchaseDto>>> GetActiveSubscriptions(string userId)
    {
        try
        {
            var query = new GetActiveSubscriptionsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active subscriptions for user {UserId}", userId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get purchases by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<PurchaseDto>>> GetPurchasesByStatus(string status)
    {
        try
        {
            var query = new GetPurchasesByStatusQuery { Status = status };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchases by status {Status}", status);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get purchase history with pagination
    /// </summary>
    [HttpGet("user/{userId}/history")]
    public async Task<ActionResult<PurchaseHistoryDto>> GetPurchaseHistory(
        string userId, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetPurchaseHistoryQuery 
            { 
                UserId = userId, 
                PageNumber = pageNumber, 
                PageSize = pageSize 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase history for user {UserId}", userId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get user purchase statistics
    /// </summary>
    [HttpGet("user/{userId}/stats")]
    public async Task<ActionResult<UserPurchaseStatsDto>> GetUserStats(string userId)
    {
        try
        {
            var query = new GetUserPurchaseStatsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase stats for user {UserId}", userId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Process payment for a purchase
    /// </summary>
    [HttpPost("{id:guid}/process-payment")]
    public async Task<ActionResult<PurchaseDto>> ProcessPayment(Guid id)
    {
        try
        {
            var command = new ProcessPaymentCommand { PurchaseId = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for purchase {PurchaseId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a purchase
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<PurchaseDto>> CancelPurchase(Guid id, [FromBody] CancelPurchaseRequest request)
    {
        try
        {
            var command = new CancelPurchaseCommand 
            { 
                PurchaseId = id, 
                Reason = request.Reason 
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling purchase {PurchaseId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Refund a purchase
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    public async Task<ActionResult<PurchaseDto>> RefundPurchase(Guid id, [FromBody] RefundPurchaseRequest request)
    {
        try
        {
            var command = new RefundPurchaseCommand 
            { 
                PurchaseId = id, 
                RefundAmount = request.RefundAmount,
                Reason = request.Reason 
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding purchase {PurchaseId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update payment method for a purchase
    /// </summary>
    [HttpPut("{id:guid}/payment-method")]
    public async Task<ActionResult<PurchaseDto>> UpdatePaymentMethod(Guid id, [FromBody] UpdatePaymentMethodCommand command)
    {
        try
        {
            command.PurchaseId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment method for purchase {PurchaseId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class CancelPurchaseRequest
{
    public string? Reason { get; set; }
}

public class RefundPurchaseRequest
{
    public decimal? RefundAmount { get; set; }
    public string? Reason { get; set; }
}