using Common.Domain;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Events;

public record UserReportedEvent(
    Guid ReportId,
    int ReporterId,
    int ReportedUserId,
    string Reason,
    string? Description,
    DateTime ReportedAt
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
};