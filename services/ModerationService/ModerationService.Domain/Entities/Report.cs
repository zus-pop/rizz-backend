using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Entities;

public class Report
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId ReporterId { get; private set; }
    public UserId ReportedUserId { get; private set; }
    public ReportReason Reason { get; private set; }
    public string? Description { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; private set; }
    public UserId? ReviewedBy { get; private set; }
    public ModerationAction? Action { get; private set; }
    public string? ReviewNotes { get; private set; }

    // For EF Core
    private Report()
    {
        ReporterId = UserId.Create(1);
        ReportedUserId = UserId.Create(1);
        Reason = ReportReason.Other;
        Status = ReportStatus.Pending;
    }

    public Report(UserId reporterId, UserId reportedUserId, ReportReason reason, string? description = null)
    {
        if (reporterId.Value == reportedUserId.Value)
            throw new InvalidOperationException("User cannot report themselves");

        ReporterId = reporterId ?? throw new ArgumentNullException(nameof(reporterId));
        ReportedUserId = reportedUserId ?? throw new ArgumentNullException(nameof(reportedUserId));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        Description = description?.Trim();
        Status = ReportStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void StartReview(UserId reviewerId)
    {
        if (Status != ReportStatus.Pending)
            throw new InvalidOperationException($"Cannot start review for report with status {Status}");

        Status = ReportStatus.UnderReview;
        ReviewedBy = reviewerId ?? throw new ArgumentNullException(nameof(reviewerId));
        ReviewedAt = DateTime.UtcNow;
    }

    public void Resolve(ModerationAction action, string? notes = null)
    {
        if (Status != ReportStatus.UnderReview)
            throw new InvalidOperationException($"Cannot resolve report with status {Status}");

        Action = action ?? throw new ArgumentNullException(nameof(action));
        ReviewNotes = notes?.Trim();
        Status = ReportStatus.Resolved;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Dismiss(string? notes = null)
    {
        if (Status != ReportStatus.UnderReview)
            throw new InvalidOperationException($"Cannot dismiss report with status {Status}");

        Action = ModerationAction.NoAction;
        ReviewNotes = notes?.Trim();
        Status = ReportStatus.Dismissed;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Escalate(string? notes = null)
    {
        if (Status != ReportStatus.UnderReview)
            throw new InvalidOperationException($"Cannot escalate report with status {Status}");

        ReviewNotes = notes?.Trim();
        Status = ReportStatus.Escalated;
        ReviewedAt = DateTime.UtcNow;
    }

    public bool IsPending => Status == ReportStatus.Pending;
    public bool IsUnderReview => Status == ReportStatus.UnderReview;
    public bool IsCompleted => Status is ReportStatus.Resolved or ReportStatus.Dismissed;
    public bool RequiresImmediateAttention => Reason.RequiresImmediateAction && IsPending;
    public TimeSpan Age => DateTime.UtcNow - CreatedAt;
    public bool IsOverdue => IsPending && Age.TotalDays > 3;
}

public enum ReportStatus
{
    Pending,
    UnderReview,
    Resolved,
    Dismissed,
    Escalated
}