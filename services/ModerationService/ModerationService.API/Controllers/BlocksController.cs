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

        [HttpPost]
        public async Task<IActionResult> BlockUser([FromBody] Block block)
        {
            block.CreatedAt = DateTime.UtcNow;
            _context.Blocks.Add(block);
            await _context.SaveChangesAsync();
            return Ok(block);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBlockedUsers(int userId)
        {
            var blocks = await _context.Blocks
                .Where(b => b.BlockerId == userId)
                .ToListAsync();
            return Ok(blocks);
        }
    }
}
