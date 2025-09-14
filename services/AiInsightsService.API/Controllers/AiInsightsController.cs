using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiInsightsService.API.Data;
using AiInsightsService.API.Models;
using System.Text.Json;

namespace AiInsightsService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiInsightsController : ControllerBase
    {
        private readonly AiDbContext _context;
        public AiInsightsController(AiDbContext context) { _context = context; }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "AiInsightsService", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInsights()
        {
            var insights = await _context.AiInsights.ToListAsync();
            return Ok(insights);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetInsight(int userId)
        {
            var insight = await _context.AiInsights.FindAsync(userId);
            if (insight == null) return NotFound(new { message = $"No insights found for user {userId}" });
            return Ok(insight);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrUpdateInsight(int userId, [FromBody] CreateInsightRequest request)
        {
            try
            {
                var insight = await _context.AiInsights.FindAsync(userId);
                
                if (insight == null)
                {
                    insight = new AiInsight 
                    { 
                        UserId = userId,
                        SummaryText = request.SummaryText,
                        PersonalityTags = JsonSerializer.Serialize(request.PersonalityTags ?? new string[] {}),
                        CompatibilityScore = request.CompatibilityScore,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.AiInsights.Add(insight);
                }
                else
                {
                    insight.SummaryText = request.SummaryText;
                    insight.PersonalityTags = JsonSerializer.Serialize(request.PersonalityTags ?? new string[] {});
                    insight.CompatibilityScore = request.CompatibilityScore;
                    insight.UpdatedAt = DateTime.UtcNow;
                }
                
                await _context.SaveChangesAsync();
                return Ok(insight);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to create/update insight", details = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteInsight(int userId)
        {
            var insight = await _context.AiInsights.FindAsync(userId);
            if (insight == null) return NotFound();
            
            _context.AiInsights.Remove(insight);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Insight for user {userId} deleted successfully" });
        }
    }

    public record CreateInsightRequest(string SummaryText, float CompatibilityScore, string[]? PersonalityTags);
}
