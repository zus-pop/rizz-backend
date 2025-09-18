using ModerationService.Domain.Entities;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Domain.Services;

public interface IModerationPolicyService
{
    ModerationAction DetermineAction(Report report, IEnumerable<Report> userReportHistory);
    ModerationCasePriority DetermineCasePriority(Report report, IEnumerable<Report> relatedReports);
    bool ShouldCreateModerationCase(Report report, IEnumerable<Report> userReports);
    bool ShouldEscalateCase(ModerationCase moderationCase);
}

public class ModerationPolicyService : IModerationPolicyService
{
    public ModerationAction DetermineAction(Report report, IEnumerable<Report> userReportHistory)
    {
        var reportCount = userReportHistory.Count();
        var seriousReports = userReportHistory.Count(r => r.Reason.IsSerious);

        // Immediate action for serious violations
        if (report.Reason.RequiresImmediateAction)
        {
            return report.Reason.Value switch
            {
                "Violence" => ModerationAction.AccountSuspension,
                "UnderAge" => ModerationAction.PermanentBan,
                "HateSpeech" => ModerationAction.AccountSuspension,
                _ => ModerationAction.TemporaryRestriction
            };
        }

        // Progressive action based on history
        return (reportCount, seriousReports) switch
        {
            (1, 0) => ModerationAction.Warning,
            (< 3, 0) => ModerationAction.Warning,
            (< 5, < 2) => ModerationAction.TemporaryRestriction,
            (< 10, < 5) => ModerationAction.AccountSuspension,
            _ => ModerationAction.PermanentBan
        };
    }

    public ModerationCasePriority DetermineCasePriority(Report report, IEnumerable<Report> relatedReports)
    {
        if (report.Reason.RequiresImmediateAction)
            return ModerationCasePriority.Critical;

        if (report.Reason.IsSerious)
            return ModerationCasePriority.High;

        var relatedCount = relatedReports.Count();
        return relatedCount switch
        {
            >= 5 => ModerationCasePriority.High,
            >= 2 => ModerationCasePriority.Medium,
            _ => ModerationCasePriority.Low
        };
    }

    public bool ShouldCreateModerationCase(Report report, IEnumerable<Report> userReports)
    {
        // Always create case for serious violations
        if (report.Reason.IsSerious)
            return true;

        // Create case if user has multiple reports
        return userReports.Count() >= 3;
    }

    public bool ShouldEscalateCase(ModerationCase moderationCase)
    {
        // Escalate if case is taking too long
        if (moderationCase.IsOverdue)
            return true;

        // Escalate if multiple reports for same user
        if (moderationCase.ReportIds.Count >= 5)
            return true;

        return false;
    }
}