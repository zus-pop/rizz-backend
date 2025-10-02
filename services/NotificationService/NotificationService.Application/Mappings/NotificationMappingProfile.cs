using AutoMapper;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;
using ValueObjects = NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        // Notification mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NotificationType.Value))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.Value))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Content.Title))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Content.Body))
            .ForMember(dest => dest.DeliveryChannels, opt => opt.MapFrom(src => src.Channels.Select(c => c.Type.ToString())))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt))
            .ForMember(dest => dest.ReadAt, opt => opt.MapFrom(src => src.ReadAt))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.Status.ToString() == "Read"))
            .ForMember(dest => dest.IsDelivered, opt => opt.MapFrom(src => src.Status.ToString() == "Delivered"))
            .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.FailureReason));

        CreateMap<CreateNotificationDto, Notification>()
            .ConstructUsing(src => CreateNotificationFromDto(src));

        // NotificationTemplate mappings
        CreateMap<NotificationTemplate, NotificationTemplateDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.NotificationType.Value))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.TitleTemplate))
            .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.BodyTemplate))
            .ForMember(dest => dest.SupportedChannels, opt => opt.MapFrom(src => src.DefaultChannels.Select(c => c.ToString())))
            .ForMember(dest => dest.Variables, opt => opt.MapFrom(src => src.Metadata.Keys.ToList()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<CreateNotificationTemplateDto, NotificationTemplate>()
            .ConstructUsing(src => CreateNotificationTemplateFromDto(src));
    }

    private static Notification CreateNotificationFromDto(CreateNotificationDto dto)
    {
        var notificationType = GetNotificationTypeFromString(dto.Type);
        var notificationPriority = GetNotificationPriorityFromString(dto.Priority);
        var notificationContent = NotificationContent.Create(dto.Title, dto.Message, dto.Variables);
        
        var notification = new Notification(
            dto.UserId,
            notificationType,
            notificationContent,
            notificationPriority
        );

        // Add delivery channels
        foreach (var channelStr in dto.DeliveryChannels)
        {
            var channel = GetDeliveryChannelFromString(channelStr);
            notification.AddDeliveryChannel(channel);
        }

        return notification;
    }

    private static NotificationTemplate CreateNotificationTemplateFromDto(CreateNotificationTemplateDto dto)
    {
        var notificationType = GetNotificationTypeFromString(dto.Type);
        var defaultChannels = dto.SupportedChannels.Select(GetDeliveryChannelTypeFromString).ToList();

        return new NotificationTemplate(
            dto.Name,
            notificationType,
            dto.Subject,
            dto.Body,
            null, // Use default priority
            defaultChannels,
            dto.Variables.ToDictionary(v => v, v => "")
        );
    }

    private static NotificationType GetNotificationTypeFromString(string type)
    {
        return NotificationType.Create(type);
    }

    private static NotificationPriority GetNotificationPriorityFromString(string priority)
    {
        return NotificationPriority.Create(priority);
    }

    private static DeliveryChannel GetDeliveryChannelFromString(string channel)
    {
        var channelType = GetDeliveryChannelTypeFromString(channel);
        return new DeliveryChannel(channelType);
    }

    private static DeliveryChannelType GetDeliveryChannelTypeFromString(string channel)
    {
        return channel.ToLower() switch
        {
            "email" => DeliveryChannelType.Email,
            "sms" => DeliveryChannelType.SMS,
            "push" => DeliveryChannelType.Push,
            "inapp" => DeliveryChannelType.InApp,
            _ => DeliveryChannelType.InApp
        };
    }
}