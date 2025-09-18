using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Entities;

public class NotificationTemplate
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public string TitleTemplate { get; private set; }
    public string BodyTemplate { get; private set; }
    public NotificationPriority DefaultPriority { get; private set; }
    public List<DeliveryChannelType> DefaultChannels { get; private set; } = new();
    public bool IsActive { get; private set; } = true;
    public Dictionary<string, string> Metadata { get; private set; } = new();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // For EF Core
    private NotificationTemplate()
    {
        Name = string.Empty;
        NotificationType = NotificationType.System;
        TitleTemplate = string.Empty;
        BodyTemplate = string.Empty;
        DefaultPriority = NotificationPriority.Normal;
    }

    public NotificationTemplate(
        string name,
        NotificationType type,
        string titleTemplate,
        string bodyTemplate,
        NotificationPriority? defaultPriority = null,
        List<DeliveryChannelType>? defaultChannels = null,
        Dictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template name cannot be null or empty", nameof(name));

        if (string.IsNullOrWhiteSpace(titleTemplate))
            throw new ArgumentException("Title template cannot be null or empty", nameof(titleTemplate));

        if (string.IsNullOrWhiteSpace(bodyTemplate))
            throw new ArgumentException("Body template cannot be null or empty", nameof(bodyTemplate));

        Name = name.Trim();
        NotificationType = type ?? throw new ArgumentNullException(nameof(type));
        TitleTemplate = titleTemplate.Trim();
        BodyTemplate = bodyTemplate.Trim();
        DefaultPriority = defaultPriority ?? NotificationPriority.Normal;
        DefaultChannels = defaultChannels ?? new List<DeliveryChannelType> { DeliveryChannelType.InApp };
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    public NotificationContent RenderContent(Dictionary<string, string> variables)
    {
        var title = ReplaceVariables(TitleTemplate, variables);
        var body = ReplaceVariables(BodyTemplate, variables);

        return NotificationContent.Create(title, body);
    }

    public void UpdateTemplate(string titleTemplate, string bodyTemplate)
    {
        if (string.IsNullOrWhiteSpace(titleTemplate))
            throw new ArgumentException("Title template cannot be null or empty", nameof(titleTemplate));

        if (string.IsNullOrWhiteSpace(bodyTemplate))
            throw new ArgumentException("Body template cannot be null or empty", nameof(bodyTemplate));

        TitleTemplate = titleTemplate.Trim();
        BodyTemplate = bodyTemplate.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(NotificationPriority priority)
    {
        DefaultPriority = priority ?? throw new ArgumentNullException(nameof(priority));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDefaultChannel(DeliveryChannelType channelType)
    {
        if (!DefaultChannels.Contains(channelType))
        {
            DefaultChannels.Add(channelType);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveDefaultChannel(DeliveryChannelType channelType)
    {
        if (DefaultChannels.Remove(channelType))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));

        Metadata[key] = value ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string ReplaceVariables(string template, Dictionary<string, string> variables)
    {
        var result = template;
        foreach (var variable in variables)
        {
            result = result.Replace($"{{{variable.Key}}}", variable.Value);
        }
        return result;
    }

    public List<string> GetTemplateVariables()
    {
        var variables = new List<string>();
        var titleVars = ExtractVariables(TitleTemplate);
        var bodyVars = ExtractVariables(BodyTemplate);
        
        variables.AddRange(titleVars);
        variables.AddRange(bodyVars);
        
        return variables.Distinct().ToList();
    }

    private static List<string> ExtractVariables(string template)
    {
        var variables = new List<string>();
        var startIndex = 0;
        
        while (true)
        {
            var openBrace = template.IndexOf('{', startIndex);
            if (openBrace == -1) break;
            
            var closeBrace = template.IndexOf('}', openBrace);
            if (closeBrace == -1) break;
            
            var variableName = template.Substring(openBrace + 1, closeBrace - openBrace - 1);
            if (!string.IsNullOrWhiteSpace(variableName))
            {
                variables.Add(variableName);
            }
            
            startIndex = closeBrace + 1;
        }
        
        return variables;
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}