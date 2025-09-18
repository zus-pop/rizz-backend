using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Services;

public interface INotificationTemplateService
{
    Task<NotificationTemplate?> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
    Task<Notification> CreateFromTemplateAsync(string templateName, string userId, Dictionary<string, string> variables, CancellationToken cancellationToken = default);
    Task<bool> ValidateTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
}

public class NotificationTemplateService : INotificationTemplateService
{
    public async Task<NotificationTemplate?> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        // This will be implemented in the infrastructure layer
        throw new NotImplementedException("Template retrieval will be implemented in infrastructure layer");
    }

    public async Task<Notification> CreateFromTemplateAsync(string templateName, string userId, Dictionary<string, string> variables, CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync(templateName, cancellationToken);
        if (template == null)
            throw new InvalidOperationException($"Template '{templateName}' not found");

        if (!template.IsActive)
            throw new InvalidOperationException($"Template '{templateName}' is not active");

        var content = template.RenderContent(variables);
        
        var notification = new Notification(
            userId,
            template.NotificationType,
            content,
            template.DefaultPriority);

        // Add default delivery channels
        foreach (var channelType in template.DefaultChannels)
        {
            var channel = channelType switch
            {
                DeliveryChannelType.InApp => DeliveryChannel.InApp(),
                DeliveryChannelType.Push => DeliveryChannel.Push(),
                _ => DeliveryChannel.InApp() // Default fallback
            };
            notification.AddDeliveryChannel(channel);
        }

        return notification;
    }

    public Task<bool> ValidateTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        if (template == null) return Task.FromResult(false);

        try
        {
            // Test template with sample variables
            var sampleVariables = template.GetTemplateVariables()
                .ToDictionary(var => var, var => "sample_value");
            
            var content = template.RenderContent(sampleVariables);
            
            // Basic validation - ensure content is created successfully
            return Task.FromResult(!string.IsNullOrWhiteSpace(content.Title) && 
                                 !string.IsNullOrWhiteSpace(content.Body));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}