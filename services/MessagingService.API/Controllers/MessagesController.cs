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

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "MessagingService", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _context.Messages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message msg)
        {
            try
            {
                if (msg == null || string.IsNullOrWhiteSpace(msg.Content))
                    return BadRequest("Message content is required");

                if (msg.MatchId <= 0 || msg.SenderId <= 0)
                    return BadRequest("Valid MatchId and SenderId are required");

                msg.CreatedAt = DateTime.UtcNow;
                _context.Messages.Add(msg);
                await _context.SaveChangesAsync();

                // TODO: publish to RabbitMQ (analytics/AI)
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to send message", details = ex.Message });
            }
        }

        [HttpGet("match/{matchId}")]
        public async Task<IActionResult> GetMessages(int matchId)
        {
            try
            {
                if (matchId <= 0)
                    return BadRequest("Valid MatchId is required");

                var messages = await _context.Messages
                    .Where(m => m.MatchId == matchId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get messages", details = ex.Message });
            }
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Valid message ID is required");

                var msg = await _context.Messages.FindAsync(id);
                if (msg == null) return NotFound(new { message = $"Message with id {id} not found" });

                if (msg.ReadAt.HasValue)
                    return Ok(new { message = "Message already marked as read", data = msg });

                msg.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to mark message as read", details = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserMessages(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("Valid UserId is required");

                var messages = await _context.Messages
                    .Where(m => m.SenderId == userId)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get user messages", details = ex.Message });
            }
        }

        [HttpGet("unread/match/{matchId}")]
        public async Task<IActionResult> GetUnreadMessages(int matchId)
        {
            try
            {
                if (matchId <= 0)
                    return BadRequest("Valid MatchId is required");

                var unreadMessages = await _context.Messages
                    .Where(m => m.MatchId == matchId && m.ReadAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(new { count = unreadMessages.Count, messages = unreadMessages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get unread messages", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Valid message ID is required");

                var message = await _context.Messages.FindAsync(id);
                if (message == null) return NotFound(new { message = $"Message with id {id} not found" });

                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Message {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to delete message", details = ex.Message });
            }
        }
    }
}
