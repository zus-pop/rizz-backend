using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Data;
using NotificationService.API.Models;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationDbContext _context;

        public NotificationController(NotificationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId, [FromQuery] bool? isRead = null)
        {
            try
            {
                var query = _context.Notifications.Where(n => n.UserId == userId);
                
                if (isRead.HasValue)
                {
                    query = query.Where(n => n.IsRead == isRead.Value);
                }

                var notifications = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = notifications,
                    total = notifications.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Notification not found" });
                }

                return Ok(new { success = true, data = notification });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                var notification = new Notification
                {
                    UserId = request.UserId,
                    Type = request.Type,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, 
                    new { success = true, data = notification });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Notification not found" });
                }

                notification.IsRead = true;
                notification.DeliveredAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = notification, message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("user/{userId}/mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            try
            {
                var unreadNotifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    notification.DeliveredAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = $"Marked {unreadNotifications.Count} notifications as read",
                    updatedCount = unreadNotifications.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Notification not found" });
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Notification deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteUserNotifications(int userId, [FromQuery] bool? onlyRead = null)
        {
            try
            {
                var query = _context.Notifications.Where(n => n.UserId == userId);
                
                if (onlyRead.HasValue && onlyRead.Value)
                {
                    query = query.Where(n => n.IsRead);
                }

                var notifications = await query.ToListAsync();
                _context.Notifications.RemoveRange(notifications);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = $"Deleted {notifications.Count} notifications",
                    deletedCount = notifications.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/stats")]
        public async Task<IActionResult> GetUserNotificationStats(int userId)
        {
            try
            {
                var totalCount = await _context.Notifications.CountAsync(n => n.UserId == userId);
                var unreadCount = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
                var readCount = totalCount - unreadCount;

                var typeStats = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .GroupBy(n => n.Type)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        total = totalCount,
                        unread = unreadCount,
                        read = readCount,
                        typeBreakdown = typeStats
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetNotificationTypes()
        {
            try
            {
                var types = await _context.Notifications
                    .Select(n => n.Type)
                    .Distinct()
                    .ToListAsync();

                return Ok(new { success = true, data = types });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
