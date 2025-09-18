using MediatR;

namespace AiInsightsService.Application.Commands;

public record CreateOrUpdateAiInsightCommand(
    int UserId,
    string SummaryText,
    float CompatibilityScore,
    List<string>? PersonalityTags = null
) : IRequest<int>;