namespace ModerationService.Application.DTOs;

public class ReportDto
{
    public Guid Id { get; set; }
    public int ReporterId { get; set; }
    public int ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
}

public class CreateReportDto
{
    public int ReportedUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ReviewReportDto
{
    public string Action { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
}