using MediatR;
using AutoMapper;
using ModerationService.Application.DTOs;
using ModerationService.Application.Interfaces;

namespace ModerationService.Application.Queries;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReportDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Reports.GetByIdAsync(request.ReportId, cancellationToken);
        return report != null ? _mapper.Map<ReportDto>(report) : null;
    }
}

public class GetReportsForUserQueryHandler : IRequestHandler<GetReportsForUserQuery, List<ReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReportsForUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ReportDto>> Handle(GetReportsForUserQuery request, CancellationToken cancellationToken)
    {
        var reports = await _unitOfWork.Reports.GetReportsForUserAsync(request.ReportedUserId, cancellationToken);
        return _mapper.Map<List<ReportDto>>(reports);
    }
}

public class GetReportsByStatusQueryHandler : IRequestHandler<GetReportsByStatusQuery, List<ReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReportsByStatusQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ReportDto>> Handle(GetReportsByStatusQuery request, CancellationToken cancellationToken)
    {
        var reports = await _unitOfWork.Reports.GetReportsByStatusAsync(request.Status, cancellationToken);
        return _mapper.Map<List<ReportDto>>(reports);
    }
}

public class GetPendingReportsQueryHandler : IRequestHandler<GetPendingReportsQuery, List<ReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPendingReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ReportDto>> Handle(GetPendingReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _unitOfWork.Reports.GetPendingReportsAsync(cancellationToken);
        return _mapper.Map<List<ReportDto>>(reports);
    }
}

public class GetModerationCaseByIdQueryHandler : IRequestHandler<GetModerationCaseByIdQuery, ModerationCaseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetModerationCaseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ModerationCaseDto?> Handle(GetModerationCaseByIdQuery request, CancellationToken cancellationToken)
    {
        var moderationCase = await _unitOfWork.ModerationCases.GetByIdAsync(request.CaseId, cancellationToken);
        return moderationCase != null ? _mapper.Map<ModerationCaseDto>(moderationCase) : null;
    }
}

public class GetCasesForUserQueryHandler : IRequestHandler<GetCasesForUserQuery, List<ModerationCaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCasesForUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ModerationCaseDto>> Handle(GetCasesForUserQuery request, CancellationToken cancellationToken)
    {
        var cases = await _unitOfWork.ModerationCases.GetCasesForUserAsync(request.TargetUserId, cancellationToken);
        return _mapper.Map<List<ModerationCaseDto>>(cases);
    }
}

public class GetAssignedCasesQueryHandler : IRequestHandler<GetAssignedCasesQuery, List<ModerationCaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssignedCasesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ModerationCaseDto>> Handle(GetAssignedCasesQuery request, CancellationToken cancellationToken)
    {
        var cases = await _unitOfWork.ModerationCases.GetAssignedCasesAsync(request.AssignedToUserId, cancellationToken);
        return _mapper.Map<List<ModerationCaseDto>>(cases);
    }
}