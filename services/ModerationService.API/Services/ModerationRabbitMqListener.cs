using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ModerationService.API.Data;
using ModerationService.API.Models;

namespace ModerationService.API.Services
{
    public class ModerationRabbitMqListener : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ModerationRabbitMqListener> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _hostname;
        private readonly int _maxRetries = 10;
        private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(5);

        public ModerationRabbitMqListener(IServiceProvider services, ILogger<ModerationRabbitMqListener> logger)
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

            // Declare queues for moderation events
            await _channel.QueueDeclareAsync(queue: "moderation_events_queue",
                                           durable: true,
                                           exclusive: false,
                                           autoDelete: false,
                                           arguments: null);

            await _channel.QueueDeclareAsync(queue: "content_analysis_queue",
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

            // Consumer for moderation events (user reports, automated flags, etc.)
            var moderationConsumer = new AsyncEventingBasicConsumer(_channel);
            moderationConsumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received moderation event: {Message}", json);

                    var evt = JsonSerializer.Deserialize<ModerationEvent>(json);
                    if (evt != null)
                    {
                        await ProcessModerationEventAsync(evt);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing moderation event");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            // Consumer for content analysis
            var contentConsumer = new AsyncEventingBasicConsumer(_channel);
            contentConsumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received content analysis event: {Message}", json);

                    var evt = JsonSerializer.Deserialize<ContentAnalysisEvent>(json);
                    if (evt != null)
                    {
                        await ProcessContentAnalysisEventAsync(evt);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing content analysis event");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(queue: "moderation_events_queue",
                                           autoAck: false,
                                           consumer: moderationConsumer);

            await _channel.BasicConsumeAsync(queue: "content_analysis_queue",
                                           autoAck: false,
                                           consumer: contentConsumer);

            _logger.LogInformation("Started consuming messages from moderation queues");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessModerationEventAsync(ModerationEvent evt)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();

            switch (evt.EventType.ToLower())
            {
                case "user_reported":
                    // Automatically create a report in the database
                    var report = new Report
                    {
                        ReporterId = evt.ReporterId,
                        ReportedId = evt.ReportedId,
                        Reason = evt.Reason ?? "Automated report from event",
                        CreatedAt = DateTime.UtcNow
                    };
                    db.Reports.Add(report);
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Created report for user {ReportedId} by {ReporterId}", evt.ReportedId, evt.ReporterId);
                    break;

                case "auto_block":
                    // Automatically block a user based on analysis
                    var block = new Block
                    {
                        BlockerId = 0, // System block
                        BlockedId = evt.ReportedId,
                        CreatedAt = DateTime.UtcNow
                    };
                    db.Blocks.Add(block);
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Auto-blocked user {BlockedId}", evt.ReportedId);
                    break;

                case "content_flagged":
                    // Handle flagged content
                    _logger.LogInformation("Content flagged for user {UserId}: {ContentType}", evt.ReportedId, evt.Reason);
                    break;

                default:
                    _logger.LogWarning("Unknown moderation event type: {EventType}", evt.EventType);
                    break;
            }
        }

        private async Task ProcessContentAnalysisEventAsync(ContentAnalysisEvent evt)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ModerationDbContext>();

            // Process content analysis (could be AI-powered content moderation)
            _logger.LogInformation("Processing content analysis for {ContentType}: UserId={UserId}, Score={ToxicityScore}", 
                evt.ContentType, evt.UserId, evt.ToxicityScore);

            // If toxicity score is above threshold, create an automatic report
            if (evt.ToxicityScore > 0.8)
            {
                var autoReport = new Report
                {
                    ReporterId = 0, // System report
                    ReportedId = evt.UserId,
                    Reason = $"Automated moderation: High toxicity score ({evt.ToxicityScore:F2}) in {evt.ContentType}",
                    CreatedAt = DateTime.UtcNow
                };
                db.Reports.Add(autoReport);
                await db.SaveChangesAsync();
                
                _logger.LogWarning("Auto-reported user {UserId} for high toxicity score {Score}", evt.UserId, evt.ToxicityScore);
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

    public record ModerationEvent(int ReporterId, int ReportedId, string EventType, string? Reason = null);
    public record ContentAnalysisEvent(int UserId, string ContentType, double ToxicityScore, string? ContentId = null);
}