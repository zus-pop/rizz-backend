using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public NotificationContent Content { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public List<DeliveryChannel> Channels { get; private set; } = new();
    public NotificationStatus Status { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; } = 0;
    public Dictionary<string, string> Metadata { get; private set; } = new();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // For EF Core
    private Notification() 
    {
        UserId = string.Empty;
        NotificationType = NotificationType.System;
        Content = NotificationContent.Create("", "");
        Priority = NotificationPriority.Normal;
        Status = NotificationStatus.Pending;
    }

    public Notification(
        string userId,
        NotificationType type,
        NotificationContent content,
        NotificationPriority priority = null!,
        DateTime? scheduledAt = null,
        DateTime? expiredAt = null,
        Dictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        UserId = userId;
        NotificationType = type ?? throw new ArgumentNullException(nameof(type));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Priority = priority ?? NotificationPriority.Normal;
        Status = NotificationStatus.Pending;
        ScheduledAt = scheduledAt;
        ExpiredAt = expiredAt;
        Metadata = metadata ?? new Dictionary<string, string>();

        ValidateNotification();
    }

    private void ValidateNotification()
    {
        if (ScheduledAt.HasValue && ScheduledAt.Value <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled time must be in the future");

        if (ExpiredAt.HasValue && ExpiredAt.Value <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future");

        if (ScheduledAt.HasValue && ExpiredAt.HasValue && ScheduledAt.Value >= ExpiredAt.Value)
            throw new ArgumentException("Scheduled time must be before expiration time");
    }

    public void AddDeliveryChannel(DeliveryChannel channel)
    {
        if (channel == null)
            throw new ArgumentNullException(nameof(channel));

        if (!Channels.Contains(channel))
        {
            Channels.Add(channel);
        }
    }

    public void RemoveDeliveryChannel(DeliveryChannelType channelType)
    {
        Channels.RemoveAll(c => c.Type == channelType);
    }

    public void MarkAsSent()
    {
        if (Status != NotificationStatus.Pending && Status != NotificationStatus.Retrying)
            throw new InvalidOperationException($"Cannot mark notification as sent when status is {Status}");

        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        FailureReason = null;
    }

    public void MarkAsDelivered()
    {
        if (Status != NotificationStatus.Sent)
            throw new InvalidOperationException($"Cannot mark notification as delivered when status is {Status}");

        Status = NotificationStatus.Delivered;
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Failed || Status == NotificationStatus.Expired)
            throw new InvalidOperationException($"Cannot mark notification as read when status is {Status}");

        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason is required", nameof(reason));

        Status = NotificationStatus.Failed;
        FailureReason = reason;
    }

    public void MarkAsExpired()
    {
        Status = NotificationStatus.Expired;
    }

    public void Retry()
    {
        if (Status != NotificationStatus.Failed)
            throw new InvalidOperationException($"Cannot retry notification when status is {Status}");

        if (RetryCount >= 3)
            throw new InvalidOperationException("Maximum retry attempts exceeded");

        Status = NotificationStatus.Retrying;
        RetryCount++;
        FailureReason = null;
    }

    public void Schedule(DateTime scheduledAt)
    {
        if (scheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled time must be in the future");

        if (Status != NotificationStatus.Pending)
            throw new InvalidOperationException($"Cannot schedule notification when status is {Status}");

        ScheduledAt = scheduledAt;
        Status = NotificationStatus.Scheduled;
    }

    public void UpdateContent(NotificationContent newContent)
    {
        if (Status == NotificationStatus.Sent || Status == NotificationStatus.Delivered || Status == NotificationStatus.Read)
            throw new InvalidOperationException("Cannot update content of sent notification");

        Content = newContent ?? throw new ArgumentNullException(nameof(newContent));
    }

    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));

        Metadata[key] = value ?? string.Empty;
    }

    // Business logic properties
    public bool IsRead => Status == NotificationStatus.Read;
    public bool IsSent => Status == NotificationStatus.Sent || Status == NotificationStatus.Delivered || Status == NotificationStatus.Read;
    public bool IsPending => Status == NotificationStatus.Pending;
    public bool IsScheduled => Status == NotificationStatus.Scheduled;
    public bool HasFailed => Status == NotificationStatus.Failed;
    public bool IsExpired => Status == NotificationStatus.Expired || (ExpiredAt.HasValue && ExpiredAt.Value <= DateTime.UtcNow);
    public bool CanRetry => Status == NotificationStatus.Failed && RetryCount < 3;
    public bool RequiresImmediateDelivery => Priority.RequiresImmediateDelivery && !IsScheduled;
    public bool IsReadyToSend => (Status == NotificationStatus.Pending || Status == NotificationStatus.Retrying) && 
                                (!ScheduledAt.HasValue || ScheduledAt.Value <= DateTime.UtcNow) && !IsExpired;
    
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum NotificationStatus
{
    Pending,
    Scheduled,
    Sent,
    Delivered,
    Read,
    Failed,
    Retrying,
    Expired
}