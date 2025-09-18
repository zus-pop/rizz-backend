using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Queries;

public record GetNotificationByIdQuery(Guid Id) : IRequest<NotificationDto?>;

public record GetNotificationsByUserIdQuery(string UserId) : IRequest<IEnumerable<NotificationDto>>;

public record GetUnreadNotificationsByUserIdQuery(string UserId) : IRequest<IEnumerable<NotificationDto>>;

public record GetPendingNotificationsQuery : IRequest<IEnumerable<NotificationDto>>;

public record GetNotificationStatsQuery : IRequest<NotificationStatsDto>;

public record GetUserNotificationStatsQuery(string UserId) : IRequest<NotificationStatsDto>;

public record GetPagedNotificationsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Filter = null) : IRequest<PagedResult<NotificationDto>>;

public record GetNotificationTemplateByIdQuery(Guid Id) : IRequest<NotificationTemplateDto?>;

public record GetNotificationTemplateByNameQuery(string Name) : IRequest<NotificationTemplateDto?>;

public record GetActiveNotificationTemplatesQuery : IRequest<IEnumerable<NotificationTemplateDto>>;

public record GetNotificationTemplatesByTypeQuery(string Type) : IRequest<IEnumerable<NotificationTemplateDto>>;

public record GetPagedNotificationTemplatesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Filter = null) : IRequest<PagedResult<NotificationTemplateDto>>;

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}