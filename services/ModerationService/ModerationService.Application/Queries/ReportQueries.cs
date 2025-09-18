using MediatR;
using ModerationService.Application.DTOs;

namespace ModerationService.Application.Queries;

public record GetReportByIdQuery(Guid ReportId) : IRequest<ReportDto?>;

public record GetReportsForUserQuery(int ReportedUserId) : IRequest<List<ReportDto>>;

public record GetReportsByStatusQuery(string Status) : IRequest<List<ReportDto>>;

public record GetPendingReportsQuery() : IRequest<List<ReportDto>>;

public record GetModerationCaseByIdQuery(Guid CaseId) : IRequest<ModerationCaseDto?>;

public record GetCasesForUserQuery(int TargetUserId) : IRequest<List<ModerationCaseDto>>;

public record GetAssignedCasesQuery(int AssignedToUserId) : IRequest<List<ModerationCaseDto>>;