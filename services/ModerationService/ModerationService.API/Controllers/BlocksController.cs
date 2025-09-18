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
public class BlocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlocksController(IMediator mediator)
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
        return Ok(new { status = "healthy", service = "ModerationService - Blocks", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Get block by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BlockDto>> GetBlock(Guid id)
    {
        var block = await _mediator.Send(new GetBlockByIdQuery(id));
        
        if (block == null)
            return NotFound();
            
        return Ok(block);
    }

    /// <summary>
    /// Get blocks created by a user
    /// </summary>
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<List<BlockDto>>> GetBlocksByUser(int userId)
    {
        var blocks = await _mediator.Send(new GetUserBlocksQuery(userId));
        return Ok(blocks);
    }

    /// <summary>
    /// Get blocks for a user (users who blocked this user)
    /// </summary>
    [HttpGet("for-user/{blockedUserId}")]
    public async Task<ActionResult<List<BlockDto>>> GetBlocksForUser(int blockedUserId)
    {
        var blocks = await _mediator.Send(new GetBlocksForUserQuery(blockedUserId));
        return Ok(blocks);
    }

    /// <summary>
    /// Check if an active block exists between two users
    /// </summary>
    [HttpGet("check/{blockerId}/{blockedUserId}")]
    public async Task<ActionResult<bool>> CheckActiveBlock(int blockerId, int blockedUserId)
    {
        var exists = await _mediator.Send(new CheckActiveBlockQuery(blockerId, blockedUserId));
        return Ok(new { exists });
    }

    /// <summary>
    /// Create a new block
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BlockDto>> CreateBlock([FromBody] CreateBlockRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var command = new CreateBlockCommand(request.BlockerId, request.BlockedUserId, request.Reason);
            var block = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetBlock), new { id = block.Id }, block);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Revoke an existing block
    /// </summary>
    [HttpPut("{id}/revoke")]
    public async Task<IActionResult> RevokeBlock(Guid id, [FromBody] RevokeBlockRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new RevokeBlockCommand(id, request.RevokedByUserId);
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    /// <summary>
    /// Restore a revoked block
    /// </summary>
    [HttpPut("{id}/restore")]
    public async Task<IActionResult> RestoreBlock(Guid id, [FromBody] RestoreBlockRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new RestoreBlockCommand(id, request.RestoredByUserId);
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}

// Request DTOs
public class CreateBlockRequest
{
    public int BlockerId { get; set; }
    public int BlockedUserId { get; set; }
    public string? Reason { get; set; }
}

public class RevokeBlockRequest
{
    public int RevokedByUserId { get; set; }
}

public class RestoreBlockRequest
{
    public int RestoredByUserId { get; set; }
}