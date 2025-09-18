namespace AiInsightsService.API.Models;

// Request models for Clean Architecture API
public record CreateOrUpdateAiInsightRequest(
    string SummaryText,
    float CompatibilityScore,
    List<string>? PersonalityTags = null
);

public record GenerateAiInsightRequest(
    Dictionary<string, object> UserProfile,
    string? BehaviorData = null
);

// Response models
public record AiInsightResponse(
    int UserId,
    string SummaryText,
    float CompatibilityScore,
    List<string> PersonalityTags,
    DateTime UpdatedAt,
    string CompatibilityLevel
);

public record AiInsightCreatedResponse(
    int UserId,
    string Message
);