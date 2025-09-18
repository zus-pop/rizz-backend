namespace NotificationService.Infrastructure.Configurations;

// Helper class for JSON serialization
public class ChannelDto
{
    public string Type { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsEnabled { get; set; } = true;
}