using Microsoft.AspNetCore.Mvc;
using MessagingService.API.Data;
using MessagingService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessagingDbContext _context;

        public MessagesController(MessagingDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message msg)
        {
            msg.CreatedAt = DateTime.UtcNow;
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // TODO: publish to RabbitMQ (analytics/AI)
            return Ok(msg);
        }

        [HttpGet("{matchId}")]
        public async Task<IActionResult> GetMessages(int matchId)
        {
            var messages = await _context.Messages
                .Where(m => m.MatchId == matchId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var msg = await _context.Messages.FindAsync(id);
            if (msg == null) return NotFound();

            msg.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(msg);
        }
    }
}
