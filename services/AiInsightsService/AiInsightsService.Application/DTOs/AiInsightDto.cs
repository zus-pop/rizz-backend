namespace AiInsightsService.Application.DTOs;

public class AiInsightDto
{
    public int UserId { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    public float CompatibilityScore { get; set; }
    public List<string> PersonalityTags { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
    public string CompatibilityLevel { get; set; } = string.Empty;
}

public class CreateAiInsightDto
{
    public string SummaryText { get; set; } = string.Empty;
    public float CompatibilityScore { get; set; }
    public List<string>? PersonalityTags { get; set; }
}

public class UpdateAiInsightDto
{
    public string SummaryText { get; set; } = string.Empty;
    public float CompatibilityScore { get; set; }
    public List<string>? PersonalityTags { get; set; }
}