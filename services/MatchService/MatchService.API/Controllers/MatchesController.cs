using Microsoft.AspNetCore.Mvc;
using MatchService.Infrastructure.Data;
using MatchService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly MatchDbContext _context;

        public MatchesController(MatchDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all matches for a user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetUserMatches(Guid userId)
        {
            var matches = await _context.Matches
                .Where(m => (m.User1Id == userId || m.User2Id == userId) && m.IsActive)
                .ToListAsync();
            
            return Ok(matches);
        }

        /// <summary>
        /// Get potential matches for a user
        /// </summary>
        [HttpGet("user/{userId}/potential")]
        public async Task<ActionResult<object>> GetPotentialMatches(Guid userId)
        {
            // Get users this user has already swiped on
            var swipedUserIds = await _context.Swipes
                .Where(s => s.SwiperId == userId)
                .Select(s => s.TargetUserId)
                .ToListAsync();

            // For demo purposes, return some mock data
            // In a real implementation, this would query the UserService
            return Ok(new { 
                message = "Potential matches API - would integrate with UserService",
                userId = userId,
                excludedUserIds = swipedUserIds.Take(5)
            });
        }

        /// <summary>
        /// Create a swipe action
        /// </summary>
        [HttpPost("swipe")]
        public async Task<ActionResult> CreateSwipe([FromBody] CreateSwipeRequest request)
        {
            var swipe = new Swipe
            {
                SwiperId = request.SwiperId,
                TargetUserId = request.TargetUserId,
                Direction = request.Direction
            };

            _context.Swipes.Add(swipe);

            // Check if this creates a match (both users liked each other)
            if (swipe.IsLike())
            {
                var reciprocalSwipe = await _context.Swipes
                    .FirstOrDefaultAsync(s => s.SwiperId == request.TargetUserId 
                                           && s.TargetUserId == request.SwiperId 
                                           && s.IsLike());

                if (reciprocalSwipe != null)
                {
                    // Create a match
                    var match = new Match
                    {
                        User1Id = request.SwiperId,
                        User2Id = request.TargetUserId
                    };
                    _context.Matches.Add(match);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Swipe recorded successfully", swipeId = swipe.Id });
        }

        /// <summary>
        /// Get match by ID
        /// </summary>
        [HttpGet("{matchId}")]
        public async Task<ActionResult<Match>> GetMatch(Guid matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        /// <summary>
        /// Unmatch users
        /// </summary>
        [HttpDelete("{matchId}/unmatch")]
        public async Task<ActionResult> UnmatchUsers(Guid matchId, [FromBody] UnmatchRequest request)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
            {
                return NotFound();
            }

            match.Unmatch(request.UserId);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Users unmatched successfully" });
        }

        /// <summary>
        /// Get swipe history for a user
        /// </summary>
        [HttpGet("user/{userId}/swipes")]
        public async Task<ActionResult<IEnumerable<Swipe>>> GetUserSwipes(Guid userId)
        {
            var swipes = await _context.Swipes
                .Where(s => s.SwiperId == userId)
                .OrderByDescending(s => s.SwipedAt)
                .Take(100)
                .ToListAsync();

            return Ok(swipes);
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "healthy", 
                service = "MatchService",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "GET /api/matches/user/{userId}",
                    "GET /api/matches/user/{userId}/potential", 
                    "POST /api/matches/swipe",
                    "GET /api/matches/{matchId}",
                    "DELETE /api/matches/{matchId}/unmatch",
                    "GET /api/matches/user/{userId}/swipes"
                }
            });
        }
    }

    public class CreateSwipeRequest
    {
        public Guid SwiperId { get; set; }
        public Guid TargetUserId { get; set; }
        public MatchService.Domain.Entities.SwipeDirection Direction { get; set; }
    }

    public class UnmatchRequest
    {
        public Guid UserId { get; set; }
    }
}