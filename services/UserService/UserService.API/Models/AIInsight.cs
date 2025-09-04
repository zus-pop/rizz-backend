using NetTopologySuite.Geometries;

namespace UserService.API.Models
{
    public class AIInsight
    {
        public int UserId { get; set; }
        public string SummaryText { get; set; }
        public float CompatibilityScore { get; set; }
        public string PersonalityTags { get; set; } // JSON string
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
