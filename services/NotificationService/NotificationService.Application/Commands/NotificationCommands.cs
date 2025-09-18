using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands;

public record CreateNotificationCommand(CreateNotificationDto NotificationDto) : IRequest<NotificationDto>;

public record CreateNotificationFromTemplateCommand(
    string TemplateName,
    string UserId,
    Dictionary<string, string> Variables) : IRequest<NotificationDto>;

public record UpdateNotificationCommand(Guid Id, UpdateNotificationDto NotificationDto) : IRequest<NotificationDto>;

public record DeleteNotificationCommand(Guid Id) : IRequest<bool>;

public record MarkNotificationAsReadCommand(Guid Id, string UserId) : IRequest<bool>;

public record MarkAllNotificationsAsReadCommand(string UserId) : IRequest<int>;

public record SendNotificationCommand(Guid Id) : IRequest<bool>;

public record SendPendingNotificationsCommand : IRequest<int>;

public record CreateNotificationTemplateCommand(CreateNotificationTemplateDto TemplateDto) : IRequest<NotificationTemplateDto>;

public record UpdateNotificationTemplateCommand(Guid Id, UpdateNotificationTemplateDto TemplateDto) : IRequest<NotificationTemplateDto>;

public record DeleteNotificationTemplateCommand(Guid Id) : IRequest<bool>;

public record ActivateNotificationTemplateCommand(Guid Id) : IRequest<bool>;

public record DeactivateNotificationTemplateCommand(Guid Id) : IRequest<bool>;