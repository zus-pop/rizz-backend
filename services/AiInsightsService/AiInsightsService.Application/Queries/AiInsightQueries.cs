using MediatR;
using AiInsightsService.Application.DTOs;

namespace AiInsightsService.Application.Queries;

public record GetAiInsightByUserIdQuery(int UserId) : IRequest<AiInsightDto?>;

public record GetAllAiInsightsQuery() : IRequest<IEnumerable<AiInsightDto>>;

public record GetAiInsightsByCompatibilityRangeQuery(
    float MinScore, 
    float MaxScore
) : IRequest<IEnumerable<AiInsightDto>>;

public record SearchAiInsightsByPersonalityTagQuery(
    string Tag
) : IRequest<IEnumerable<AiInsightDto>>;