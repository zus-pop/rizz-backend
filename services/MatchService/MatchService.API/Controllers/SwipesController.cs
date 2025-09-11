using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchService.API.Data;
using MatchService.API.Models;
using MatchService.API.Services;

namespace MatchService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwipesController : ControllerBase
    {
        private readonly MatchDbContext _context;
        // private readonly RabbitMqService _rabbit; // Disabled for now

        public SwipesController(MatchDbContext context)
        {
            _context = context;
            // _rabbit = rabbit; // Disabled for now
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "MatchService", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSwipes()
        {
            var swipes = await _context.Swipes.ToListAsync();
            return Ok(swipes);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserSwipes(int userId)
        {
            var swipes = await _context.Swipes
                .Where(s => s.SwiperId == userId)
                .ToListAsync();
            return Ok(swipes);
        }

        [HttpGet("matches")]
        public async Task<IActionResult> GetAllMatches()
        {
            var matches = await _context.Matches.ToListAsync();
            return Ok(matches);
        }

        [HttpGet("matches/user/{userId}")]
        public async Task<IActionResult> GetUserMatches(int userId)
        {
            var matches = await _context.Matches
                .Where(m => m.User1Id == userId || m.User2Id == userId)
                .ToListAsync();
            return Ok(matches);
        }

        [HttpPost]
        public IActionResult Swipe([FromBody] Swipe swipe)
        {
            try
            {
                if (swipe == null || swipe.SwiperId <= 0 || swipe.SwipeeId <= 0)
                    return BadRequest("Invalid swipe data");

                _context.Swipes.Add(swipe);
                _context.SaveChanges();

            if (swipe.Direction == "right")
            {
                // Check if the other user also swiped right
                var backSwipe = _context.Swipes.FirstOrDefault(s =>
                    s.SwiperId == swipe.SwipeeId &&
                    s.SwipeeId == swipe.SwiperId &&
                    s.Direction == "right");

                if (backSwipe != null)
                {
                    var match = new Match
                    {
                        User1Id = swipe.SwiperId,
                        User2Id = swipe.SwipeeId
                    };
                    _context.Matches.Add(match);
                    _context.SaveChanges();

                    // TODO: Publish match event to RabbitMQ when available
                    // var message = $"{{\"user1\":{match.User1Id},\"user2\":{match.User2Id},\"matchedAt\":\"{match.MatchedAt:o}\"}}";
                    // _rabbit.Publish("matches_queue", message);

                    return Ok(new { matched = true, match });
                }
            }

                return Ok(new { matched = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Swipe operation failed", details = ex.Message });
            }
        }
    }
}
