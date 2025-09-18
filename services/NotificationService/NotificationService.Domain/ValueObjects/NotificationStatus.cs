namespace NotificationService.Domain.ValueObjects;

public enum NotificationStatus
{
    Pending,
    Scheduled,
    Sent,
    Delivered,
    Read,
    Failed,
    Expired,
    Cancelled,
    Retrying
}