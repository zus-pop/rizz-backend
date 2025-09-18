using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Entities;

public class ModerationCase
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId TargetUserId { get; private set; }
    public List<Guid> ReportIds { get; private set; } = new();
    public ModerationCasePriority Priority { get; private set; }
    public ModerationCaseStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? AssignedAt { get; private set; }
    public UserId? AssignedTo { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public ModerationAction? FinalAction { get; private set; }
    public string? Resolution { get; private set; }

    // For EF Core
    private ModerationCase()
    {
        TargetUserId = UserId.Create(1);
        Priority = ModerationCasePriority.Low;
        Status = ModerationCaseStatus.Open;
    }

    public ModerationCase(UserId targetUserId, ModerationCasePriority priority = ModerationCasePriority.Low)
    {
        TargetUserId = targetUserId ?? throw new ArgumentNullException(nameof(targetUserId));
        Priority = priority;
        Status = ModerationCaseStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddReport(Guid reportId)
    {
        if (Status != ModerationCaseStatus.Open)
            throw new InvalidOperationException("Cannot add reports to closed case");

        if (!ReportIds.Contains(reportId))
        {
            ReportIds.Add(reportId);
        }
    }

    public void AssignTo(UserId moderatorId)
    {
        if (Status != ModerationCaseStatus.Open)
            throw new InvalidOperationException("Cannot assign closed case");

        AssignedTo = moderatorId ?? throw new ArgumentNullException(nameof(moderatorId));
        AssignedAt = DateTime.UtcNow;
        Status = ModerationCaseStatus.InProgress;
    }

    public void Escalate()
    {
        if (Status != ModerationCaseStatus.InProgress)
            throw new InvalidOperationException("Can only escalate cases in progress");

        Priority = Priority switch
        {
            ModerationCasePriority.Low => ModerationCasePriority.Medium,
            ModerationCasePriority.Medium => ModerationCasePriority.High,
            ModerationCasePriority.High => ModerationCasePriority.Critical,
            ModerationCasePriority.Critical => ModerationCasePriority.Critical,
            _ => ModerationCasePriority.Medium
        };
        Status = ModerationCaseStatus.Escalated;
    }

    public void Resolve(ModerationAction action, string resolution)
    {
        if (Status == ModerationCaseStatus.Resolved)
            throw new InvalidOperationException("Case is already resolved");

        FinalAction = action ?? throw new ArgumentNullException(nameof(action));
        Resolution = resolution?.Trim() ?? throw new ArgumentException("Resolution cannot be empty", nameof(resolution));
        Status = ModerationCaseStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }

    public bool IsOpen => Status == ModerationCaseStatus.Open;
    public bool IsInProgress => Status == ModerationCaseStatus.InProgress;
    public bool IsAssigned => AssignedTo != null;
    public TimeSpan Age => DateTime.UtcNow - CreatedAt;
    public bool IsOverdue => IsInProgress && Age.TotalDays > (Priority == ModerationCasePriority.Critical ? 1 : 7);
}

public enum ModerationCaseStatus
{
    Open,
    InProgress,
    Escalated,
    Resolved
}

public enum ModerationCasePriority
{
    Low,
    Medium,
    High,
    Critical
}