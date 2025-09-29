namespace UserService.Domain.Entities
{
    public class Profile : BaseEntity
    {
        public int UserId { get; private set; }
        public string? Bio { get; private set; }
        public string? Voice { get; private set; } // URL to audio intro
        public string? University { get; private set; }
        public string? InterestedIn { get; private set; }
        public string? LookingFor { get; private set; }
        public string? StudyStyle { get; private set; }
        public string? WeekendHobby { get; private set; }
        public string? CampusLife { get; private set; }
        public string? FuturePlan { get; private set; }
        public string? CommunicationPreference { get; private set; }
        public string? DealBreakers { get; private set; }
        public string? Zodiac { get; private set; }
        public string? LoveLanguage { get; private set; }
        
        // Legacy fields for backward compatibility
        public string? Job { get; private set; }
        public string? School { get; private set; }
        public int? InterestedInAgeMin { get; private set; }
        public int? InterestedInAgeMax { get; private set; }
        public string? InterestedInGender { get; private set; }
        public double? MaxDistanceKm { get; private set; }
        public bool ShowOnlyVerified { get; private set; }

        private Profile() { } // For EF Core

        public Profile(int userId)
        {
            UserId = userId;
        }

        public void SetBio(string? bio)
        {
            Bio = bio?.Trim();
            SetUpdatedAt();
        }

        public void SetJob(string? job)
        {
            Job = job?.Trim();
            SetUpdatedAt();
        }

        public void SetSchool(string? school)
        {
            School = school?.Trim();
            SetUpdatedAt();
        }

        public void SetInterestedInAgeRange(int? minAge, int? maxAge)
        {
            if (minAge.HasValue && minAge < 18)
                throw new ArgumentException("Minimum age cannot be less than 18", nameof(minAge));

            if (maxAge.HasValue && maxAge > 100)
                throw new ArgumentException("Maximum age cannot be greater than 100", nameof(maxAge));

            if (minAge.HasValue && maxAge.HasValue && minAge > maxAge)
                throw new ArgumentException("Minimum age cannot be greater than maximum age");

            InterestedInAgeMin = minAge;
            InterestedInAgeMax = maxAge;
            SetUpdatedAt();
        }

        public void SetInterestedInGender(string? gender)
        {
            if (!string.IsNullOrEmpty(gender))
            {
                var validGenders = new[] { "male", "female", "both" };
                if (!validGenders.Contains(gender.ToLower()))
                    throw new ArgumentException("Invalid gender preference", nameof(gender));
                
                InterestedInGender = gender.ToLower();
            }
            else
            {
                InterestedInGender = null;
            }
            
            SetUpdatedAt();
        }

        public void SetMaxDistance(double? maxDistanceKm)
        {
            if (maxDistanceKm.HasValue && (maxDistanceKm <= 0 || maxDistanceKm > 1000))
                throw new ArgumentException("Max distance must be between 0 and 1000 km", nameof(maxDistanceKm));

            MaxDistanceKm = maxDistanceKm;
            SetUpdatedAt();
        }

        public void SetShowOnlyVerified(bool showOnlyVerified)
        {
            ShowOnlyVerified = showOnlyVerified;
            SetUpdatedAt();
        }

        // New field setters
        public void SetVoice(string? voice)
        {
            Voice = voice?.Trim();
            SetUpdatedAt();
        }

        public void SetUniversity(string? university)
        {
            University = university?.Trim();
            SetUpdatedAt();
        }

        public void SetInterestedIn(string? interestedIn)
        {
            InterestedIn = interestedIn?.Trim();
            SetUpdatedAt();
        }

        public void SetLookingFor(string? lookingFor)
        {
            LookingFor = lookingFor?.Trim();
            SetUpdatedAt();
        }

        public void SetStudyStyle(string? studyStyle)
        {
            StudyStyle = studyStyle?.Trim();
            SetUpdatedAt();
        }

        public void SetWeekendHobby(string? weekendHobby)
        {
            WeekendHobby = weekendHobby?.Trim();
            SetUpdatedAt();
        }

        public void SetCampusLife(string? campusLife)
        {
            CampusLife = campusLife?.Trim();
            SetUpdatedAt();
        }

        public void SetFuturePlan(string? futurePlan)
        {
            FuturePlan = futurePlan?.Trim();
            SetUpdatedAt();
        }

        public void SetCommunicationPreference(string? communicationPreference)
        {
            if (!string.IsNullOrEmpty(communicationPreference))
            {
                var validPreferences = new[] { "texting", "calling", "video_calls", "in_person", "social_media" };
                if (!validPreferences.Contains(communicationPreference.ToLower()))
                    throw new ArgumentException("Invalid communication preference", nameof(communicationPreference));
                
                CommunicationPreference = communicationPreference.ToLower();
            }
            else
            {
                CommunicationPreference = null;
            }
            
            SetUpdatedAt();
        }

        public void SetDealBreakers(string? dealBreakers)
        {
            DealBreakers = dealBreakers?.Trim();
            SetUpdatedAt();
        }

        public void SetZodiac(string? zodiac)
        {
            if (!string.IsNullOrEmpty(zodiac))
            {
                var validSigns = new[] { "aries", "taurus", "gemini", "cancer", "leo", "virgo", 
                                       "libra", "scorpio", "sagittarius", "capricorn", "aquarius", "pisces" };
                if (!validSigns.Contains(zodiac.ToLower()))
                    throw new ArgumentException("Invalid zodiac sign", nameof(zodiac));
                
                Zodiac = zodiac.ToLower();
            }
            else
            {
                Zodiac = null;
            }
            
            SetUpdatedAt();
        }

        public void SetLoveLanguage(string? loveLanguage)
        {
            if (!string.IsNullOrEmpty(loveLanguage))
            {
                var validLanguages = new[] { "words_of_affirmation", "acts_of_service", "receiving_gifts", 
                                           "quality_time", "physical_touch" };
                if (!validLanguages.Contains(loveLanguage.ToLower()))
                    throw new ArgumentException("Invalid love language", nameof(loveLanguage));
                
                LoveLanguage = loveLanguage.ToLower();
            }
            else
            {
                LoveLanguage = null;
            }
            
            SetUpdatedAt();
        }
    }
}