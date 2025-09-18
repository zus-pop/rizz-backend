using AutoMapper;
using MediatR;
using PushService.Application.Commands;
using PushService.Domain.Repositories;
using PushService.Domain.ValueObjects;

namespace PushService.Application.Handlers
{
    public class PushNotificationCommandHandlers : 
        IRequestHandler<SendPushNotificationCommand, bool>
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IMapper _mapper;

        public PushNotificationCommandHandlers(
            IDeviceTokenRepository deviceTokenRepository,
            IPushNotificationService pushNotificationService,
            IMapper mapper)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _pushNotificationService = pushNotificationService;
            _mapper = mapper;
        }

        public async Task<bool> Handle(SendPushNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new PushNotification(
                request.Notification.Title,
                request.Notification.Body,
                request.Notification.Priority)
                .WithData(request.Notification.Data ?? new Dictionary<string, string>());

            if (!string.IsNullOrWhiteSpace(request.Notification.ImageUrl))
                notification = notification.WithImage(request.Notification.ImageUrl);

            if (!string.IsNullOrWhiteSpace(request.Notification.Sound))
                notification = notification.WithSound(request.Notification.Sound);

            // Send to specific token
            if (!string.IsNullOrWhiteSpace(request.Token))
            {
                return await _pushNotificationService.SendNotificationAsync(request.Token, notification, cancellationToken);
            }

            // Send to specific user
            if (request.UserId.HasValue)
            {
                return await _pushNotificationService.SendNotificationToUserAsync(request.UserId.Value, notification, cancellationToken);
            }

            // Send to multiple users
            if (request.UserIds != null && request.UserIds.Any())
            {
                return await _pushNotificationService.SendNotificationToMultipleUsersAsync(request.UserIds, notification, cancellationToken);
            }

            // Send to all users
            if (request.SendToAll)
            {
                return await _pushNotificationService.SendNotificationToAllAsync(notification, cancellationToken);
            }

            throw new ArgumentException("Invalid notification request - no target specified");
        }
    }
}