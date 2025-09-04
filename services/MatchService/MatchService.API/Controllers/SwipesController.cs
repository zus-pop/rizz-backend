using Microsoft.AspNetCore.Mvc;
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
        private readonly RabbitMqService _rabbit;

        public SwipesController(MatchDbContext context, RabbitMqService rabbit)
        {
            _context = context;
            _rabbit = rabbit;
        }

        [HttpPost]
        public IActionResult Swipe([FromBody] Swipe swipe)
        {
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

                    // Publish match event to RabbitMQ
                    var message = $"{{\"user1\":{match.User1Id},\"user2\":{match.User2Id},\"matchedAt\":\"{match.MatchedAt:o}\"}}";
                    _rabbit.Publish("matches_queue", message);

                    return Ok(new { matched = true, match });
                }
            }

            return Ok(new { matched = false });
        }
    }
}
