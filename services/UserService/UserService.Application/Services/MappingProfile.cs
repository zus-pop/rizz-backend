using AutoMapper;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using DomainProfile = UserService.Domain.Entities.Profile;

namespace UserService.Application.Services
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => 
                    src.Location != null ? new LocationDto 
                    { 
                        Latitude = src.Location.Latitude, 
                        Longitude = src.Location.Longitude 
                    } : null));

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore());

            // Profile mappings
            CreateMap<DomainProfile, ProfileDto>();
            CreateMap<CreateProfileDto, DomainProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Photo mappings
            CreateMap<Photo, PhotoDto>();
            CreateMap<CreatePhotoDto, Photo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // AIInsight mappings
            CreateMap<AIInsight, AIInsightDto>();

            // DeviceToken mappings
            CreateMap<DeviceToken, DeviceTokenDto>();

            // Preference mappings
            CreateMap<Preference, PreferenceDto>();
        }
    }

    // Additional DTOs for completeness
    public class AIInsightDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? PersonalityAnalysis { get; set; }
        public string? CompatibilityFactors { get; set; }
        public string? ImprovementSuggestions { get; set; }
        public double? ConfidenceScore { get; set; }
        public DateTime? LastAnalyzedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class DeviceTokenDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PreferenceDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}