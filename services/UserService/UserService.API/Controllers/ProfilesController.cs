using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {
                var query = new GetProfileByIdQuery { Id = id };
                var profile = await _mediator.Send(query);

                if (profile == null)
                    return NotFound(new { success = false, message = "Profile not found" });

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetProfileByUserId(int userId)
        {
            try
            {
                var query = new GetProfileByUserIdQuery { UserId = userId };
                var profile = await _mediator.Send(query);

                if (profile == null)
                    return NotFound(new { success = false, message = "Profile not found" });

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
        {
            try
            {
                var command = new CreateProfileCommand
                {
                    UserId = request.UserId,
                    Bio = request.Bio,
                    Voice = request.Voice,
                    University = request.University,
                    InterestedIn = request.InterestedIn,
                    LookingFor = request.LookingFor,
                    StudyStyle = request.StudyStyle,
                    WeekendHobby = request.WeekendHobby,
                    CampusLife = request.CampusLife,
                    FuturePlan = request.FuturePlan,
                    CommunicationPreference = request.CommunicationPreference,
                    DealBreakers = request.DealBreakers,
                    Zodiac = request.Zodiac,
                    LoveLanguage = request.LoveLanguage,
                    
                    // Legacy fields
                    Job = request.Job,
                    School = request.School,
                    InterestedInAgeMin = request.InterestedInAgeMin,
                    InterestedInAgeMax = request.InterestedInAgeMax,
                    InterestedInGender = request.InterestedInGender,
                    MaxDistanceKm = request.MaxDistanceKm,
                    ShowOnlyVerified = request.ShowOnlyVerified
                };

                var profile = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, new
                {
                    success = true,
                    data = profile,
                    message = "Profile created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateProfileDto dto)
        {
            try
            {
                var command = new UpdateProfileCommand
                {
                    UserId = userId,
                    Bio = dto.Bio,
                    Voice = dto.Voice,
                    University = dto.University,
                    InterestedIn = dto.InterestedIn,
                    LookingFor = dto.LookingFor,
                    StudyStyle = dto.StudyStyle,
                    WeekendHobby = dto.WeekendHobby,
                    CampusLife = dto.CampusLife,
                    FuturePlan = dto.FuturePlan,
                    CommunicationPreference = dto.CommunicationPreference,
                    DealBreakers = dto.DealBreakers,
                    Zodiac = dto.Zodiac,
                    LoveLanguage = dto.LoveLanguage,
                    
                    // Legacy fields
                    Job = dto.Job,
                    School = dto.School,
                    InterestedInAgeMin = dto.InterestedInAgeMin,
                    InterestedInAgeMax = dto.InterestedInAgeMax,
                    InterestedInGender = dto.InterestedInGender,
                    MaxDistanceKm = dto.MaxDistanceKm,
                    ShowOnlyVerified = dto.ShowOnlyVerified
                };

                var profile = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = profile,
                    message = "Profile updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            try
            {
                var command = new DeleteProfileCommand { UserId = userId };
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new { success = false, message = "Profile not found" });

                return Ok(new
                {
                    success = true,
                    message = "Profile deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("matches/{userId}")]
        public async Task<IActionResult> GetProfileMatches(int userId, 
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new GetProfilesByPreferencesQuery
                {
                    UserId = userId,
                    UserLatitude = latitude,
                    UserLongitude = longitude,
                    Page = page,
                    PageSize = pageSize
                };

                var profiles = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = profiles,
                    pagination = new
                    {
                        page = page,
                        pageSize = pageSize,
                        hasNext = profiles.Count() == pageSize
                    },
                    message = "Profile matches retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateProfileRequest
    {
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Voice { get; set; }
        public string? University { get; set; }
        public string? InterestedIn { get; set; }
        public string? LookingFor { get; set; }
        public string? StudyStyle { get; set; }
        public string? WeekendHobby { get; set; }
        public string? CampusLife { get; set; }
        public string? FuturePlan { get; set; }
        public string? CommunicationPreference { get; set; }
        public string? DealBreakers { get; set; }
        public string? Zodiac { get; set; }
        public string? LoveLanguage { get; set; }
        
        // Legacy fields for backward compatibility
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
    }
}