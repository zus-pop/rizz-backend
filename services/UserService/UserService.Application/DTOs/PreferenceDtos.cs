using UserService.Domain.Enums;

namespace UserService.Application.DTOs
{
    public class PreferenceDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public GenderType? LookingForGender { get; set; }
        public int? AgeRangeMin { get; set; }
        public int? AgeRangeMax { get; set; }
        public int? LocationRadiusKm { get; set; }
        public string? InterestsFilter { get; set; }
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePreferenceDto
    {
        public GenderType? LookingForGender { get; set; }
        public int? AgeRangeMin { get; set; }
        public int? AgeRangeMax { get; set; }
        public int? LocationRadiusKm { get; set; }
        public object? InterestsFilter { get; set; }
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
    }

    public class UpdatePreferenceDto
    {
        public GenderType? LookingForGender { get; set; }
        public int? AgeRangeMin { get; set; }
        public int? AgeRangeMax { get; set; }
        public int? LocationRadiusKm { get; set; }
        public object? InterestsFilter { get; set; }
        public string? Emotion { get; set; }
        public string? VoiceQuality { get; set; }
        public string? Accent { get; set; }
    }
}