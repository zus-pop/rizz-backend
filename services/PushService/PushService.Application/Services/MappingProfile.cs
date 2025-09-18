using AutoMapper;
using PushService.Application.DTOs;
using PushService.Domain.Entities;

namespace PushService.Application.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DeviceToken, DeviceTokenDto>();
            CreateMap<RegisterDeviceTokenDto, DeviceToken>()
                .ConstructUsing(src => new DeviceToken(src.UserId, src.Token, src.DeviceType, src.DeviceId));
        }
    }
}