using MediatR;
using ModerationService.Application.DTOs;

namespace ModerationService.Application.Commands;

public record CreateReportCommand(
    int ReporterId,
    int ReportedUserId,
    string Reason,
    string? Description
) : IRequest<ReportDto>;

public record ReviewReportCommand(
    Guid ReportId,
    int ReviewedByUserId,
    string Action,
    string? ReviewNotes
) : IRequest<bool>;

public record EscalateReportCommand(
    Guid ReportId,
    int EscalatedByUserId
) : IRequest<bool>;