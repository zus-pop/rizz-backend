using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands
{
    public class CreateProfileCommand : IRequest<ProfileDto>
    {
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Voice { get; set; }
        public string? University { get; set; }
        public string? InterestedIn { get; set; }
        public string? LookingFor { get; set; }
        public string? StudyStyle { get; set; }
        public string? WeekendHobby { get; set; }
        public string? CampusLife { get; set; }
        public string? FuturePlan { get; set; }
        public string? CommunicationPreference { get; set; }
        public string? DealBreakers { get; set; }
        public string? Zodiac { get; set; }
        public string? LoveLanguage { get; set; }
        
        // New Vietnamese localization fields
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
        
        // Legacy fields for backward compatibility
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
    }

    public class UpdateProfileCommand : IRequest<ProfileDto>
    {
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Voice { get; set; }
        public string? University { get; set; }
        public string? InterestedIn { get; set; }
        public string? LookingFor { get; set; }
        public string? StudyStyle { get; set; }
        public string? WeekendHobby { get; set; }
        public string? CampusLife { get; set; }
        public string? FuturePlan { get; set; }
        public string? CommunicationPreference { get; set; }
        public string? DealBreakers { get; set; }
        public string? Zodiac { get; set; }
        public string? LoveLanguage { get; set; }
        
        // New Vietnamese localization fields
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
        
        // Legacy fields for backward compatibility
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool? ShowOnlyVerified { get; set; }
    }

    public class DeleteProfileCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}