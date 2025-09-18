using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Services;

public interface INotificationDeliveryService
{
    Task<bool> CanDeliverAsync(Notification notification, DeliveryChannel channel, CancellationToken cancellationToken = default);
    Task<DeliveryResult> DeliverAsync(Notification notification, DeliveryChannel channel, CancellationToken cancellationToken = default);
    Task<List<DeliveryResult>> DeliverToAllChannelsAsync(Notification notification, CancellationToken cancellationToken = default);
}

public class DeliveryResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public DeliveryChannelType Channel { get; init; }
    public DateTime DeliveredAt { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();

    public static DeliveryResult Success(DeliveryChannelType channel, Dictionary<string, string>? metadata = null)
    {
        return new DeliveryResult
        {
            IsSuccess = true,
            Channel = channel,
            DeliveredAt = DateTime.UtcNow,
            Metadata = metadata ?? new Dictionary<string, string>()
        };
    }

    public static DeliveryResult Failure(DeliveryChannelType channel, string errorMessage)
    {
        return new DeliveryResult
        {
            IsSuccess = false,
            Channel = channel,
            ErrorMessage = errorMessage,
            DeliveredAt = DateTime.UtcNow
        };
    }
}