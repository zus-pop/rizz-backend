using AutoMapper;
using MediatR;
using PushService.Application.Commands;
using PushService.Application.DTOs;
using PushService.Application.Queries;
using PushService.Domain.Entities;
using PushService.Domain.Repositories;
using PushService.Domain.ValueObjects;

namespace PushService.Application.Handlers
{
    public class DeviceTokenCommandHandlers : 
        IRequestHandler<RegisterDeviceTokenCommand, DeviceTokenDto>,
        IRequestHandler<UpdateDeviceTokenCommand, DeviceTokenDto>,
        IRequestHandler<DeactivateDeviceTokenCommand, bool>,
        IRequestHandler<DeleteDeviceTokenCommand, bool>,
        IRequestHandler<CleanupExpiredTokensCommand, int>
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IMapper _mapper;

        public DeviceTokenCommandHandlers(IDeviceTokenRepository deviceTokenRepository, IMapper mapper)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _mapper = mapper;
        }

        public async Task<DeviceTokenDto> Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            // Check if token already exists for this user and device type
            var existingTokens = await _deviceTokenRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var existingToken = existingTokens.FirstOrDefault(t => 
                t.DeviceType == request.DeviceType && 
                t.DeviceId == request.DeviceId);

            if (existingToken != null)
            {
                // Update existing token
                existingToken.UpdateToken(request.Token);
                existingToken.Activate();
                var updatedToken = await _deviceTokenRepository.UpdateAsync(existingToken, cancellationToken);
                return _mapper.Map<DeviceTokenDto>(updatedToken);
            }

            // Create new token
            var deviceToken = new DeviceToken(request.UserId, request.Token, request.DeviceType, request.DeviceId);
            var createdToken = await _deviceTokenRepository.AddAsync(deviceToken, cancellationToken);
            return _mapper.Map<DeviceTokenDto>(createdToken);
        }

        public async Task<DeviceTokenDto> Handle(UpdateDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            var deviceToken = await _deviceTokenRepository.GetByIdAsync(request.Id, cancellationToken);
            if (deviceToken == null)
                throw new ArgumentException("Device token not found");

            deviceToken.UpdateToken(request.Token);
            if (!string.IsNullOrWhiteSpace(request.DeviceId))
            {
                // Would need to add SetDeviceId method to entity
            }

            var updatedToken = await _deviceTokenRepository.UpdateAsync(deviceToken, cancellationToken);
            return _mapper.Map<DeviceTokenDto>(updatedToken);
        }

        public async Task<bool> Handle(DeactivateDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            var deviceToken = await _deviceTokenRepository.GetByIdAsync(request.Id, cancellationToken);
            if (deviceToken == null)
                return false;

            deviceToken.Deactivate();
            await _deviceTokenRepository.UpdateAsync(deviceToken, cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            await _deviceTokenRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }

        public async Task<int> Handle(CleanupExpiredTokensCommand request, CancellationToken cancellationToken)
        {
            var expiredTokens = await _deviceTokenRepository.GetExpiredTokensAsync(request.ExpirationTime, cancellationToken);
            int count = expiredTokens.Count();
            
            await _deviceTokenRepository.DeactivateExpiredTokensAsync(request.ExpirationTime, cancellationToken);
            
            return count;
        }
    }
}