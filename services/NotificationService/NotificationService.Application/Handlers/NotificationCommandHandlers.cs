using AutoMapper;
using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Events;
using NotificationService.Domain.Services;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Handlers;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public CreateNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<NotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = _mapper.Map<Notification>(request.NotificationDto);
        
        var createdNotification = await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var notificationCreatedEvent = new NotificationCreatedEvent(
            createdNotification.Id,
            createdNotification.UserId,
            createdNotification.NotificationType.Value,
            createdNotification.Priority.Value);

        await _eventPublisher.PublishAsync(notificationCreatedEvent, cancellationToken);

        return _mapper.Map<NotificationDto>(createdNotification);
    }
}

public class CreateNotificationFromTemplateCommandHandler : IRequestHandler<CreateNotificationFromTemplateCommand, NotificationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationTemplateService _templateService;
    private readonly IEventPublisher _eventPublisher;

    public CreateNotificationFromTemplateCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationTemplateService templateService,
        IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _templateService = templateService;
        _eventPublisher = eventPublisher;
    }

    public async Task<NotificationDto> Handle(CreateNotificationFromTemplateCommand request, CancellationToken cancellationToken)
    {
        var notification = await _templateService.CreateFromTemplateAsync(
            request.TemplateName,
            request.UserId,
            request.Variables,
            cancellationToken);

        var createdNotification = await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var notificationCreatedEvent = new NotificationCreatedEvent(
            createdNotification.Id,
            createdNotification.UserId,
            createdNotification.NotificationType.Value,
            createdNotification.Priority.Value);

        await _eventPublisher.PublishAsync(notificationCreatedEvent, cancellationToken);

        return _mapper.Map<NotificationDto>(createdNotification);
    }
}

public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, NotificationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateNotificationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificationDto> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken);
        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with ID {request.Id} not found");
        }

        // Update notification properties
        if (!string.IsNullOrEmpty(request.NotificationDto.Title) || !string.IsNullOrEmpty(request.NotificationDto.Message))
        {
            var newContent = NotificationContent.Create(
                request.NotificationDto.Title ?? notification.Content.Title,
                request.NotificationDto.Message ?? notification.Content.Body);
            // Note: There's no UpdateContent method in the current domain model
            // This would need to be added to the Notification entity
        }

        if (!string.IsNullOrEmpty(request.NotificationDto.Priority))
        {
            // Note: There's no UpdatePriority method in the current domain model
            // This would need to be added to the Notification entity
        }

        if (request.NotificationDto.DeliveryChannels != null && request.NotificationDto.DeliveryChannels.Any())
        {
            // Clear existing channels and add new ones
            // Note: This would need proper methods in the domain model
        }

        await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationDto>(notification);
    }
}

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Notifications.ExistsAsync(request.Id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        await _unitOfWork.Notifications.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;

    public MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken);
        if (notification == null || notification.UserId != request.UserId)
        {
            return false;
        }

        notification.MarkAsRead();

        await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var notificationReadEvent = new NotificationReadEvent(notification.Id, notification.UserId);
        await _eventPublisher.PublishAsync(notificationReadEvent, cancellationToken);

        return true;
    }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationDeliveryInfrastructure _deliveryService;
    private readonly IEventPublisher _eventPublisher;

    public SendNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationDeliveryInfrastructure deliveryService,
        IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _deliveryService = deliveryService;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken);
        if (notification == null)
        {
            return false;
        }

        try
        {
            var delivered = await _deliveryService.DeliverAsync(notification, cancellationToken);
            
            if (delivered)
            {
                notification.MarkAsDelivered();
                
                // Publish success event
                var notificationSentEvent = new NotificationSentEvent(
                    notification.Id,
                    notification.UserId,
                    notification.Channels.Select(c => c.Type.ToString()).ToList());
                
                await _eventPublisher.PublishAsync(notificationSentEvent, cancellationToken);
            }
            else
            {
                notification.MarkAsFailed("Delivery failed");
                
                // Publish failure event
                var notificationFailedEvent = new NotificationFailedEvent(
                    notification.Id,
                    notification.UserId,
                    "Delivery failed",
                    notification.RetryCount);
                
                await _eventPublisher.PublishAsync(notificationFailedEvent, cancellationToken);
            }

            await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return delivered;
        }
        catch (Exception ex)
        {
            notification.MarkAsFailed(ex.Message);
            
            // Publish failure event
            var notificationFailedEvent = new NotificationFailedEvent(
                notification.Id,
                notification.UserId,
                ex.Message,
                notification.RetryCount);
            
            await _eventPublisher.PublishAsync(notificationFailedEvent, cancellationToken);
            
            await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return false;
        }
    }
}