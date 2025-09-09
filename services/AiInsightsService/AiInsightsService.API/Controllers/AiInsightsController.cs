using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiInsightsService.API.Data;

namespace AiInsightsService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiInsightsController : ControllerBase
    {
        private readonly AiDbContext _context;
        public AiInsightsController(AiDbContext context) { _context = context; }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetInsight(int userId)
        {
            var insight = await _context.AiInsights.FindAsync(userId);
            if (insight == null) return NotFound();
            return Ok(insight);
        }
    }
}
