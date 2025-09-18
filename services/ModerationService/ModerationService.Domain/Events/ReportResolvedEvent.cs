using Common.Domain;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Events;

public record ReportResolvedEvent(
    Guid ReportId,
    int ReportedUserId,
    string Action,
    int ReviewedBy,
    DateTime ResolvedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
};