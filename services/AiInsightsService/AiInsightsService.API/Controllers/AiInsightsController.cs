using Microsoft.AspNetCore.Mvc;
using MediatR;
using AiInsightsService.Application.Commands;
using AiInsightsService.Application.Queries;
using AiInsightsService.API.Models;

namespace AiInsightsService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiInsightsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AiInsightsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "AiInsightsService", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInsights()
        {
            try
            {
                var query = new GetAllAiInsightsQuery();
                var insights = await _mediator.Send(query);
                
                var response = insights.Select(dto => new AiInsightResponse(
                    dto.UserId,
                    dto.SummaryText,
                    dto.CompatibilityScore,
                    dto.PersonalityTags,
                    dto.UpdatedAt,
                    dto.CompatibilityLevel
                ));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve insights", error = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetInsight(int userId)
        {
            try
            {
                var query = new GetAiInsightByUserIdQuery(userId);
                var insight = await _mediator.Send(query);
                
                if (insight == null)
                    return NotFound(new { message = $"No insights found for user {userId}" });

                var response = new AiInsightResponse(
                    insight.UserId,
                    insight.SummaryText,
                    insight.CompatibilityScore,
                    insight.PersonalityTags,
                    insight.UpdatedAt,
                    insight.CompatibilityLevel
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve insight", error = ex.Message });
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateOrUpdateInsight(int userId, [FromBody] CreateOrUpdateAiInsightRequest request)
        {
            try
            {
                var command = new CreateOrUpdateAiInsightCommand(
                    userId,
                    request.SummaryText,
                    request.CompatibilityScore,
                    request.PersonalityTags
                );

                var resultUserId = await _mediator.Send(command);
                
                return Ok(new AiInsightCreatedResponse(
                    resultUserId,
                    "AI insight created/updated successfully"
                ));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create/update insight", error = ex.Message });
            }
        }

        [HttpPost("{userId}/generate")]
        public async Task<IActionResult> GenerateInsight(int userId, [FromBody] GenerateAiInsightRequest request)
        {
            try
            {
                var command = new GenerateAiInsightCommand(
                    userId,
                    request.UserProfile,
                    request.BehaviorData
                );

                var resultUserId = await _mediator.Send(command);
                
                return Ok(new AiInsightCreatedResponse(
                    resultUserId,
                    "AI insight generated successfully"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to generate insight", error = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteInsight(int userId)
        {
            try
            {
                var command = new DeleteAiInsightCommand(userId);
                var success = await _mediator.Send(command);
                
                if (!success)
                    return NotFound(new { message = $"No insight found for user {userId}" });

                return Ok(new { message = $"Insight for user {userId} deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete insight", error = ex.Message });
            }
        }

        [HttpGet("compatibility/{minScore}/{maxScore}")]
        public async Task<IActionResult> GetInsightsByCompatibilityRange(float minScore, float maxScore)
        {
            try
            {
                var query = new GetAiInsightsByCompatibilityRangeQuery(minScore, maxScore);
                var insights = await _mediator.Send(query);
                
                var response = insights.Select(dto => new AiInsightResponse(
                    dto.UserId,
                    dto.SummaryText,
                    dto.CompatibilityScore,
                    dto.PersonalityTags,
                    dto.UpdatedAt,
                    dto.CompatibilityLevel
                ));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve insights", error = ex.Message });
            }
        }

        [HttpGet("search/personality/{tag}")]
        public async Task<IActionResult> SearchByPersonalityTag(string tag)
        {
            try
            {
                var query = new SearchAiInsightsByPersonalityTagQuery(tag);
                var insights = await _mediator.Send(query);
                
                var response = insights.Select(dto => new AiInsightResponse(
                    dto.UserId,
                    dto.SummaryText,
                    dto.CompatibilityScore,
                    dto.PersonalityTags,
                    dto.UpdatedAt,
                    dto.CompatibilityLevel
                ));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to search insights", error = ex.Message });
            }
        }
    }
}