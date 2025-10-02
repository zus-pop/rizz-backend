using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Domain.Enums;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PreferencesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PreferencesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var query = new GetPreferenceByUserIdQuery { UserId = userId };
                var pref = await _mediator.Send(query);
                if (pref == null) return NotFound(new { success = false, message = "Preference not found" });
                return Ok(new { success = true, data = pref });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePreferenceRequest req)
        {
            try
            {
                if (!req.LookingForGender.HasValue)
                    return BadRequest(new { success = false, message = "LookingForGender is required" });
                var command = new CreatePreferenceCommand
                {
                    UserId = req.UserId,
                    LookingForGender = req.LookingForGender.Value,
                    AgeMin = req.AgeMin,
                    AgeMax = req.AgeMax,
                    MaxDistanceKm = req.MaxDistanceKm,
                    InterestsFilter = req.InterestsFilter,
                    Emotion = req.Emotion,
                    VoiceQuality = req.VoiceQuality,
                    Accent = req.Accent
                };
                var created = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetByUserId), new { userId = created.UserId }, new { success = true, data = created, message = "Preference created" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("user/{userId}")]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdatePreferenceRequest req)
        {
            try
            {
                var command = new UpdatePreferenceCommand
                {
                    UserId = userId,
                    LookingForGender = req.LookingForGender,
                    AgeMin = req.AgeMin,
                    AgeMax = req.AgeMax,
                    MaxDistanceKm = req.MaxDistanceKm,
                    InterestsFilter = req.InterestsFilter,
                    Emotion = req.Emotion,
                    VoiceQuality = req.VoiceQuality,
                    Accent = req.Accent
                };
                var updated = await _mediator.Send(command);
                return Ok(new { success = true, data = updated, message = "Preference updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            try
            {
                var command = new DeletePreferenceCommand { UserId = userId };
                var result = await _mediator.Send(command);
                if (!result) return NotFound(new { success = false, message = "Preference not found" });
                return Ok(new { success = true, message = "Preference deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreatePreferenceRequest
    {
        public int UserId { get; set; }
        public GenderType? LookingForGender { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public int? MaxDistanceKm { get; set; }
        public string? InterestsFilter { get; set; }
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
    }

    public class UpdatePreferenceRequest
    {
        public GenderType? LookingForGender { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public int? MaxDistanceKm { get; set; }
        public string? InterestsFilter { get; set; }
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
    }
}
