using System.Text.Json;
using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class Preference : BaseEntity
    {
        public int UserId { get; private set; }
        public GenderType? LookingForGender { get; private set; }
        public int? AgeRangeMin { get; private set; }
        public int? AgeRangeMax { get; private set; }
        public int? LocationRadiusKm { get; private set; }
        public string? InterestsFilter { get; private set; } // JSON string for interests
        public string? Emotion { get; private set; }
        public string? VoiceQuality { get; private set; }
        public string? Accent { get; private set; }

        private Preference() { } // For EF Core

        public Preference(int userId)
        {
            UserId = userId;
        }

        public void SetLookingForGender(GenderType? gender)
        {
            LookingForGender = gender;
            SetUpdatedAt();
        }

        public void SetAgeRange(int? minAge, int? maxAge)
        {
            if (minAge.HasValue && minAge < 18)
                throw new ArgumentException("Minimum age cannot be less than 18", nameof(minAge));

            if (maxAge.HasValue && maxAge > 100)
                throw new ArgumentException("Maximum age cannot be greater than 100", nameof(maxAge));

            if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
                throw new ArgumentException("Minimum age cannot be greater than maximum age");

            AgeRangeMin = minAge;
            AgeRangeMax = maxAge;
            SetUpdatedAt();
        }

        public void SetLocationRadius(int? radiusKm)
        {
            if (radiusKm.HasValue && (radiusKm <= 0 || radiusKm > 1000))
                throw new ArgumentException("Location radius must be between 0 and 1000 km", nameof(radiusKm));

            LocationRadiusKm = radiusKm;
            SetUpdatedAt();
        }

        public void SetInterestsFilter(object? interests)
        {
            if (interests != null)
            {
                InterestsFilter = JsonSerializer.Serialize(interests);
            }
            else
            {
                InterestsFilter = null;
            }
            SetUpdatedAt();
        }

        public void SetEmotion(string? emotion)
        {
            Emotion = emotion?.Trim();
            SetUpdatedAt();
        }

        public void SetVoiceQuality(string? voiceQuality)
        {
            VoiceQuality = voiceQuality?.Trim();
            SetUpdatedAt();
        }

        public void SetAccent(string? accent)
        {
            Accent = accent?.Trim();
            SetUpdatedAt();
        }
    }
}