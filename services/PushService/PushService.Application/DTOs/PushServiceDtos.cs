using PushService.Domain.ValueObjects;

namespace PushService.Application.DTOs
{
    public class DeviceTokenDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DeviceType DeviceType { get; set; }
        public string? DeviceId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RegisterDeviceTokenDto
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DeviceType DeviceType { get; set; }
        public string? DeviceId { get; set; }
    }

    public class UpdateDeviceTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
    }

    public class PushNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public Dictionary<string, string>? Data { get; set; }
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        public string? ImageUrl { get; set; }
        public string? Sound { get; set; }
    }

    public class SendNotificationDto
    {
        public string? Token { get; set; }
        public int? UserId { get; set; }
        public IEnumerable<int>? UserIds { get; set; }
        public PushNotificationDto Notification { get; set; } = new();
        public bool SendToAll { get; set; } = false;
    }
}