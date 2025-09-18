using AutoMapper;
using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Queries;

namespace NotificationService.Application.Handlers;

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificationDto?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken);
        return notification == null ? null : _mapper.Map<NotificationDto>(notification);
    }
}

public class GetNotificationsByUserIdQueryHandler : IRequestHandler<GetNotificationsByUserIdQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}

public class GetUnreadNotificationsByUserIdQueryHandler : IRequestHandler<GetUnreadNotificationsByUserIdQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUnreadNotificationsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetUnreadNotificationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Notifications.GetUnreadByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}

public class GetPendingNotificationsQueryHandler : IRequestHandler<GetPendingNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPendingNotificationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetPendingNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Notifications.GetPendingNotificationsAsync(cancellationToken);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }
}

public class GetNotificationStatsQueryHandler : IRequestHandler<GetNotificationStatsQuery, NotificationStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationStatsDto> Handle(GetNotificationStatsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Notifications.GetStatsAsync(cancellationToken);
    }
}

public class GetUserNotificationStatsQueryHandler : IRequestHandler<GetUserNotificationStatsQuery, NotificationStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserNotificationStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationStatsDto> Handle(GetUserNotificationStatsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Notifications.GetUserStatsAsync(request.UserId, cancellationToken);
    }
}

public class GetPagedNotificationsQueryHandler : IRequestHandler<GetPagedNotificationsQuery, PagedResult<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPagedNotificationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<NotificationDto>> Handle(GetPagedNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Notifications.GetPagedAsync(
            request.Page, 
            request.PageSize, 
            request.Filter, 
            cancellationToken);

        var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        
        // Note: This is a simplified implementation. In a real scenario, you'd need to get the total count
        // from the repository to calculate pagination properly
        var totalCount = notificationDtos.Count(); // This should come from repository
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PagedResult<NotificationDto>(
            notificationDtos,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages);
    }
}

public class GetNotificationTemplateByIdQueryHandler : IRequestHandler<GetNotificationTemplateByIdQuery, NotificationTemplateDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationTemplateByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificationTemplateDto?> Handle(GetNotificationTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.NotificationTemplates.GetByIdAsync(request.Id, cancellationToken);
        return template == null ? null : _mapper.Map<NotificationTemplateDto>(template);
    }
}

public class GetNotificationTemplateByNameQueryHandler : IRequestHandler<GetNotificationTemplateByNameQuery, NotificationTemplateDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationTemplateByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificationTemplateDto?> Handle(GetNotificationTemplateByNameQuery request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.NotificationTemplates.GetByNameAsync(request.Name, cancellationToken);
        return template == null ? null : _mapper.Map<NotificationTemplateDto>(template);
    }
}

public class GetActiveNotificationTemplatesQueryHandler : IRequestHandler<GetActiveNotificationTemplatesQuery, IEnumerable<NotificationTemplateDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveNotificationTemplatesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationTemplateDto>> Handle(GetActiveNotificationTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.NotificationTemplates.GetActiveTemplatesAsync(cancellationToken);
        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }
}

public class GetNotificationTemplatesByTypeQueryHandler : IRequestHandler<GetNotificationTemplatesByTypeQuery, IEnumerable<NotificationTemplateDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNotificationTemplatesByTypeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationTemplateDto>> Handle(GetNotificationTemplatesByTypeQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.NotificationTemplates.GetByTypeAsync(request.Type, cancellationToken);
        return _mapper.Map<IEnumerable<NotificationTemplateDto>>(templates);
    }
}