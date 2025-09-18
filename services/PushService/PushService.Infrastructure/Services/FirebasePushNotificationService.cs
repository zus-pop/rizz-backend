using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using PushService.Domain.Repositories;
using PushService.Domain.ValueObjects;
using DomainNotificationPriority = PushService.Domain.ValueObjects.NotificationPriority;

namespace PushService.Infrastructure.Services
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly ILogger<FirebasePushNotificationService> _logger;

        public FirebasePushNotificationService(
            IDeviceTokenRepository deviceTokenRepository,
            ILogger<FirebasePushNotificationService> logger)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(string token, PushNotification notification, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = CreateFirebaseMessage(token, notification);
                
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
                _logger.LogInformation("Successfully sent message: {Response}", response);
                
                // Update token last used time
                var deviceToken = await _deviceTokenRepository.GetByTokenAsync(token, cancellationToken);
                if (deviceToken != null)
                {
                    deviceToken.UpdateLastUsed();
                    await _deviceTokenRepository.UpdateAsync(deviceToken, cancellationToken);
                }
                
                return true;
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "Firebase messaging error for token {Token}: {ErrorCode}", token, ex.MessagingErrorCode);
                
                // Handle invalid token
                if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered || ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                {
                    var deviceToken = await _deviceTokenRepository.GetByTokenAsync(token, cancellationToken);
                    if (deviceToken != null)
                    {
                        deviceToken.Deactivate();
                        await _deviceTokenRepository.UpdateAsync(deviceToken, cancellationToken);
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending notification to token {Token}", token);
                return false;
            }
        }

        public async Task<bool> SendNotificationToUserAsync(int userId, PushNotification notification, CancellationToken cancellationToken = default)
        {
            var activeTokens = await _deviceTokenRepository.GetActiveTokensByUserIdAsync(userId, cancellationToken);
            var tokens = activeTokens.Select(dt => dt.Token).ToList();
            
            if (!tokens.Any())
            {
                _logger.LogWarning("No active tokens found for user {UserId}", userId);
                return false;
            }

            var results = await SendNotificationToTokensAsync(tokens, notification, cancellationToken);
            return results.Values.Any(success => success);
        }

        public async Task<bool> SendNotificationToMultipleUsersAsync(IEnumerable<int> userIds, PushNotification notification, CancellationToken cancellationToken = default)
        {
            var allTokens = new List<string>();
            
            foreach (var userId in userIds)
            {
                var activeTokens = await _deviceTokenRepository.GetActiveTokensByUserIdAsync(userId, cancellationToken);
                allTokens.AddRange(activeTokens.Select(dt => dt.Token));
            }
            
            if (!allTokens.Any())
            {
                _logger.LogWarning("No active tokens found for users {UserIds}", string.Join(", ", userIds));
                return false;
            }

            var results = await SendNotificationToTokensAsync(allTokens, notification, cancellationToken);
            return results.Values.Any(success => success);
        }

        public async Task<bool> SendNotificationToAllAsync(PushNotification notification, CancellationToken cancellationToken = default)
        {
            // For safety, implement pagination or limit to prevent overwhelming Firebase
            var allActiveTokens = await _deviceTokenRepository.GetByDeviceTypeAsync(Domain.ValueObjects.DeviceType.iOS, cancellationToken);
            var androidTokens = await _deviceTokenRepository.GetByDeviceTypeAsync(Domain.ValueObjects.DeviceType.Android, cancellationToken);
            var webTokens = await _deviceTokenRepository.GetByDeviceTypeAsync(Domain.ValueObjects.DeviceType.Web, cancellationToken);
            
            var allTokens = allActiveTokens.Concat(androidTokens).Concat(webTokens)
                .Select(dt => dt.Token).Distinct().ToList();
            
            if (!allTokens.Any())
            {
                _logger.LogWarning("No active tokens found for broadcast notification");
                return false;
            }

            // Send in batches to avoid Firebase limits
            const int batchSize = 500;
            var batches = allTokens.Chunk(batchSize);
            bool anySuccess = false;
            
            foreach (var batch in batches)
            {
                var results = await SendNotificationToTokensAsync(batch, notification, cancellationToken);
                if (results.Values.Any(success => success))
                    anySuccess = true;
            }
            
            return anySuccess;
        }

        public async Task<Dictionary<string, bool>> SendNotificationToTokensAsync(IEnumerable<string> tokens, PushNotification notification, CancellationToken cancellationToken = default)
        {
            var results = new Dictionary<string, bool>();
            var tokenList = tokens.ToList();
            
            if (!tokenList.Any())
                return results;

            try
            {
                var message = CreateMulticastMessage(tokenList, notification);
                
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);
                
                _logger.LogInformation("Multicast message sent. Success: {SuccessCount}, Failure: {FailureCount}", 
                    response.SuccessCount, response.FailureCount);
                
                // Process results
                for (int i = 0; i < tokenList.Count; i++)
                {
                    var token = tokenList[i];
                    bool success = i < response.Responses.Count && response.Responses[i].IsSuccess;
                    results[token] = success;
                    
                    if (success)
                    {
                        // Update token last used time
                        var deviceToken = await _deviceTokenRepository.GetByTokenAsync(token, cancellationToken);
                        if (deviceToken != null)
                        {
                            deviceToken.UpdateLastUsed();
                            await _deviceTokenRepository.UpdateAsync(deviceToken, cancellationToken);
                        }
                    }
                    else if (i < response.Responses.Count)
                    {
                        var error = response.Responses[i].Exception;
                        _logger.LogWarning("Failed to send to token {Token}: {Error}", token, error?.Message);
                        
                        // Handle invalid tokens
                        if (error is FirebaseMessagingException fmEx && 
                            (fmEx.MessagingErrorCode == MessagingErrorCode.Unregistered || fmEx.MessagingErrorCode == MessagingErrorCode.InvalidArgument))
                        {
                            var deviceToken = await _deviceTokenRepository.GetByTokenAsync(token, cancellationToken);
                            if (deviceToken != null)
                            {
                                deviceToken.Deactivate();
                                await _deviceTokenRepository.UpdateAsync(deviceToken, cancellationToken);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending multicast notification");
                foreach (var token in tokenList)
                {
                    results[token] = false;
                }
            }
            
            return results;
        }

        private static Message CreateFirebaseMessage(string token, PushNotification notification)
        {
            var messageBuilder = new Message()
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl
                },
                Data = notification.Data
            };

            // Set priority and other options based on notification priority
            switch (notification.Priority)
            {
                case DomainNotificationPriority.High:
                    messageBuilder.Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = notification.Sound ?? "default"
                        }
                    };
                    messageBuilder.Apns = new ApnsConfig
                    {
                        Headers = new Dictionary<string, string>
                        {
                            ["apns-priority"] = "10"
                        },
                        Aps = new Aps
                        {
                            Sound = notification.Sound ?? "default"
                        }
                    };
                    break;
                case DomainNotificationPriority.Low:
                    messageBuilder.Android = new AndroidConfig
                    {
                        Priority = Priority.Normal
                    };
                    messageBuilder.Apns = new ApnsConfig
                    {
                        Headers = new Dictionary<string, string>
                        {
                            ["apns-priority"] = "5"
                        }
                    };
                    break;
                default:
                    messageBuilder.Android = new AndroidConfig
                    {
                        Priority = Priority.Normal,
                        Notification = new AndroidNotification
                        {
                            Sound = notification.Sound
                        }
                    };
                    if (!string.IsNullOrWhiteSpace(notification.Sound))
                    {
                        messageBuilder.Apns = new ApnsConfig
                        {
                            Aps = new Aps
                            {
                                Sound = notification.Sound
                            }
                        };
                    }
                    break;
            }

            return messageBuilder;
        }

        private static MulticastMessage CreateMulticastMessage(IEnumerable<string> tokens, PushNotification notification)
        {
            var messageBuilder = new MulticastMessage()
            {
                Tokens = tokens.ToList(),
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl
                },
                Data = notification.Data
            };

            // Set priority and other options
            switch (notification.Priority)
            {
                case DomainNotificationPriority.High:
                    messageBuilder.Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = notification.Sound ?? "default"
                        }
                    };
                    messageBuilder.Apns = new ApnsConfig
                    {
                        Headers = new Dictionary<string, string>
                        {
                            ["apns-priority"] = "10"
                        },
                        Aps = new Aps
                        {
                            Sound = notification.Sound ?? "default"
                        }
                    };
                    break;
                case DomainNotificationPriority.Low:
                    messageBuilder.Android = new AndroidConfig
                    {
                        Priority = Priority.Normal
                    };
                    messageBuilder.Apns = new ApnsConfig
                    {
                        Headers = new Dictionary<string, string>
                        {
                            ["apns-priority"] = "5"
                        }
                    };
                    break;
                default:
                    messageBuilder.Android = new AndroidConfig
                    {
                        Priority = Priority.Normal,
                        Notification = new AndroidNotification
                        {
                            Sound = notification.Sound
                        }
                    };
                    if (!string.IsNullOrWhiteSpace(notification.Sound))
                    {
                        messageBuilder.Apns = new ApnsConfig
                        {
                            Aps = new Aps
                            {
                                Sound = notification.Sound
                            }
                        };
                    }
                    break;
            }

            return messageBuilder;
        }
    }
}