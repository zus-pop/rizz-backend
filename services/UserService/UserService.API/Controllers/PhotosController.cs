using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.API.Data;
using UserService.API.Models;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhotosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPhotos([FromQuery] int? userId = null)
        {
            try
            {
                var query = _context.Photos.AsQueryable();

                if (userId.HasValue)
                {
                    query = query.Where(p => p.UserId == userId.Value);
                }

                var photos = await query
                    .OrderBy(p => p.UserId)
                    .ThenBy(p => p.SortOrder)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = photos,
                    total = photos.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            try
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound(new { success = false, message = "Photo not found" });
                }

                return Ok(new { success = true, data = photo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPhotos(int userId)
        {
            try
            {
                var photos = await _context.Photos
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.SortOrder)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = photos,
                    total = photos.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/profile-picture")]
        public async Task<IActionResult> GetUserProfilePicture(int userId)
        {
            try
            {
                var profilePicture = await _context.Photos
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.IsProfilePicture);

                if (profilePicture == null)
                {
                    return NotFound(new { success = false, message = "Profile picture not found" });
                }

                return Ok(new { success = true, data = profilePicture });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhoto([FromBody] CreatePhotoRequest request)
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

                // If this is set as profile picture, remove profile picture flag from other photos
                if (request.IsProfilePicture)
                {
                    var existingProfilePictures = await _context.Photos
                        .Where(p => p.UserId == request.UserId && p.IsProfilePicture)
                        .ToListAsync();

                    foreach (var pic in existingProfilePictures)
                    {
                        pic.IsProfilePicture = false;
                    }
                }

                // Set sort order if not provided
                var sortOrder = request.SortOrder;
                if (sortOrder == 0)
                {
                    var maxSortOrder = await _context.Photos
                        .Where(p => p.UserId == request.UserId)
                        .MaxAsync(p => (int?)p.SortOrder) ?? 0;
                    sortOrder = maxSortOrder + 1;
                }

                var photo = new Photo
                {
                    UserId = request.UserId,
                    Url = request.Url,
                    SortOrder = sortOrder,
                    IsProfilePicture = request.IsProfilePicture,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPhoto), new { id = photo.Id }, 
                    new { success = true, data = photo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhoto(int id, [FromBody] UpdatePhotoRequest request)
        {
            try
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound(new { success = false, message = "Photo not found" });
                }

                // If this is being set as profile picture, remove profile picture flag from other photos
                if (request.IsProfilePicture == true)
                {
                    var existingProfilePictures = await _context.Photos
                        .Where(p => p.UserId == photo.UserId && p.IsProfilePicture && p.Id != id)
                        .ToListAsync();

                    foreach (var pic in existingProfilePictures)
                    {
                        pic.IsProfilePicture = false;
                    }
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.Url))
                    photo.Url = request.Url;
                if (request.SortOrder.HasValue)
                    photo.SortOrder = request.SortOrder.Value;
                if (request.IsProfilePicture.HasValue)
                    photo.IsProfilePicture = request.IsProfilePicture.Value;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = photo, message = "Photo updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/set-profile-picture")]
        public async Task<IActionResult> SetAsProfilePicture(int id)
        {
            try
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound(new { success = false, message = "Photo not found" });
                }

                // Remove profile picture flag from other photos of the same user
                var existingProfilePictures = await _context.Photos
                    .Where(p => p.UserId == photo.UserId && p.IsProfilePicture && p.Id != id)
                    .ToListAsync();

                foreach (var pic in existingProfilePictures)
                {
                    pic.IsProfilePicture = false;
                }

                photo.IsProfilePicture = true;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = photo, message = "Photo set as profile picture" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("user/{userId}/reorder")]
        public async Task<IActionResult> ReorderPhotos(int userId, [FromBody] ReorderPhotosRequest request)
        {
            try
            {
                var photos = await _context.Photos
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                if (request.PhotoIds.Count != photos.Count)
                {
                    return BadRequest(new { success = false, message = "Photo ID count mismatch" });
                }

                foreach (var photo in photos)
                {
                    var newOrder = request.PhotoIds.IndexOf(photo.Id) + 1;
                    if (newOrder == 0)
                    {
                        return BadRequest(new { success = false, message = $"Photo ID {photo.Id} not found in request" });
                    }
                    photo.SortOrder = newOrder;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Photos reordered successfully", data = photos.OrderBy(p => p.SortOrder) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            try
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound(new { success = false, message = "Photo not found" });
                }

                _context.Photos.Remove(photo);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Photo deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteAllUserPhotos(int userId)
        {
            try
            {
                var photos = await _context.Photos
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                if (photos.Count == 0)
                {
                    return NotFound(new { success = false, message = "No photos found for this user" });
                }

                _context.Photos.RemoveRange(photos);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = $"Deleted {photos.Count} photos", deletedCount = photos.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CreatePhotoRequest
    {
        public int UserId { get; set; }
        public string Url { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public bool IsProfilePicture { get; set; } = false;
    }

    public class UpdatePhotoRequest
    {
        public string? Url { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsProfilePicture { get; set; }
    }

    public class ReorderPhotosRequest
    {
        public List<int> PhotoIds { get; set; } = new List<int>();
    }
}
