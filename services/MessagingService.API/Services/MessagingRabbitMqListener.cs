using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MessagingService.API.Data;
using MessagingService.API.Models;

namespace MessagingService.API.Services
{
    public class MessagingRabbitMqListener : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<MessagingRabbitMqListener> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _hostname;
        private readonly int _maxRetries = 10;
        private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(5);

        public MessagingRabbitMqListener(IServiceProvider services, ILogger<MessagingRabbitMqListener> logger)
        {
            _services = services;
            _logger = logger;
            _hostname = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "rabbitmq";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitForRabbitMqAsync(stoppingToken);
            
            if (stoppingToken.IsCancellationRequested)
                return;

            await SetupRabbitMqAsync();
            await StartConsumingAsync(stoppingToken);
        }

        private async Task WaitForRabbitMqAsync(CancellationToken stoppingToken)
        {
            var retryCount = 0;
            
            while (retryCount < _maxRetries && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to RabbitMQ at {Hostname} (attempt {RetryCount}/{MaxRetries})", 
                        _hostname, retryCount + 1, _maxRetries);

                    var factory = new ConnectionFactory
                    {
                        HostName = _hostname,
                        UserName = "guest",
                        Password = "guest",
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(10),
                        SocketReadTimeout = TimeSpan.FromSeconds(10),
                        SocketWriteTimeout = TimeSpan.FromSeconds(10)
                    };

                    _connection = await factory.CreateConnectionAsync();
                    _logger.LogInformation("Successfully connected to RabbitMQ");
                    return;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning("Failed to connect to RabbitMQ (attempt {RetryCount}/{MaxRetries}): {Error}", 
                        retryCount, _maxRetries, ex.Message);
                    
                    if (retryCount < _maxRetries)
                    {
                        await Task.Delay(_retryDelay, stoppingToken);
                    }
                }
            }
            
            if (retryCount >= _maxRetries)
            {
                _logger.LogError("Failed to connect to RabbitMQ after {MaxRetries} attempts", _maxRetries);
                throw new InvalidOperationException($"Could not connect to RabbitMQ after {_maxRetries} attempts");
            }
        }

        private async Task SetupRabbitMqAsync()
        {
            if (_connection == null)
                throw new InvalidOperationException("RabbitMQ connection is not established");

            _channel = await _connection.CreateChannelAsync();

            // Declare queues for different messaging events
            await _channel.QueueDeclareAsync(queue: "messaging_events_queue",
                                           durable: true,
                                           exclusive: false,
                                           autoDelete: false,
                                           arguments: null);

            await _channel.QueueDeclareAsync(queue: "message_analytics_queue",
                                           durable: true,
                                           exclusive: false,
                                           autoDelete: false,
                                           arguments: null);
            
            _logger.LogInformation("RabbitMQ queues declared successfully");
        }

        private async Task StartConsumingAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not established");

            // Consumer for messaging events (like new matches, notifications)
            var messagingConsumer = new AsyncEventingBasicConsumer(_channel);
            messagingConsumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received messaging event: {Message}", json);

                    var evt = JsonSerializer.Deserialize<MessagingEvent>(json);
                    if (evt != null)
                    {
                        await ProcessMessagingEventAsync(evt);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing messaging event");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            // Consumer for message analytics
            var analyticsConsumer = new AsyncEventingBasicConsumer(_channel);
            analyticsConsumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received analytics event: {Message}", json);

                    var evt = JsonSerializer.Deserialize<MessageAnalyticsEvent>(json);
                    if (evt != null)
                    {
                        await ProcessAnalyticsEventAsync(evt);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing analytics event");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(queue: "messaging_events_queue",
                                           autoAck: false,
                                           consumer: messagingConsumer);

            await _channel.BasicConsumeAsync(queue: "message_analytics_queue",
                                           autoAck: false,
                                           consumer: analyticsConsumer);

            _logger.LogInformation("Started consuming messages from messaging queues");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessMessagingEventAsync(MessagingEvent evt)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();

            switch (evt.EventType.ToLower())
            {
                case "new_match":
                    // Create welcome message for new match
                    var welcomeMessage = new Message
                    {
                        MatchId = evt.MatchId,
                        SenderId = 0, // System message
                        Content = "🎉 You have a new match! Start the conversation!",
                        Type = "system",
                        ExtraData = JsonSerializer.Serialize(new { isWelcome = true }),
                        CreatedAt = DateTime.UtcNow
                    };
                    db.Messages.Add(welcomeMessage);
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Created welcome message for new match {MatchId}", evt.MatchId);
                    break;

                case "match_expired":
                    // Could archive messages or send notification
                    _logger.LogInformation("Match {MatchId} expired, could implement cleanup logic", evt.MatchId);
                    break;

                default:
                    _logger.LogWarning("Unknown messaging event type: {EventType}", evt.EventType);
                    break;
            }
        }

        private async Task ProcessAnalyticsEventAsync(MessageAnalyticsEvent evt)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();

            // Process analytics data (could be for reporting, insights, etc.)
            _logger.LogInformation("Processing analytics for {EventType}: MessageId={MessageId}, UserId={UserId}", 
                evt.EventType, evt.MessageId, evt.UserId);

            // Here you could:
            // - Update message read receipts
            // - Track user engagement metrics
            // - Generate conversation insights
            // - Feed data to AI services

            if (evt.EventType == "message_read" && evt.MessageId.HasValue)
            {
                var message = await db.Messages.FindAsync(evt.MessageId.Value);
                if (message != null && message.ReadAt == null)
                {
                    message.ReadAt = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Marked message {MessageId} as read", evt.MessageId);
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                if (_channel != null)
                {
                    _channel.CloseAsync().GetAwaiter().GetResult();
                    _channel.DisposeAsync().GetAwaiter().GetResult();
                }
                if (_connection != null)
                {
                    _connection.CloseAsync().GetAwaiter().GetResult();
                    _connection.DisposeAsync().GetAwaiter().GetResult();
                }
                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connection");
            }
            base.Dispose();
        }
    }

    public record MessagingEvent(int MatchId, string EventType, string Data);
    public record MessageAnalyticsEvent(int UserId, string EventType, int? MessageId = null, string? Data = null);
}