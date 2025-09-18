using AutoMapper;
using ModerationService.Application.DTOs;
using ModerationService.Domain.Entities;

namespace ModerationService.Application.Mappings;

public class ModerationMappingProfile : Profile
{
    public ModerationMappingProfile()
    {
        CreateMap<Block, BlockDto>()
            .ForMember(dest => dest.BlockerId, opt => opt.MapFrom(src => src.BlockerId.Value))
            .ForMember(dest => dest.BlockedUserId, opt => opt.MapFrom(src => src.BlockedUserId.Value));

        CreateMap<Report, ReportDto>()
            .ForMember(dest => dest.ReporterId, opt => opt.MapFrom(src => src.ReporterId.Value))
            .ForMember(dest => dest.ReportedUserId, opt => opt.MapFrom(src => src.ReportedUserId.Value))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ReportedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ReviewedBy, opt => opt.MapFrom(src => src.ReviewedBy != null ? src.ReviewedBy.Value : (int?)null));

        CreateMap<ModerationCase, ModerationCaseDto>()
            .ForMember(dest => dest.TargetUserId, opt => opt.MapFrom(src => src.TargetUserId.Value))
            .ForMember(dest => dest.CaseType, opt => opt.MapFrom(src => "Standard")) // Default case type
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.AssignedToUserId, opt => opt.MapFrom(src => src.AssignedTo != null ? src.AssignedTo.Value : (int?)null))
            .ForMember(dest => dest.RelatedReportIds, opt => opt.MapFrom(src => src.ReportIds));
    }
}