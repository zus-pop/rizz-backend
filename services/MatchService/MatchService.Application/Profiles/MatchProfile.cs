using AutoMapper;
using MatchService.Application.DTOs;
using MatchService.Domain.Entities;

namespace MatchService.Application.Profiles
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            // Match mappings
            CreateMap<Match, MatchDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.User1Id, opt => opt.MapFrom(src => src.User1Id))
                .ForMember(dest => dest.User2Id, opt => opt.MapFrom(src => src.User2Id))
                .ForMember(dest => dest.MatchedAt, opt => opt.MapFrom(src => src.MatchedAt))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.UnmatchedAt, opt => opt.MapFrom(src => src.UnmatchedAt))
                .ForMember(dest => dest.UnmatchedByUserId, opt => opt.MapFrom(src => src.UnmatchedByUserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // Swipe mappings
            CreateMap<Swipe, SwipeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SwiperId, opt => opt.MapFrom(src => src.SwiperId))
                .ForMember(dest => dest.TargetUserId, opt => opt.MapFrom(src => src.TargetUserId))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()))
                .ForMember(dest => dest.SwipedAt, opt => opt.MapFrom(src => src.SwipedAt))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // Reverse mappings for creation scenarios
            CreateMap<CreateMatchDto, Match>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MatchedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UnmatchedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UnmatchedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CreateSwipeDto, Swipe>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => 
                    Enum.Parse<SwipeDirection>(src.Direction, true)))
                .ForMember(dest => dest.SwipedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}