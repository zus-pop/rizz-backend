using MediatR;
using AiInsightsService.Application.DTOs;
using AiInsightsService.Application.Queries;
using AiInsightsService.Domain.Interfaces;
using AiInsightsService.Domain.ValueObjects;

namespace AiInsightsService.Application.Handlers;

public class GetAiInsightByUserIdQueryHandler : IRequestHandler<GetAiInsightByUserIdQuery, AiInsightDto?>
{
    private readonly IAiInsightRepository _repository;

    public GetAiInsightByUserIdQueryHandler(IAiInsightRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiInsightDto?> Handle(GetAiInsightByUserIdQuery request, CancellationToken cancellationToken)
    {
        var insight = await _repository.GetByUserIdAsync(request.UserId);
        if (insight == null)
            return null;

        return new AiInsightDto
        {
            UserId = insight.UserId,
            SummaryText = insight.SummaryText,
            CompatibilityScore = insight.CompatibilityScore,
            PersonalityTags = insight.PersonalityTags,
            UpdatedAt = insight.UpdatedAt,
            CompatibilityLevel = new CompatibilityScore(insight.CompatibilityScore).GetCompatibilityLevel()
        };
    }
}

public class GetAllAiInsightsQueryHandler : IRequestHandler<GetAllAiInsightsQuery, IEnumerable<AiInsightDto>>
{
    private readonly IAiInsightRepository _repository;

    public GetAllAiInsightsQueryHandler(IAiInsightRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AiInsightDto>> Handle(GetAllAiInsightsQuery request, CancellationToken cancellationToken)
    {
        var insights = await _repository.GetAllAsync();
        return insights.Select(insight => new AiInsightDto
        {
            UserId = insight.UserId,
            SummaryText = insight.SummaryText,
            CompatibilityScore = insight.CompatibilityScore,
            PersonalityTags = insight.PersonalityTags,
            UpdatedAt = insight.UpdatedAt,
            CompatibilityLevel = new CompatibilityScore(insight.CompatibilityScore).GetCompatibilityLevel()
        });
    }
}

public class GetAiInsightsByCompatibilityRangeQueryHandler : IRequestHandler<GetAiInsightsByCompatibilityRangeQuery, IEnumerable<AiInsightDto>>
{
    private readonly IAiInsightRepository _repository;

    public GetAiInsightsByCompatibilityRangeQueryHandler(IAiInsightRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AiInsightDto>> Handle(GetAiInsightsByCompatibilityRangeQuery request, CancellationToken cancellationToken)
    {
        var insights = await _repository.GetByCompatibilityScoreRangeAsync(request.MinScore, request.MaxScore);
        return insights.Select(insight => new AiInsightDto
        {
            UserId = insight.UserId,
            SummaryText = insight.SummaryText,
            CompatibilityScore = insight.CompatibilityScore,
            PersonalityTags = insight.PersonalityTags,
            UpdatedAt = insight.UpdatedAt,
            CompatibilityLevel = new CompatibilityScore(insight.CompatibilityScore).GetCompatibilityLevel()
        });
    }
}

public class SearchAiInsightsByPersonalityTagQueryHandler : IRequestHandler<SearchAiInsightsByPersonalityTagQuery, IEnumerable<AiInsightDto>>
{
    private readonly IAiInsightRepository _repository;

    public SearchAiInsightsByPersonalityTagQueryHandler(IAiInsightRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AiInsightDto>> Handle(SearchAiInsightsByPersonalityTagQuery request, CancellationToken cancellationToken)
    {
        var insights = await _repository.SearchByPersonalityTagAsync(request.Tag);
        return insights.Select(insight => new AiInsightDto
        {
            UserId = insight.UserId,
            SummaryText = insight.SummaryText,
            CompatibilityScore = insight.CompatibilityScore,
            PersonalityTags = insight.PersonalityTags,
            UpdatedAt = insight.UpdatedAt,
            CompatibilityLevel = new CompatibilityScore(insight.CompatibilityScore).GetCompatibilityLevel()
        });
    }
}