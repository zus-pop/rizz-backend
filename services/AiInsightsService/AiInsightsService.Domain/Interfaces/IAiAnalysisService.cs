namespace AiInsightsService.Domain.Interfaces;

/// <summary>
/// Domain service for handling AI-related business logic
/// </summary>
public interface IAiAnalysisService
{
    Task<float> CalculateCompatibilityScoreAsync(int userId, Dictionary<string, object> userProfile);
    Task<List<string>> ExtractPersonalityTagsAsync(string userBehaviorData);
    Task<string> GenerateInsightSummaryAsync(int userId, Dictionary<string, object> userProfile);
    Task<bool> ShouldUpdateInsightAsync(int userId, DateTime lastUpdate);
}