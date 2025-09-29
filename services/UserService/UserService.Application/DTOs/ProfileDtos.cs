using UserService.Domain.Enums;

namespace UserService.Application.DTOs
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Voice { get; set; }
        public EmotionType? Emotion { get; set; }
        public VoiceQualityType? VoiceQuality { get; set; }
        public AccentType? Accent { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProfileDto
    {
        public string? Bio { get; set; }
        public string? Voice { get; set; }
        public EmotionType? Emotion { get; set; }
        public VoiceQualityType? VoiceQuality { get; set; }
        public AccentType? Accent { get; set; }
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
    }

    public class UpdateProfileDto
    {
        public string? Bio { get; set; }
        public string? Voice { get; set; }
        public EmotionType? Emotion { get; set; }
        public VoiceQualityType? VoiceQuality { get; set; }
        public AccentType? Accent { get; set; }
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
    }
}
