using MediatR;
using PushService.Application.DTOs;
using PushService.Domain.ValueObjects;

namespace PushService.Application.Queries
{
    public class GetDeviceTokenByIdQuery : IRequest<DeviceTokenDto?>
    {
        public int Id { get; set; }
    }

    public class GetDeviceTokensByUserIdQuery : IRequest<IEnumerable<DeviceTokenDto>>
    {
        public int UserId { get; set; }
        public bool ActiveOnly { get; set; } = true;
    }

    public class GetDeviceTokensByDeviceTypeQuery : IRequest<IEnumerable<DeviceTokenDto>>
    {
        public DeviceType DeviceType { get; set; }
        public bool ActiveOnly { get; set; } = true;
    }

    public class GetExpiredDeviceTokensQuery : IRequest<IEnumerable<DeviceTokenDto>>
    {
        public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromDays(30);
    }

    public class GetActiveTokenCountByUserQuery : IRequest<int>
    {
        public int UserId { get; set; }
    }
}