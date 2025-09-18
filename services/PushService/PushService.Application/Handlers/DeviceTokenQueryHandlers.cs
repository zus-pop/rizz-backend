using AutoMapper;
using MediatR;
using PushService.Application.DTOs;
using PushService.Application.Queries;
using PushService.Domain.Repositories;

namespace PushService.Application.Handlers
{
    public class DeviceTokenQueryHandlers : 
        IRequestHandler<GetDeviceTokenByIdQuery, DeviceTokenDto?>,
        IRequestHandler<GetDeviceTokensByUserIdQuery, IEnumerable<DeviceTokenDto>>,
        IRequestHandler<GetDeviceTokensByDeviceTypeQuery, IEnumerable<DeviceTokenDto>>,
        IRequestHandler<GetExpiredDeviceTokensQuery, IEnumerable<DeviceTokenDto>>,
        IRequestHandler<GetActiveTokenCountByUserQuery, int>
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IMapper _mapper;

        public DeviceTokenQueryHandlers(IDeviceTokenRepository deviceTokenRepository, IMapper mapper)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _mapper = mapper;
        }

        public async Task<DeviceTokenDto?> Handle(GetDeviceTokenByIdQuery request, CancellationToken cancellationToken)
        {
            var deviceToken = await _deviceTokenRepository.GetByIdAsync(request.Id, cancellationToken);
            return deviceToken != null ? _mapper.Map<DeviceTokenDto>(deviceToken) : null;
        }

        public async Task<IEnumerable<DeviceTokenDto>> Handle(GetDeviceTokensByUserIdQuery request, CancellationToken cancellationToken)
        {
            var deviceTokens = request.ActiveOnly 
                ? await _deviceTokenRepository.GetActiveTokensByUserIdAsync(request.UserId, cancellationToken)
                : await _deviceTokenRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            
            return _mapper.Map<IEnumerable<DeviceTokenDto>>(deviceTokens);
        }

        public async Task<IEnumerable<DeviceTokenDto>> Handle(GetDeviceTokensByDeviceTypeQuery request, CancellationToken cancellationToken)
        {
            var deviceTokens = await _deviceTokenRepository.GetByDeviceTypeAsync(request.DeviceType, cancellationToken);
            
            if (request.ActiveOnly)
            {
                deviceTokens = deviceTokens.Where(t => t.IsActive);
            }
            
            return _mapper.Map<IEnumerable<DeviceTokenDto>>(deviceTokens);
        }

        public async Task<IEnumerable<DeviceTokenDto>> Handle(GetExpiredDeviceTokensQuery request, CancellationToken cancellationToken)
        {
            var expiredTokens = await _deviceTokenRepository.GetExpiredTokensAsync(request.ExpirationTime, cancellationToken);
            return _mapper.Map<IEnumerable<DeviceTokenDto>>(expiredTokens);
        }

        public async Task<int> Handle(GetActiveTokenCountByUserQuery request, CancellationToken cancellationToken)
        {
            return await _deviceTokenRepository.GetActiveTokenCountByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}