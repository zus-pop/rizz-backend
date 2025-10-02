using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NotificationService.Infrastructure.Services;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventPublisher(ILogger<RabbitMqEventPublisher> logger, IConnection connection)
    {
        _logger = logger;
        _connection = connection;
        _channel = _connection.CreateModel();

        // Declare the exchange for notification events
        _channel.ExchangeDeclare(
            exchange: "notification.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var eventName = @event.GetType().Name;
            var routingKey = $"notification.{eventName.ToLower()}";
            
            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Type = eventName;

            _channel.BasicPublish(
                exchange: "notification.events",
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogDebug("Published event {EventName} to RabbitMQ with routing key {RoutingKey}", 
                eventName, routingKey);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType}", typeof(T).Name);
            throw;
        }
    }

    public async Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}

public class CurrentUserService : ICurrentUserService
{
    // This would typically be injected with actual user context from HTTP context
    public string? UserId => "system"; // For background services
    public string? UserName => "System";
    public bool IsAuthenticated => false;
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
    public DateTimeOffset UtcOffsetNow => DateTimeOffset.UtcNow;
}