namespace AiInsightsService.API.Models
{
    public class AiInsight
    {
        public int UserId { get; set; } // PK, matches USERS.id
        public string SummaryText { get; set; } = string.Empty;
        public float CompatibilityScore { get; set; }
        public string PersonalityTags { get; set; } = "{}"; // stored as JSON
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
