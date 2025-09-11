using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.API.Data;
using UserService.API.Models;
using NetTopologySuite.Geometries;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool? verified = null, [FromQuery] string? gender = null)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (verified.HasValue)
                {
                    query = query.Where(u => u.IsVerified == verified.Value);
                }

                if (!string.IsNullOrEmpty(gender))
                {
                    query = query.Where(u => u.Gender.ToLower() == gender.ToLower());
                }

                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = users.Select(user => new {
                        user.Id, 
                        user.FirstName, 
                        user.LastName, 
                        user.Email, 
                        user.PhoneNumber, 
                        user.Gender, 
                        user.Birthday, 
                        user.Height, 
                        user.Personality, 
                        Location = user.Location != null ? new { 
                            Latitude = user.Location.Y, 
                            Longitude = user.Location.X 
                        } : null,
                        user.IsVerified, 
                        user.VerifiedAt,
                        user.CreatedAt 
                    }),
                    total = users.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                return Ok(new { 
                    success = true, 
                    data = new { 
                        user.Id, 
                        user.FirstName, 
                        user.LastName, 
                        user.Email, 
                        user.PhoneNumber, 
                        user.Gender, 
                        user.Birthday, 
                        user.Height, 
                        user.Personality, 
                        Location = user.Location != null ? new { 
                            Latitude = user.Location.Y, 
                            Longitude = user.Location.X 
                        } : null,
                        user.IsVerified, 
                        user.VerifiedAt,
                        user.CreatedAt 
                    } 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                // Create location point if coordinates provided
                Point? location = null;
                if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                    location = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
                }

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Gender = request.Gender,
                    Birthday = request.Birthday.HasValue ? DateTime.SpecifyKind(request.Birthday.Value, DateTimeKind.Utc) : null,
                    Height = request.Height,
                    Personality = request.Personality,
                    Location = location,
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
                    new { 
                        success = true, 
                        data = new { 
                            user.Id, 
                            user.FirstName, 
                            user.LastName, 
                            user.Email, 
                            user.PhoneNumber, 
                            user.Gender, 
                            user.Birthday, 
                            user.Height, 
                            user.Personality, 
                            Location = user.Location != null ? new { 
                                Latitude = user.Location.Y, 
                                Longitude = user.Location.X 
                            } : null,
                            user.IsVerified, 
                            user.VerifiedAt,
                            user.CreatedAt 
                        } 
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                if (!string.IsNullOrEmpty(request.Email))
                    user.Email = request.Email;
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    user.PhoneNumber = request.PhoneNumber;
                if (!string.IsNullOrEmpty(request.Gender))
                    user.Gender = request.Gender;
                if (request.Birthday.HasValue)
                    user.Birthday = DateTime.SpecifyKind(request.Birthday.Value, DateTimeKind.Utc);
                if (request.Height.HasValue)
                    user.Height = request.Height;
                if (!string.IsNullOrEmpty(request.Personality))
                    user.Personality = request.Personality;

                // Update location if coordinates provided
                if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                    user.Location = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = user, message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/verify")]
        public async Task<IActionResult> VerifyUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                user.IsVerified = true;
                user.VerifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = user, message = "User verified successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/unverify")]
        public async Task<IActionResult> UnverifyUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                user.IsVerified = false;
                user.VerifiedAt = null;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = user, message = "User verification removed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var verifiedUsers = await _context.Users.CountAsync(u => u.IsVerified);
                var unverifiedUsers = totalUsers - verifiedUsers;

                var genderStats = await _context.Users
                    .GroupBy(u => u.Gender)
                    .Select(g => new { Gender = g.Key, Count = g.Count() })
                    .ToListAsync();

                var recentUsers = await _context.Users
                    .Where(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                    .CountAsync();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        total = totalUsers,
                        verified = verifiedUsers,
                        unverified = unverifiedUsers,
                        recentlyJoined = recentUsers,
                        genderBreakdown = genderStats
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(u => u.FirstName.Contains(request.Name) || u.LastName.Contains(request.Name));
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    query = query.Where(u => u.Email.Contains(request.Email));
                }

                if (!string.IsNullOrEmpty(request.Gender))
                {
                    query = query.Where(u => u.Gender.ToLower() == request.Gender.ToLower());
                }

                if (request.MinAge.HasValue || request.MaxAge.HasValue)
                {
                    var now = DateTime.UtcNow;
                    if (request.MinAge.HasValue)
                    {
                        var maxBirthDate = now.AddYears(-request.MinAge.Value);
                        query = query.Where(u => u.Birthday.HasValue && u.Birthday <= maxBirthDate);
                    }
                    if (request.MaxAge.HasValue)
                    {
                        var minBirthDate = now.AddYears(-request.MaxAge.Value - 1);
                        query = query.Where(u => u.Birthday.HasValue && u.Birthday > minBirthDate);
                    }
                }

                if (request.IsVerified.HasValue)
                {
                    query = query.Where(u => u.IsVerified == request.IsVerified.Value);
                }

                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = users,
                    total = users.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public double? Height { get; set; }
        public string? Personality { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public double? Height { get; set; }
        public string? Personality { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class SearchUsersRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool? IsVerified { get; set; }
    }
}
