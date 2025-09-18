using MediatR;

namespace AiInsightsService.Application.Commands;

public record GenerateAiInsightCommand(
    int UserId,
    Dictionary<string, object> UserProfile,
    string? BehaviorData = null
) : IRequest<int>;