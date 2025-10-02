using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Data;
using UserService.Domain.Enums;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VietnameseTestController : ControllerBase
    {
        private readonly UserDbContext _context;

        public VietnameseTestController(UserDbContext context)
        {
            _context = context;
        }

        [HttpGet("seed-sample-data")]
        public async Task<IActionResult> SeedSampleData()
        {
            try
            {
                await VietnameseSampleDataSeeder.SeedAsync(_context);
                return Ok(new { Message = "Vietnamese sample data seeded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("profiles")]
        public async Task<IActionResult> GetVietnameseProfiles()
        {
            try
            {
                var users = await _context.Users
                    .Where(u => EF.Property<string>(u, "Email").Contains("vietnamese.test"))
                    .ToListAsync();

                var profiles = await _context.Profiles
                    .Where(p => users.Select(u => u.Id).Contains(p.UserId))
                    .ToListAsync();

                var result = users.Select(u => new
                {
                    UserId = u.Id,
                    UserName = $"{u.FirstName} {u.LastName}",
                    Gender = u.Gender,
                    Email = u.Email.Value,
                    PhoneNumber = u.PhoneNumber.Value,
                    Personality = u.Personality,
                    Profile = profiles.Where(p => p.UserId == u.Id).Select(p => new
                    {
                        Bio = p.Bio,
                        // Vietnamese characteristics
                        Emotion = p.Emotion?.ToString(),
                        VoiceQuality = p.VoiceQuality?.ToString(),
                        Accent = p.Accent?.ToString()
                    }).FirstOrDefault()
                }).ToList();

                return Ok(new
                {
                    Message = "Vietnamese profiles retrieved successfully",
                    Count = result.Count,
                    Profiles = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("enums")]
        public IActionResult GetVietnameseEnums()
        {
            try
            {
                var enums = new
                {
                    EmotionTypes = Enum.GetNames(typeof(EmotionType)).ToList(),
                    VoiceQualityTypes = Enum.GetNames(typeof(VoiceQualityType)).ToList(),
                    AccentTypes = Enum.GetNames(typeof(AccentType)).ToList(),
                    GenderTypes = Enum.GetNames(typeof(GenderType)).ToList()
                };

                return Ok(new
                {
                    Message = "Vietnamese enum values retrieved successfully",
                    EnumValues = enums
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}