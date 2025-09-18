using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MediatR;
using AiInsightsService.Application.Commands;

namespace AiInsightsService.Infrastructure.Services;

public class AiRabbitMqListener : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AiRabbitMqListener> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly string _hostname;
    private readonly int _maxRetries = 10;
    private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(5);

    public AiRabbitMqListener(IServiceProvider services, ILogger<AiRabbitMqListener> logger)
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
        _logger.LogInformation("Waiting for RabbitMQ to be available...");
        
        for (int i = 0; i < _maxRetries && !stoppingToken.IsCancellationRequested; i++)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _hostname };
                using var testConnection = factory.CreateConnection();
                _logger.LogInformation("RabbitMQ is available!");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"RabbitMQ not available (attempt {i + 1}/{_maxRetries}): {ex.Message}");
                await Task.Delay(_retryDelay, stoppingToken);
            }
        }
        
        _logger.LogError("Failed to connect to RabbitMQ after {MaxRetries} attempts", _maxRetries);
    }

    private async Task SetupRabbitMqAsync()
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchanges and queues
            _channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Topic, durable: true);
            _channel.ExchangeDeclare(exchange: "ai_insights", type: ExchangeType.Topic, durable: true);

            // Queue for processing user profile updates
            _channel.QueueDeclare(queue: "ai_insights.user_profile_updates", 
                                 durable: true, 
                                 exclusive: false, 
                                 autoDelete: false);
            
            _channel.QueueBind(queue: "ai_insights.user_profile_updates",
                              exchange: "user_events",
                              routingKey: "user.profile.updated");

            // Queue for processing user behavior events
            _channel.QueueDeclare(queue: "ai_insights.user_behavior", 
                                 durable: true, 
                                 exclusive: false, 
                                 autoDelete: false);
            
            _channel.QueueBind(queue: "ai_insights.user_behavior",
                              exchange: "user_events",
                              routingKey: "user.behavior.*");

            _logger.LogInformation("RabbitMQ setup completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ");
            throw;
        }
    }

    private async Task StartConsumingAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
            return;

        // Consumer for user profile updates
        var profileConsumer = new EventingBasicConsumer(_channel);
        profileConsumer.Received += async (model, ea) =>
        {
            await ProcessUserProfileUpdateAsync(ea);
        };

        // Consumer for user behavior events
        var behaviorConsumer = new EventingBasicConsumer(_channel);
        behaviorConsumer.Received += async (model, ea) =>
        {
            await ProcessUserBehaviorEventAsync(ea);
        };

        _channel.BasicConsume(queue: "ai_insights.user_profile_updates",
                             autoAck: false,
                             consumer: profileConsumer);

        _channel.BasicConsume(queue: "ai_insights.user_behavior",
                             autoAck: false,
                             consumer: behaviorConsumer);

        _logger.LogInformation("Started consuming messages from RabbitMQ");

        // Keep running until cancellation is requested
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessUserProfileUpdateAsync(BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var profileUpdate = JsonSerializer.Deserialize<UserProfileUpdateEvent>(message);

            if (profileUpdate != null)
            {
                _logger.LogInformation("Processing user profile update for user {UserId}", profileUpdate.UserId);

                using var scope = _services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new GenerateAiInsightCommand(
                    profileUpdate.UserId,
                    profileUpdate.ProfileData,
                    profileUpdate.BehaviorData
                );

                await mediator.Send(command);
                
                _logger.LogInformation("AI insight generated for user {UserId}", profileUpdate.UserId);
            }

            _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user profile update");
            _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private async Task ProcessUserBehaviorEventAsync(BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var behaviorEvent = JsonSerializer.Deserialize<UserBehaviorEvent>(message);

            if (behaviorEvent != null)
            {
                _logger.LogInformation("Processing user behavior event for user {UserId}: {EventType}", 
                    behaviorEvent.UserId, behaviorEvent.EventType);

                // For behavior events, we might just update existing insights
                // rather than generating completely new ones
                using var scope = _services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // This could trigger a lighter update of existing insights
                // Implementation depends on business requirements
            }

            _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user behavior event");
            _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

// Event models for RabbitMQ messages
public class UserProfileUpdateEvent
{
    public int UserId { get; set; }
    public Dictionary<string, object> ProfileData { get; set; } = new();
    public string? BehaviorData { get; set; }
    public DateTime Timestamp { get; set; }
}

public class UserBehaviorEvent
{
    public int UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Dictionary<string, object> EventData { get; set; } = new();
    public DateTime Timestamp { get; set; }
}