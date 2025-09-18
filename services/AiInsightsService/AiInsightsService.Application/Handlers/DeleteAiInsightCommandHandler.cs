using MediatR;
using AiInsightsService.Application.Commands;
using AiInsightsService.Domain.Interfaces;

namespace AiInsightsService.Application.Handlers;

public class DeleteAiInsightCommandHandler : IRequestHandler<DeleteAiInsightCommand, bool>
{
    private readonly IAiInsightRepository _repository;

    public DeleteAiInsightCommandHandler(IAiInsightRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteAiInsightCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(request.UserId);
        if (!exists)
            return false;

        await _repository.DeleteAsync(request.UserId);
        return true;
    }
}