using FluentValidation;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Validators;

public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.NotificationDto)
            .NotNull()
            .WithMessage("Notification data is required");

        RuleFor(x => x.NotificationDto.UserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .MaximumLength(255)
            .WithMessage("User ID must not exceed 255 characters");

        RuleFor(x => x.NotificationDto.Type)
            .NotEmpty()
            .WithMessage("Notification type is required")
            .Must(BeValidNotificationType)
            .WithMessage("Invalid notification type");

        RuleFor(x => x.NotificationDto.Priority)
            .NotEmpty()
            .WithMessage("Priority is required")
            .Must(BeValidPriority)
            .WithMessage("Invalid priority");

        RuleFor(x => x.NotificationDto.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.NotificationDto.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .MaximumLength(2000)
            .WithMessage("Message must not exceed 2000 characters");

        RuleFor(x => x.NotificationDto.DeliveryChannels)
            .NotEmpty()
            .WithMessage("At least one delivery channel is required")
            .Must(channels => channels.All(BeValidDeliveryChannel))
            .WithMessage("Invalid delivery channel specified");
    }

    private static bool BeValidNotificationType(string type)
    {
        var validTypes = new[] { "Info", "Warning", "Error", "Success", "Promotion", "Reminder", "Alert", "Message" };
        return validTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidPriority(string priority)
    {
        var validPriorities = new[] { "Low", "Normal", "High", "Urgent" };
        return validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidDeliveryChannel(string channel)
    {
        var validChannels = new[] { "Email", "SMS", "Push", "InApp" };
        return validChannels.Contains(channel, StringComparer.OrdinalIgnoreCase);
    }
}

public class CreateNotificationFromTemplateCommandValidator : AbstractValidator<CreateNotificationFromTemplateCommand>
{
    public CreateNotificationFromTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateName)
            .NotEmpty()
            .WithMessage("Template name is required")
            .MaximumLength(100)
            .WithMessage("Template name must not exceed 100 characters");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .MaximumLength(255)
            .WithMessage("User ID must not exceed 255 characters");

        RuleFor(x => x.Variables)
            .NotNull()
            .WithMessage("Variables dictionary is required");
    }
}

public class UpdateNotificationCommandValidator : AbstractValidator<UpdateNotificationCommand>
{
    public UpdateNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Notification ID is required");

        RuleFor(x => x.NotificationDto)
            .NotNull()
            .WithMessage("Update data is required");

        RuleFor(x => x.NotificationDto.Title)
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.NotificationDto.Title));

        RuleFor(x => x.NotificationDto.Message)
            .MaximumLength(2000)
            .WithMessage("Message must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.NotificationDto.Message));

        RuleFor(x => x.NotificationDto.Priority)
            .Must(BeValidPriority!)
            .WithMessage("Invalid priority")
            .When(x => !string.IsNullOrEmpty(x.NotificationDto.Priority));

        RuleFor(x => x.NotificationDto.DeliveryChannels)
            .Must(channels => channels!.All(BeValidDeliveryChannel))
            .WithMessage("Invalid delivery channel specified")
            .When(x => x.NotificationDto.DeliveryChannels != null && x.NotificationDto.DeliveryChannels.Any());
    }

    private static bool BeValidPriority(string priority)
    {
        var validPriorities = new[] { "Low", "Normal", "High", "Urgent" };
        return validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidDeliveryChannel(string channel)
    {
        var validChannels = new[] { "Email", "SMS", "Push", "InApp" };
        return validChannels.Contains(channel, StringComparer.OrdinalIgnoreCase);
    }
}

public class CreateNotificationTemplateCommandValidator : AbstractValidator<CreateNotificationTemplateCommand>
{
    public CreateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.TemplateDto)
            .NotNull()
            .WithMessage("Template data is required");

        RuleFor(x => x.TemplateDto.Name)
            .NotEmpty()
            .WithMessage("Template name is required")
            .MaximumLength(100)
            .WithMessage("Template name must not exceed 100 characters");

        RuleFor(x => x.TemplateDto.Type)
            .NotEmpty()
            .WithMessage("Template type is required")
            .Must(BeValidNotificationType)
            .WithMessage("Invalid notification type");

        RuleFor(x => x.TemplateDto.Subject)
            .NotEmpty()
            .WithMessage("Subject is required")
            .MaximumLength(200)
            .WithMessage("Subject must not exceed 200 characters");

        RuleFor(x => x.TemplateDto.Body)
            .NotEmpty()
            .WithMessage("Body is required")
            .MaximumLength(5000)
            .WithMessage("Body must not exceed 5000 characters");

        RuleFor(x => x.TemplateDto.SupportedChannels)
            .NotEmpty()
            .WithMessage("At least one supported channel is required")
            .Must(channels => channels.All(BeValidDeliveryChannel))
            .WithMessage("Invalid delivery channel specified");
    }

    private static bool BeValidNotificationType(string type)
    {
        var validTypes = new[] { "Match", "Message", "Like", "SuperLike", "Purchase", "Subscription", "Security", "System", "Promotion" };
        return validTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidDeliveryChannel(string channel)
    {
        var validChannels = new[] { "Email", "SMS", "Push", "InApp" };
        return validChannels.Contains(channel, StringComparer.OrdinalIgnoreCase);
    }
}