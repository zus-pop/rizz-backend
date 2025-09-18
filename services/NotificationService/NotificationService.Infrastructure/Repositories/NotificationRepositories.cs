using Microsoft.EntityFrameworkCore;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(x => x.UserId == userId && x.Status.ToString() != "Read")
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(x => x.Status.ToString() == "Pending")
            .Where(x => x.ScheduledAt == null || x.ScheduledAt <= DateTime.UtcNow)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Notifications.AddAsync(notification, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        notification.UpdateTimestamp();
        _context.Notifications.Update(notification);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await GetByIdAsync(id, cancellationToken);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<NotificationStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var total = await _context.Notifications.CountAsync(cancellationToken);
        
        var statusCounts = await _context.Notifications
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var typeCounts = await _context.Notifications
            .GroupBy(x => x.NotificationType.Value)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new NotificationStatsDto
        {
            TotalNotifications = total,
            PendingNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Pending")?.Count ?? 0,
            DeliveredNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Delivered")?.Count ?? 0,
            FailedNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Failed")?.Count ?? 0,
            ReadNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Read")?.Count ?? 0,
            NotificationsByType = typeCounts.ToDictionary(x => x.Type, x => x.Count),
            NotificationsByChannel = new Dictionary<string, int>() // This would need more complex query for channels
        };
    }

    public async Task<NotificationStatsDto> GetUserStatsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var total = await _context.Notifications
            .Where(x => x.UserId == userId)
            .CountAsync(cancellationToken);
        
        var statusCounts = await _context.Notifications
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var typeCounts = await _context.Notifications
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.NotificationType.Value)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new NotificationStatsDto
        {
            TotalNotifications = total,
            PendingNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Pending")?.Count ?? 0,
            DeliveredNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Delivered")?.Count ?? 0,
            FailedNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Failed")?.Count ?? 0,
            ReadNotifications = statusCounts.FirstOrDefault(x => x.Status.ToString() == "Read")?.Count ?? 0,
            NotificationsByType = typeCounts.ToDictionary(x => x.Type, x => x.Count),
            NotificationsByChannel = new Dictionary<string, int>()
        };
    }

    public async Task<IEnumerable<Notification>> GetPagedAsync(int page, int pageSize, string? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.Content.Title.Contains(filter) || x.Content.Body.Contains(filter));
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly NotificationDbContext _context;

    public NotificationTemplateRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .Where(x => x.NotificationType.Value == type)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<NotificationTemplate> AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        var entry = await _context.NotificationTemplates.AddAsync(template, cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        template.UpdateTimestamp();
        _context.NotificationTemplates.Update(template);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await GetByIdAsync(id, cancellationToken);
        if (template != null)
        {
            _context.NotificationTemplates.Remove(template);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetPagedAsync(int page, int pageSize, string? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.NotificationTemplates.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.Name.Contains(filter) || x.TitleTemplate.Contains(filter) || x.BodyTemplate.Contains(filter));
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}