using Common.Domain;

namespace NotificationService.Domain.Events;

public class NotificationCreatedEvent : IDomainEvent
{
    public Guid NotificationId { get; }
    public string UserId { get; }
    public string NotificationType { get; }
    public string Priority { get; }
    public DateTime CreatedAt { get; }
    public DateTime OccurredOn { get; }

    public NotificationCreatedEvent(Guid notificationId, string userId, string notificationType, string priority)
    {
        NotificationId = notificationId;
        UserId = userId;
        NotificationType = notificationType;
        Priority = priority;
        CreatedAt = DateTime.UtcNow;
        OccurredOn = DateTime.UtcNow;
    }
}

public class NotificationSentEvent : IDomainEvent
{
    public Guid NotificationId { get; }
    public string UserId { get; }
    public List<string> DeliveryChannels { get; }
    public DateTime SentAt { get; }
    public DateTime OccurredOn { get; }

    public NotificationSentEvent(Guid notificationId, string userId, List<string> deliveryChannels)
    {
        NotificationId = notificationId;
        UserId = userId;
        DeliveryChannels = deliveryChannels;
        SentAt = DateTime.UtcNow;
        OccurredOn = DateTime.UtcNow;
    }
}

public class NotificationReadEvent : IDomainEvent
{
    public Guid NotificationId { get; }
    public string UserId { get; }
    public DateTime ReadAt { get; }
    public DateTime OccurredOn { get; }

    public NotificationReadEvent(Guid notificationId, string userId)
    {
        NotificationId = notificationId;
        UserId = userId;
        ReadAt = DateTime.UtcNow;
        OccurredOn = DateTime.UtcNow;
    }
}

public class NotificationFailedEvent : IDomainEvent
{
    public Guid NotificationId { get; }
    public string UserId { get; }
    public string FailureReason { get; }
    public int RetryCount { get; }
    public DateTime FailedAt { get; }
    public DateTime OccurredOn { get; }

    public NotificationFailedEvent(Guid notificationId, string userId, string failureReason, int retryCount)
    {
        NotificationId = notificationId;
        UserId = userId;
        FailureReason = failureReason;
        RetryCount = retryCount;
        FailedAt = DateTime.UtcNow;
        OccurredOn = DateTime.UtcNow;
    }
}