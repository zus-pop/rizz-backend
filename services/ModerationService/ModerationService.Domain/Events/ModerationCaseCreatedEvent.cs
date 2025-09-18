using Common.Domain;

namespace ModerationService.Domain.Events;

public record ModerationCaseCreatedEvent(
    Guid CaseId,
    int TargetUserId,
    string Priority,
    DateTime CreatedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
};