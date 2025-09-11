using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModerationService.API.Data;
using ModerationService.API.Models;

namespace ModerationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlocksController : ControllerBase
    {
        private readonly ModerationDbContext _context;

        public BlocksController(ModerationDbContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "ModerationService - Blocks", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlocks()
        {
            var blocks = await _context.Blocks
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return Ok(blocks);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser([FromBody] Block block)
        {
            try
            {
                if (block == null || block.BlockerId <= 0 || block.BlockedId <= 0)
                    return BadRequest("Valid BlockerId and BlockedId are required");

                if (block.BlockerId == block.BlockedId)
                    return BadRequest("User cannot block themselves");

                // Check if block already exists
                var existingBlock = await _context.Blocks
                    .FirstOrDefaultAsync(b => b.BlockerId == block.BlockerId && b.BlockedId == block.BlockedId);

                if (existingBlock != null)
                    return Conflict("User is already blocked");

                block.CreatedAt = DateTime.UtcNow;
                _context.Blocks.Add(block);
                await _context.SaveChangesAsync();
                return Ok(block);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to block user", details = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBlockedUsers(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Valid UserId is required");

                var blocks = await _context.Blocks
                    .Where(b => b.BlockerId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
                return Ok(blocks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get blocked users", details = ex.Message });
            }
        }

        [HttpGet("blocked-by/{userId}")]
        public async Task<IActionResult> GetUsersWhoBlockedUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Valid UserId is required");

                var blocks = await _context.Blocks
                    .Where(b => b.BlockedId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
                return Ok(blocks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get users who blocked this user", details = ex.Message });
            }
        }

        [HttpGet("check/{blockerId}/{blockedId}")]
        public async Task<IActionResult> CheckIfBlocked(int blockerId, int blockedId)
        {
            try
            {
                if (blockerId <= 0 || blockedId <= 0)
                    return BadRequest("Valid BlockerId and BlockedId are required");

                var isBlocked = await _context.Blocks
                    .AnyAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

                return Ok(new { isBlocked, blockerId, blockedId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to check block status", details = ex.Message });
            }
        }

        [HttpDelete("{blockerId}/{blockedId}")]
        public async Task<IActionResult> UnblockUser(int blockerId, int blockedId)
        {
            try
            {
                if (blockerId <= 0 || blockedId <= 0)
                    return BadRequest("Valid BlockerId and BlockedId are required");

                var block = await _context.Blocks
                    .FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

                if (block == null)
                    return NotFound("Block relationship not found");

                _context.Blocks.Remove(block);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"User {blockedId} has been unblocked by user {blockerId}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to unblock user", details = ex.Message });
            }
        }
    }
}
