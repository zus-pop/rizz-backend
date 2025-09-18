using AutoMapper;
using MessagingService.Application.DTOs;
using MessagingService.Domain.Entities;
using MessagingService.Domain.Enums;

namespace MessagingService.Application.Mappings
{
    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString().ToLower()));

            CreateMap<CreateMessageDto, Message>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<MessageType>(src.Type, true)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore());

            CreateMap<UpdateMessageDto, Message>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}