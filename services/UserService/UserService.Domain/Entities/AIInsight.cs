namespace UserService.Domain.Entities
{
    public class AIInsight : BaseEntity
    {
        public int UserId { get; private set; }
        public string? PersonalityAnalysis { get; private set; }
        public string? CompatibilityFactors { get; private set; }
        public string? ImprovementSuggestions { get; private set; }
        public double? ConfidenceScore { get; private set; }
        public DateTime? LastAnalyzedAt { get; private set; }

        private AIInsight() { } // For EF Core

        public AIInsight(int userId)
        {
            UserId = userId;
        }

        public void UpdatePersonalityAnalysis(string personalityAnalysis)
        {
            if (string.IsNullOrWhiteSpace(personalityAnalysis))
                throw new ArgumentException("Personality analysis cannot be empty", nameof(personalityAnalysis));

            PersonalityAnalysis = personalityAnalysis.Trim();
            LastAnalyzedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void UpdateCompatibilityFactors(string compatibilityFactors)
        {
            if (string.IsNullOrWhiteSpace(compatibilityFactors))
                throw new ArgumentException("Compatibility factors cannot be empty", nameof(compatibilityFactors));

            CompatibilityFactors = compatibilityFactors.Trim();
            LastAnalyzedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void UpdateImprovementSuggestions(string improvementSuggestions)
        {
            if (string.IsNullOrWhiteSpace(improvementSuggestions))
                throw new ArgumentException("Improvement suggestions cannot be empty", nameof(improvementSuggestions));

            ImprovementSuggestions = improvementSuggestions.Trim();
            LastAnalyzedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void SetConfidenceScore(double confidenceScore)
        {
            if (confidenceScore < 0 || confidenceScore > 1)
                throw new ArgumentException("Confidence score must be between 0 and 1", nameof(confidenceScore));

            ConfidenceScore = confidenceScore;
            SetUpdatedAt();
        }

        public void UpdateAnalysis(string personalityAnalysis, string compatibilityFactors, 
                                 string improvementSuggestions, double confidenceScore)
        {
            UpdatePersonalityAnalysis(personalityAnalysis);
            UpdateCompatibilityFactors(compatibilityFactors);
            UpdateImprovementSuggestions(improvementSuggestions);
            SetConfidenceScore(confidenceScore);
        }
    }
}