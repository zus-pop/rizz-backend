using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
    Task<NotificationStatsDto> GetUserStatsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetPagedAsync(int page, int pageSize, string? filter = null, CancellationToken cancellationToken = default);
}

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);
    Task<NotificationTemplate> AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetPagedAsync(int page, int pageSize, string? filter = null, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    INotificationRepository Notifications { get; }
    INotificationTemplateRepository NotificationTemplates { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public interface INotificationDeliveryInfrastructure
{
    Task<bool> DeliverAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<bool> DeliverToChannelAsync(Notification notification, string channel, CancellationToken cancellationToken = default);
    Task<bool> IsChannelAvailableAsync(string channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAvailableChannelsAsync(CancellationToken cancellationToken = default);
}

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class;
}

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
    DateTimeOffset UtcOffsetNow { get; }
}