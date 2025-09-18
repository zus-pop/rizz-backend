using MediatR;
using PushService.Application.DTOs;
using PushService.Domain.ValueObjects;

namespace PushService.Application.Commands
{
    public class RegisterDeviceTokenCommand : IRequest<DeviceTokenDto>
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DeviceType DeviceType { get; set; }
        public string? DeviceId { get; set; }
    }

    public class UpdateDeviceTokenCommand : IRequest<DeviceTokenDto>
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
    }

    public class DeactivateDeviceTokenCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteDeviceTokenCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class SendPushNotificationCommand : IRequest<bool>
    {
        public string? Token { get; set; }
        public int? UserId { get; set; }
        public IEnumerable<int>? UserIds { get; set; }
        public PushNotificationDto Notification { get; set; } = new();
        public bool SendToAll { get; set; } = false;
    }

    public class CleanupExpiredTokensCommand : IRequest<int>
    {
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromDays(30);
    }
}