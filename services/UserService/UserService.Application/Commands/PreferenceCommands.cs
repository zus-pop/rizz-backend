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
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
    }

    public class UpdatePreferenceCommand : IRequest<PreferenceDto>
    {
        public int UserId { get; set; }
        public GenderType? LookingForGender { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public double? MaxDistanceKm { get; set; }
        public string? InterestsFilter { get; set; } // JSON string
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
    }

    public class DeletePreferenceCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}