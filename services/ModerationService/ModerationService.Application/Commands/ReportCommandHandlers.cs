using MediatR;
using AutoMapper;
using ModerationService.Application.DTOs;
using ModerationService.Application.Interfaces;
using ModerationService.Domain.Entities;
using ModerationService.Domain.ValueObjects;
using ModerationService.Domain.Events;
using ModerationService.Domain.Services;

namespace ModerationService.Application.Commands;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, ReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ModerationPolicyService _policyService;

    public CreateReportCommandHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IMediator mediator,
        ModerationPolicyService policyService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
        _policyService = policyService;
    }

    public async Task<ReportDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var reportReason = ReportReason.Create(request.Reason);
        
        var report = new Report(
            UserId.Create(request.ReporterId),
            UserId.Create(request.ReportedUserId),
            reportReason,
            request.Description
        );

        await _unitOfWork.Reports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Check if auto-moderation is needed
        var reportCount = await _unitOfWork.Reports.GetActiveReportsCountForUserAsync(
            request.ReportedUserId, cancellationToken);

        var userReports = await _unitOfWork.Reports.GetReportsForUserAsync(
            request.ReportedUserId, cancellationToken);

        if (_policyService.ShouldCreateModerationCase(report, userReports))
        {
            // Auto-resolve the report or create a moderation case
            var action = _policyService.DetermineAction(report, userReports);
            report.StartReview(UserId.Create(0)); // System user
            report.Resolve(action, "Auto-moderated");
            
            await _unitOfWork.Reports.UpdateAsync(report, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Publish domain event
        var domainEvent = new UserReportedEvent(
            report.Id,
            request.ReporterId,
            request.ReportedUserId,
            request.Reason,
            request.Description,
            report.CreatedAt
        );

        await _mediator.Publish(domainEvent, cancellationToken);

        return _mapper.Map<ReportDto>(report);
    }
}

public class ReviewReportCommandHandler : IRequestHandler<ReviewReportCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ReviewReportCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<bool> Handle(ReviewReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(request.ReportId, cancellationToken);
        if (report == null)
        {
            return false;
        }

        var reviewedBy = UserId.Create(request.ReviewedByUserId);

        if (report.Status == ReportStatus.Pending)
        {
            report.StartReview(reviewedBy);
        }

        switch (request.Action.ToLower())
        {
            case "dismiss":
                report.Dismiss(request.ReviewNotes);
                break;
            case "resolve":
                var action = GetModerationActionFromString(request.Action);
                report.Resolve(action, request.ReviewNotes);
                break;
            case "escalate":
                report.Escalate();
                break;
            default:
                return false;
        }

        await _unitOfWork.Reports.UpdateAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event for resolved reports
        if (request.Action.ToLower() == "resolve")
        {
            var domainEvent = new ReportResolvedEvent(
                report.Id,
                report.ReportedUserId.Value,
                request.Action,
                request.ReviewedByUserId,
                DateTime.UtcNow
            );

            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return true;
    }

    private static ModerationAction GetModerationActionFromString(string action)
    {
        return action.ToLower() switch
        {
            "warning" => ModerationAction.Warning,
            "suspend" => ModerationAction.AccountSuspension,
            "ban" => ModerationAction.PermanentBan,
            "restrict" => ModerationAction.TemporaryRestriction,
            "remove" => ModerationAction.ContentRemoval,
            _ => ModerationAction.NoAction
        };
    }
}

public class EscalateReportCommandHandler : IRequestHandler<EscalateReportCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public EscalateReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(EscalateReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(request.ReportId, cancellationToken);
        if (report == null)
        {
            return false;
        }

        report.Escalate();
        await _unitOfWork.Reports.UpdateAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}