using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PushService.API.Data;
using PushService.API.Models;
using PushService.API.Services;

namespace PushService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceTokensController : ControllerBase
    {
        private readonly PushDbContext _context;
        private readonly PushNotificationService _pushService;

        public DeviceTokensController(PushDbContext context, PushNotificationService pushService)
        {
            _context = context;
            _pushService = pushService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterToken([FromBody] DeviceToken token)
        {
            token.LastUsedAt = DateTime.UtcNow;
            _context.DeviceTokens.Add(token);
            await _context.SaveChangesAsync();
            return Ok(token);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromQuery] int userId, [FromQuery] string title, [FromQuery] string body)
        {
            var tokens = await _context.DeviceTokens
                .Where(t => t.UserId == userId)
                .Select(t => t.Token)
                .ToListAsync();

            foreach (var t in tokens)
            {
                await _pushService.SendNotificationAsync(t, title, body);
            }

            return Ok(new { Sent = tokens.Count });
        }
    }
}
