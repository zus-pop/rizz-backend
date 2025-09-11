using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.API.Data;
using UserService.API.Models;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfiles()
        {
            try
            {
                var profiles = await _context.Profiles
                    .OrderBy(p => p.ProfileId)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = profiles,
                    total = profiles.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetProfile(int profileId)
        {
            try
            {
                var profile = await _context.Profiles.FindAsync(profileId);
                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                var profile = await _context.Profiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found for this user" });
                }

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                // Check if user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                {
                    return BadRequest(new { success = false, message = "User does not exist" });
                }

                // Check if profile already exists for this user
                var existingProfile = await _context.Profiles
                    .FirstOrDefaultAsync(p => p.UserId == request.UserId);
                if (existingProfile != null)
                {
                    return BadRequest(new { success = false, message = "Profile already exists for this user" });
                }

                var profile = new Profile
                {
                    UserId = request.UserId,
                    Bio = request.Bio ?? string.Empty,
                    Voice = request.Voice ?? string.Empty,
                    University = request.University ?? string.Empty,
                    InterestedIn = request.InterestedIn ?? string.Empty,
                    LookingFor = request.LookingFor ?? string.Empty,
                    StudyStyle = request.StudyStyle ?? string.Empty,
                    WeekendHobby = request.WeekendHobby ?? string.Empty,
                    CampusLife = request.CampusLife ?? string.Empty,
                    FuturePlan = request.FuturePlan ?? string.Empty,
                    CommunicationPreference = request.CommunicationPreference ?? string.Empty,
                    DealBreakers = request.DealBreakers ?? string.Empty,
                    Zodiac = request.Zodiac ?? string.Empty,
                    LoveLanguage = request.LoveLanguage ?? string.Empty
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProfile), new { profileId = profile.ProfileId }, 
                    new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile(int profileId, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var profile = await _context.Profiles.FindAsync(profileId);
                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                // Update fields if provided
                if (request.Bio != null)
                    profile.Bio = request.Bio;
                if (request.Voice != null)
                    profile.Voice = request.Voice;
                if (request.University != null)
                    profile.University = request.University;
                if (request.InterestedIn != null)
                    profile.InterestedIn = request.InterestedIn;
                if (request.LookingFor != null)
                    profile.LookingFor = request.LookingFor;
                if (request.StudyStyle != null)
                    profile.StudyStyle = request.StudyStyle;
                if (request.WeekendHobby != null)
                    profile.WeekendHobby = request.WeekendHobby;
                if (request.CampusLife != null)
                    profile.CampusLife = request.CampusLife;
                if (request.FuturePlan != null)
                    profile.FuturePlan = request.FuturePlan;
                if (request.CommunicationPreference != null)
                    profile.CommunicationPreference = request.CommunicationPreference;
                if (request.DealBreakers != null)
                    profile.DealBreakers = request.DealBreakers;
                if (request.Zodiac != null)
                    profile.Zodiac = request.Zodiac;
                if (request.LoveLanguage != null)
                    profile.LoveLanguage = request.LoveLanguage;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = profile, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{profileId}")]
        public async Task<IActionResult> DeleteProfile(int profileId)
        {
            try
            {
                var profile = await _context.Profiles.FindAsync(profileId);
                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Profile deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProfiles([FromQuery] SearchProfilesRequest request)
        {
            try
            {
                var query = _context.Profiles.AsQueryable();

                if (!string.IsNullOrEmpty(request.University))
                {
                    query = query.Where(p => p.University.Contains(request.University));
                }

                if (!string.IsNullOrEmpty(request.InterestedIn))
                {
                    query = query.Where(p => p.InterestedIn.Contains(request.InterestedIn));
                }

                if (!string.IsNullOrEmpty(request.LookingFor))
                {
                    query = query.Where(p => p.LookingFor.Contains(request.LookingFor));
                }

                if (!string.IsNullOrEmpty(request.Zodiac))
                {
                    query = query.Where(p => p.Zodiac.ToLower() == request.Zodiac.ToLower());
                }

                if (!string.IsNullOrEmpty(request.LoveLanguage))
                {
                    query = query.Where(p => p.LoveLanguage.Contains(request.LoveLanguage));
                }

                var profiles = await query.ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = profiles,
                    total = profiles.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
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
    }

    public class UpdateProfileRequest
    {
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
    }

    public class SearchProfilesRequest
    {
        public string? University { get; set; }
        public string? InterestedIn { get; set; }
        public string? LookingFor { get; set; }
        public string? Zodiac { get; set; }
        public string? LoveLanguage { get; set; }
    }
}
