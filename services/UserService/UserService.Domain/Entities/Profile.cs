using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class Profile : BaseEntity
    {
        public int UserId { get; private set; }
        public string? Bio { get; private set; }
        public string? Voice { get; private set; } // URL to audio intro
        public EmotionType? Emotion { get; private set; }
        public VoiceQualityType? VoiceQuality { get; private set; }
        public AccentType? Accent { get; private set; }
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

        public void SetVoice(string? voice)
        {
            Voice = voice?.Trim();
            SetUpdatedAt();
        }

        public void SetEmotion(EmotionType? emotion)
        {
            Emotion = emotion;
            SetUpdatedAt();
        }

        public void SetVoiceQuality(VoiceQualityType? voiceQuality)
        {
            VoiceQuality = voiceQuality;
            SetUpdatedAt();
        }

        public void SetAccent(AccentType? accent)
        {
            Accent = accent;
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
            CommunicationPreference = communicationPreference?.Trim();
            SetUpdatedAt();
        }

        public void SetDealBreakers(string? dealBreakers)
        {
            DealBreakers = dealBreakers?.Trim();
            SetUpdatedAt();
        }

        public void SetZodiac(string? zodiac)
        {
            Zodiac = zodiac?.Trim();
            SetUpdatedAt();
        }

        public void SetLoveLanguage(string? loveLanguage)
        {
            LoveLanguage = loveLanguage?.Trim();
            SetUpdatedAt();
        }
    }
}