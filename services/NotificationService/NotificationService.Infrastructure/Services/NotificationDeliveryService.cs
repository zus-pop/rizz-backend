using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Infrastructure.Services;

public class NotificationDeliveryService : INotificationDeliveryInfrastructure
{
    private readonly ILogger<NotificationDeliveryService> _logger;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IPushNotificationService _pushService;

    public NotificationDeliveryService(
        ILogger<NotificationDeliveryService> logger,
        IEmailService emailService,
        ISmsService smsService,
        IPushNotificationService pushService)
    {
        _logger = logger;
        _emailService = emailService;
        _smsService = smsService;
        _pushService = pushService;
    }

    public async Task<bool> DeliverAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting delivery for notification {NotificationId}", notification.Id);

        var deliveryResults = new List<bool>();

        foreach (var channel in notification.Channels)
        {
            var result = await DeliverToChannelAsync(notification, channel.Type.ToString(), cancellationToken);
            deliveryResults.Add(result);
        }

        // Return true if at least one channel succeeded
        var overallSuccess = deliveryResults.Any(r => r);
        
        _logger.LogInformation("Delivery completed for notification {NotificationId}. Success: {Success}", 
            notification.Id, overallSuccess);

        return overallSuccess;
    }

    public async Task<bool> DeliverToChannelAsync(Notification notification, string channel, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Delivering notification {NotificationId} via {Channel}", notification.Id, channel);

            return channel.ToLower() switch
            {
                "email" => await _emailService.SendEmailAsync(
                    notification.UserId,
                    notification.Content.Title,
                    notification.Content.Body,
                    cancellationToken),
                
                "sms" => await _smsService.SendSmsAsync(
                    notification.UserId,
                    notification.Content.Body,
                    cancellationToken),
                
                "push" => await _pushService.SendPushNotificationAsync(
                    notification.UserId,
                    notification.Content.Title,
                    notification.Content.Body,
                    notification.Content.Data,
                    cancellationToken),
                
                "inapp" => await Task.FromResult(true), // In-app notifications are stored in DB
                
                _ => throw new NotSupportedException($"Delivery channel '{channel}' is not supported")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deliver notification {NotificationId} via {Channel}", 
                notification.Id, channel);
            return false;
        }
    }

    public async Task<bool> IsChannelAvailableAsync(string channel, CancellationToken cancellationToken = default)
    {
        return channel.ToLower() switch
        {
            "email" => await _emailService.IsAvailableAsync(cancellationToken),
            "sms" => await _smsService.IsAvailableAsync(cancellationToken),
            "push" => await _pushService.IsAvailableAsync(cancellationToken),
            "inapp" => true, // Always available
            _ => false
        };
    }

    public async Task<IEnumerable<string>> GetAvailableChannelsAsync(CancellationToken cancellationToken = default)
    {
        var channels = new List<string>();

        if (await IsChannelAvailableAsync("email", cancellationToken))
            channels.Add("Email");

        if (await IsChannelAvailableAsync("sms", cancellationToken))
            channels.Add("SMS");

        if (await IsChannelAvailableAsync("push", cancellationToken))
            channels.Add("Push");

        channels.Add("InApp"); // Always available

        return channels;
    }
}

// Channel-specific service interfaces and implementations
public interface IEmailService
{
    Task<bool> SendEmailAsync(string userId, string subject, string body, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public interface ISmsService
{
    Task<bool> SendSmsAsync(string userId, string message, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public interface IPushNotificationService
{
    Task<bool> SendPushNotificationAsync(string userId, string title, string body, Dictionary<string, string> data, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string userId, string subject, string body, CancellationToken cancellationToken = default)
    {
        // Implementation would integrate with email service (SendGrid, AWS SES, etc.)
        _logger.LogInformation("Sending email to user {UserId} with subject: {Subject}", userId, subject);
        
        // Simulate async email sending
        await Task.Delay(100, cancellationToken);
        
        // For now, return true (mock implementation)
        return true;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        // Check if email service is available
        await Task.CompletedTask;
        return true;
    }
}

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendSmsAsync(string userId, string message, CancellationToken cancellationToken = default)
    {
        // Implementation would integrate with SMS service (Twilio, AWS SNS, etc.)
        _logger.LogInformation("Sending SMS to user {UserId} with message: {Message}", userId, message);
        
        // Simulate async SMS sending
        await Task.Delay(150, cancellationToken);
        
        // For now, return true (mock implementation)
        return true;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        // Check if SMS service is available
        await Task.CompletedTask;
        return true;
    }
}

public class PushNotificationService : IPushNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PushNotificationService> _logger;
    private readonly IConfiguration _configuration;

    public PushNotificationService(
        HttpClient httpClient,
        ILogger<PushNotificationService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        
        // Configure HttpClient for PushService
        var pushServiceUrl = _configuration["Services:PushService:BaseUrl"] ?? "http://pushservice:8080";
        _httpClient.BaseAddress = new Uri(pushServiceUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<bool> SendPushNotificationAsync(string userId, string title, string body, Dictionary<string, string> data, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!int.TryParse(userId, out int parsedUserId))
            {
                _logger.LogWarning("Invalid userId '{UserId}' provided to SendPushNotificationAsync. Must be a valid integer.", userId);
                return false;
            }

            var requestPayload = new
            {
                UserId = parsedUserId,
                Notification = new
                {
                    Title = title,
                    Body = body,
                    Data = data ?? new Dictionary<string, string>()
                }
            };

            var json = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/notifications/send-to-user/" + userId, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully sent push notification to user {UserId}", userId);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Failed to send push notification to user {UserId}. Status: {StatusCode}, Error: {Error}", 
                    userId, response.StatusCode, errorContent);
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error when sending push notification to user {UserId}", userId);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Timeout when sending push notification to user {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when sending push notification to user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "PushService health check failed");
            return false;
        }
    }
}