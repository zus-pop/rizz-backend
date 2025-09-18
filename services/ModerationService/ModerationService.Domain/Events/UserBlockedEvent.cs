using Common.Domain;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Events;

public record UserBlockedEvent(
    Guid BlockId,
    int BlockerId,
    int BlockedUserId,
    string? Reason,
    DateTime BlockedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
};