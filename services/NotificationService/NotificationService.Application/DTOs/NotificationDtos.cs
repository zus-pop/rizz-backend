namespace NotificationService.Application.DTOs;

public record NotificationDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public List<string> DeliveryChannels { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? SentAt { get; init; }
    public DateTime? ReadAt { get; init; }
    public bool IsRead { get; init; }
    public bool IsDelivered { get; init; }
    public string? ErrorMessage { get; init; }
}

public record CreateNotificationDto
{
    public string UserId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public List<string> DeliveryChannels { get; init; } = new();
    public Dictionary<string, string> Variables { get; init; } = new();
}

public record UpdateNotificationDto
{
    public string? Title { get; init; }
    public string? Message { get; init; }
    public string? Priority { get; init; }
    public List<string>? DeliveryChannels { get; init; }
}

public record NotificationTemplateDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public List<string> SupportedChannels { get; init; } = new();
    public List<string> Variables { get; init; } = new();
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateNotificationTemplateDto
{
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public List<string> SupportedChannels { get; init; } = new();
    public List<string> Variables { get; init; } = new();
}

public record UpdateNotificationTemplateDto
{
    public string? Subject { get; init; }
    public string? Body { get; init; }
    public List<string>? SupportedChannels { get; init; }
    public List<string>? Variables { get; init; }
    public bool? IsActive { get; init; }
}

public record NotificationStatsDto
{
    public int TotalNotifications { get; init; }
    public int PendingNotifications { get; init; }
    public int DeliveredNotifications { get; init; }
    public int FailedNotifications { get; init; }
    public int ReadNotifications { get; init; }
    public Dictionary<string, int> NotificationsByType { get; init; } = new();
    public Dictionary<string, int> NotificationsByChannel { get; init; } = new();
}