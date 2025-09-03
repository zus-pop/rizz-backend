using NetTopologySuite.Geometries;

namespace UserService.API.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string Bio { get; set; }
        public string Voice { get; set; }
        public string University { get; set; }
        public string InterestedIn { get; set; }
        public string LookingFor { get; set; }
        public string StudyStyle { get; set; }
        public string WeekendHobby { get; set; }
        public string CampusLife { get; set; }
        public string FuturePlan { get; set; }
        public string CommunicationPreference { get; set; }
        public string DealBreakers { get; set; }
        public string Zodiac { get; set; }
        public string LoveLanguage { get; set; }
    }
}
