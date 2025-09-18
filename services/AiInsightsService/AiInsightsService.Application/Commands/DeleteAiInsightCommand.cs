using MediatR;

namespace AiInsightsService.Application.Commands;

public record DeleteAiInsightCommand(int UserId) : IRequest<bool>;