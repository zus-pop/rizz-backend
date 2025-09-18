using AiInsightsService.Domain.Entities;

namespace AiInsightsService.Domain.Interfaces;

public interface IAiInsightRepository
{
    Task<AiInsight?> GetByUserIdAsync(int userId);
    Task<IEnumerable<AiInsight>> GetAllAsync();
    Task<AiInsight> CreateAsync(AiInsight insight);
    Task<AiInsight> UpdateAsync(AiInsight insight);
    Task DeleteAsync(int userId);
    Task<bool> ExistsAsync(int userId);
    Task<IEnumerable<AiInsight>> GetByCompatibilityScoreRangeAsync(float minScore, float maxScore);
    Task<IEnumerable<AiInsight>> SearchByPersonalityTagAsync(string tag);
}