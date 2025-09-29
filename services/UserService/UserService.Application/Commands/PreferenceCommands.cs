using MediatR;
using UserService.Application.DTOs;
using UserService.Domain.Enums;

namespace UserService.Application.Commands
{
    public class CreatePreferenceCommand : IRequest<PreferenceDto>
    {
        public int UserId { get; set; }
        public GenderType LookingForGender { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public double? MaxDistanceKm { get; set; }
        public string? InterestsFilter { get; set; } // JSON string
        public EmotionType? Emotion { get; set; }
        public VoiceQualityType? VoiceQuality { get; set; }
        public AccentType? Accent { get; set; }
    }

    public class UpdatePreferenceCommand : IRequest<PreferenceDto>
    {
        public int UserId { get; set; }
        public GenderType? LookingForGender { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public double? MaxDistanceKm { get; set; }
        public string? InterestsFilter { get; set; } // JSON string
        public EmotionType? Emotion { get; set; }
        public VoiceQualityType? VoiceQuality { get; set; }
        public AccentType? Accent { get; set; }
    }

    public class DeletePreferenceCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}