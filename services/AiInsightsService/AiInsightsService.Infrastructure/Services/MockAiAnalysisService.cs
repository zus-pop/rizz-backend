using AiInsightsService.Domain.Interfaces;

namespace AiInsightsService.Infrastructure.Services;

/// <summary>
/// Mock implementation of AI Analysis Service for development purposes
/// In production, this would connect to actual AI/ML services
/// </summary>
public class MockAiAnalysisService : IAiAnalysisService
{
    private readonly Random _random = new();

    public async Task<float> CalculateCompatibilityScoreAsync(int userId, Dictionary<string, object> userProfile)
    {
        // Mock compatibility score calculation
        await Task.Delay(100); // Simulate AI processing time
        
        // Generate a more realistic score based on user ID for consistency
        var seed = userId * 17; // Use user ID as part of seed for consistency
        var random = new Random(seed);
        
        // Generate score between 20-95 for more realistic distribution
        return (float)(random.NextDouble() * 75 + 20);
    }

    public async Task<List<string>> ExtractPersonalityTagsAsync(string userBehaviorData)
    {
        await Task.Delay(50); // Simulate processing time

        var possibleTags = new[]
        {
            "outgoing", "introvert", "analytical", "creative", "adventurous",
            "ambitious", "empathetic", "logical", "spontaneous", "organized",
            "social", "independent", "optimistic", "thoughtful", "energetic",
            "calm", "passionate", "practical", "imaginative", "confident"
        };

        // Return 2-5 random tags
        var tagCount = _random.Next(2, 6);
        return possibleTags.OrderBy(x => _random.Next()).Take(tagCount).ToList();
    }

    public async Task<string> GenerateInsightSummaryAsync(int userId, Dictionary<string, object> userProfile)
    {
        await Task.Delay(200); // Simulate AI text generation time

        var summaryTemplates = new[]
        {
            "This user shows strong compatibility patterns and demonstrates excellent communication skills. Their profile suggests someone who values meaningful connections.",
            "Analysis indicates a well-rounded individual with balanced social and intellectual interests. Strong potential for lasting relationships.",
            "Profile demonstrates high emotional intelligence and adaptability. User appears to prioritize authentic connections and personal growth.",
            "The user exhibits traits of a thoughtful communicator with genuine interest in others. Strong foundation for relationship building.",
            "Analysis suggests someone with diverse interests and strong social awareness. Excellent compatibility potential with similar personalities."
        };

        var index = userId % summaryTemplates.Length;
        return summaryTemplates[index];
    }

    public async Task<bool> ShouldUpdateInsightAsync(int userId, DateTime lastUpdate)
    {
        await Task.Delay(10);
        
        // Update if insight is older than 7 days
        return DateTime.UtcNow - lastUpdate > TimeSpan.FromDays(7);
    }
}