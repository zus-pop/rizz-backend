using MediatR;
using Microsoft.Extensions.Logging;
using PushService.Application.Commands;
using PushService.Application.DTOs;
using PushService.Application.Queries;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace PushService.Infrastructure.Services;

public interface IEnhancedPushNotificationService
{
    Task<PushNotificationResult> SendToUserAsync(int userId, PushNotificationDto notification, CancellationToken cancellationToken = default);
    Task<PushNotificationResult> SendToTokenAsync(string token, PushNotificationDto notification, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

public class EnhancedPushNotificationService : IEnhancedPushNotificationService
{
    private readonly IMediator _mediator;
    private readonly ILogger<EnhancedPushNotificationService> _logger;
    private readonly FirebaseMessaging _firebaseMessaging;

    public EnhancedPushNotificationService(
        IMediator mediator,
        ILogger<EnhancedPushNotificationService> logger)
    {
        _mediator = mediator;
        _logger = logger;
        
        // Initialize Firebase if not already done
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID")
            });
        }
        
        _firebaseMessaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task<PushNotificationResult> SendToUserAsync(int userId, PushNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all active device tokens for the user
            var tokensQuery = new GetDeviceTokensByUserIdQuery { UserId = userId, ActiveOnly = true };
            var deviceTokens = await _mediator.Send(tokensQuery, cancellationToken);

            if (!deviceTokens.Any())
            {
                _logger.LogWarning("No active device tokens found for user {UserId}", userId);
                return PushNotificationResult.Failed("No active device tokens found");
            }

            var results = new List<SingleTokenResult>();
            var tasks = deviceTokens.Select(async token =>
            {
                var result = await SendToTokenAsync(token.Token, notification, cancellationToken);
                return new SingleTokenResult
                {
                    Token = token.Token,
                    DeviceTokenId = token.Id,
                    Success = result.Success,
                    Error = result.Error
                };
            });

            var tokenResults = await Task.WhenAll(tasks);
            results.AddRange(tokenResults);

            // Handle failed tokens
            var failedTokens = results.Where(r => !r.Success).ToList();
            foreach (var failedToken in failedTokens)
            {
                await HandleFailedToken(failedToken.DeviceTokenId, failedToken.Error, cancellationToken);
            }

            var successCount = results.Count(r => r.Success);
            var totalCount = results.Count;

            _logger.LogInformation("Sent push notification to user {UserId}: {SuccessCount}/{TotalCount} successful", 
                userId, successCount, totalCount);

            return new PushNotificationResult
            {
                Success = successCount > 0,
                Message = $"Sent to {successCount}/{totalCount} devices",
                TokenResults = results
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {UserId}", userId);
            return PushNotificationResult.Failed($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<PushNotificationResult> SendToTokenAsync(string token, PushNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl
                },
                Data = notification.Data ?? new Dictionary<string, string>(),
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "default",
                        DefaultSound = true,
                        DefaultVibrateTimings = true
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Alert = new ApsAlert
                        {
                            Title = notification.Title,
                            Body = notification.Body
                        },
                        Badge = 1,
                        Sound = "default"
                    }
                }
            };

            var response = await _firebaseMessaging.SendAsync(message, cancellationToken);
            
            _logger.LogDebug("Successfully sent message to token {Token}. Response: {Response}", 
                MaskToken(token), response);

            return PushNotificationResult.Success();
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogWarning("Firebase messaging error for token {Token}: {ErrorCode} - {Message}", 
                MaskToken(token), ex.ErrorCode, ex.Message);

            // Handle specific Firebase errors
            return ex.ErrorCode switch
            {
                ErrorCode.Unregistered or ErrorCode.InvalidRegistration => 
                    PushNotificationResult.Failed("Invalid or unregistered token", isTokenInvalid: true),
                ErrorCode.NotFound => 
                    PushNotificationResult.Failed("Token not found", isTokenInvalid: true),
                ErrorCode.MessageRateExceeded => 
                    PushNotificationResult.Failed("Rate limit exceeded", isRetryable: true),
                ErrorCode.Unavailable => 
                    PushNotificationResult.Failed("Service unavailable", isRetryable: true),
                _ => PushNotificationResult.Failed($"Firebase error: {ex.Message}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending push notification to token {Token}", MaskToken(token));
            return PushNotificationResult.Failed($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            // Send a dry-run message to validate the token
            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = "Validation",
                    Body = "Token validation"
                }
            };

            await _firebaseMessaging.SendAsync(message, dryRun: true, cancellationToken);
            return true;
        }
        catch (FirebaseMessagingException ex) when (
            ex.ErrorCode == ErrorCode.Unregistered || 
            ex.ErrorCode == ErrorCode.InvalidRegistration ||
            ex.ErrorCode == ErrorCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error validating token {Token}", MaskToken(token));
            return false;
        }
    }

    private async Task HandleFailedToken(int deviceTokenId, string? error, CancellationToken cancellationToken)
    {
        try
        {
            // If the token is invalid, deactivate it
            if (error != null && (error.Contains("Invalid") || error.Contains("unregistered") || error.Contains("not found")))
            {
                var deactivateCommand = new DeactivateDeviceTokenCommand { Id = deviceTokenId };
                await _mediator.Send(deactivateCommand, cancellationToken);
                
                _logger.LogInformation("Deactivated invalid device token {DeviceTokenId}", deviceTokenId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling failed token {DeviceTokenId}", deviceTokenId);
        }
    }

    private static string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length < 8)
            return "***";
        
        return $"{token[..4]}...{token[^4..]}";
    }
}

public class PushNotificationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public bool IsTokenInvalid { get; set; }
    public bool IsRetryable { get; set; }
    public List<SingleTokenResult> TokenResults { get; set; } = new();

    public static PushNotificationResult Success(string message = "Success")
    {
        return new PushNotificationResult { Success = true, Message = message };
    }

    public static PushNotificationResult Failed(string error, bool isTokenInvalid = false, bool isRetryable = false)
    {
        return new PushNotificationResult 
        { 
            Success = false, 
            Error = error, 
            Message = error,
            IsTokenInvalid = isTokenInvalid,
            IsRetryable = isRetryable
        };
    }
}

public class SingleTokenResult
{
    public string Token { get; set; } = string.Empty;
    public int DeviceTokenId { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}