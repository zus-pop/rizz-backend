namespace ModerationService.Application.DTOs;

public class ModerationCaseDto
{
    public Guid Id { get; set; }
    public int TargetUserId { get; set; }
    public string CaseType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? EscalatedAt { get; set; }
    public string? Notes { get; set; }
    public List<Guid> RelatedReportIds { get; set; } = new();
}

public class CreateModerationCaseDto
{
    public int TargetUserId { get; set; }
    public string CaseType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<Guid> RelatedReportIds { get; set; } = new();
}

public class AssignModerationCaseDto
{
    public int AssignedToUserId { get; set; }
}