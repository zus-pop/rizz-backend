using MediatR;
using AiInsightsService.Application.Commands;
using AiInsightsService.Domain.Entities;
using AiInsightsService.Domain.Interfaces;

namespace AiInsightsService.Application.Handlers;

public class GenerateAiInsightCommandHandler : IRequestHandler<GenerateAiInsightCommand, int>
{
    private readonly IAiInsightRepository _repository;
    private readonly IAiAnalysisService _aiAnalysisService;

    public GenerateAiInsightCommandHandler(
        IAiInsightRepository repository,
        IAiAnalysisService aiAnalysisService)
    {
        _repository = repository;
        _aiAnalysisService = aiAnalysisService;
    }

    public async Task<int> Handle(GenerateAiInsightCommand request, CancellationToken cancellationToken)
    {
        // Check if we should update existing insight
        var existingInsight = await _repository.GetByUserIdAsync(request.UserId);
        if (existingInsight != null)
        {
            var shouldUpdate = await _aiAnalysisService.ShouldUpdateInsightAsync(
                request.UserId, 
                existingInsight.UpdatedAt
            );
            
            if (!shouldUpdate)
                return existingInsight.UserId;
        }

        // Generate new insight using AI analysis
        var compatibilityScore = await _aiAnalysisService.CalculateCompatibilityScoreAsync(
            request.UserId, 
            request.UserProfile
        );

        var personalityTags = await _aiAnalysisService.ExtractPersonalityTagsAsync(
            request.BehaviorData ?? string.Empty
        );

        var summaryText = await _aiAnalysisService.GenerateInsightSummaryAsync(
            request.UserId,
            request.UserProfile
        );

        if (existingInsight == null)
        {
            var newInsight = new AiInsight(
                request.UserId,
                summaryText,
                compatibilityScore,
                personalityTags
            );

            await _repository.CreateAsync(newInsight);
            return newInsight.UserId;
        }
        else
        {
            existingInsight.UpdateInsight(summaryText, compatibilityScore, personalityTags);
            await _repository.UpdateAsync(existingInsight);
            return existingInsight.UserId;
        }
    }
}