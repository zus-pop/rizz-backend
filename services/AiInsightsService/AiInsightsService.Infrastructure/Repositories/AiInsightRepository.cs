using Microsoft.EntityFrameworkCore;
using AiInsightsService.Domain.Entities;
using AiInsightsService.Domain.Interfaces;
using AiInsightsService.Infrastructure.Data;

namespace AiInsightsService.Infrastructure.Repositories;

public class AiInsightRepository : IAiInsightRepository
{
    private readonly AiInsightsDbContext _context;

    public AiInsightRepository(AiInsightsDbContext context)
    {
        _context = context;
    }

    public async Task<AiInsight?> GetByUserIdAsync(int userId)
    {
        return await _context.AiInsights.FindAsync(userId);
    }

    public async Task<IEnumerable<AiInsight>> GetAllAsync()
    {
        return await _context.AiInsights.ToListAsync();
    }

    public async Task<AiInsight> CreateAsync(AiInsight insight)
    {
        _context.AiInsights.Add(insight);
        await _context.SaveChangesAsync();
        return insight;
    }

    public async Task<AiInsight> UpdateAsync(AiInsight insight)
    {
        _context.AiInsights.Update(insight);
        await _context.SaveChangesAsync();
        return insight;
    }

    public async Task DeleteAsync(int userId)
    {
        var insight = await _context.AiInsights.FindAsync(userId);
        if (insight != null)
        {
            _context.AiInsights.Remove(insight);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int userId)
    {
        return await _context.AiInsights.AnyAsync(ai => ai.UserId == userId);
    }

    public async Task<IEnumerable<AiInsight>> GetByCompatibilityScoreRangeAsync(float minScore, float maxScore)
    {
        return await _context.AiInsights
            .Where(ai => ai.CompatibilityScore >= minScore && ai.CompatibilityScore <= maxScore)
            .OrderByDescending(ai => ai.CompatibilityScore)
            .ToListAsync();
    }

    public async Task<IEnumerable<AiInsight>> SearchByPersonalityTagAsync(string tag)
    {
        // Load all insights and filter in memory since PersonalityTags is stored as JSON
        // This approach ensures compatibility across different database providers
        var allInsights = await _context.AiInsights.ToListAsync();
        
        // Filter in memory for exact tag matches (case-insensitive)
        return allInsights.Where(ai => ai.PersonalityTags
            .Any(t => t.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0))
            .ToList();
    }
}