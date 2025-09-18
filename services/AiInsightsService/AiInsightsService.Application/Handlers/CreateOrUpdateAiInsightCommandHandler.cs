using MediatR;
using AiInsightsService.Application.Commands;
using AiInsightsService.Domain.Entities;
using AiInsightsService.Domain.Interfaces;

namespace AiInsightsService.Application.Handlers;

public class CreateOrUpdateAiInsightCommandHandler : IRequestHandler<CreateOrUpdateAiInsightCommand, int>
{
    private readonly IAiInsightRepository _repository;

    public CreateOrUpdateAiInsightCommandHandler(IAiInsightRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateOrUpdateAiInsightCommand request, CancellationToken cancellationToken)
    {
        var existingInsight = await _repository.GetByUserIdAsync(request.UserId);

        if (existingInsight == null)
        {
            var newInsight = new AiInsight(
                request.UserId,
                request.SummaryText,
                request.CompatibilityScore,
                request.PersonalityTags
            );

            await _repository.CreateAsync(newInsight);
            return newInsight.UserId;
        }
        else
        {
            existingInsight.UpdateInsight(
                request.SummaryText,
                request.CompatibilityScore,
                request.PersonalityTags
            );

            await _repository.UpdateAsync(existingInsight);
            return existingInsight.UserId;
        }
    }
}