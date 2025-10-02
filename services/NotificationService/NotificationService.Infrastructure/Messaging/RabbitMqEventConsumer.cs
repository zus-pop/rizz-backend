using Common.Contracts.Events;
using Common.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using NotificationService.Application.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastructure.Messaging;

public interface IEventConsumer : IDisposable
{
    Task StartConsumingAsync(CancellationToken cancellationToken = default);
    Task StopConsumingAsync();
}

public class RabbitMqEventConsumer : IEventConsumer
{
    private readonly IConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqEventConsumer> _logger;
    private IModel? _channel;
    private readonly Dictionary<string, Func<string, Task>> _eventHandlers;

    public RabbitMqEventConsumer(
        IConnection connection,
        IServiceProvider serviceProvider,
        ILogger<RabbitMqEventConsumer> logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _eventHandlers = new Dictionary<string, Func<string, Task>>
        {
            ["MatchCreatedEvent"] = HandleMatchCreatedEventAsync,
            ["PurchaseCompletedEvent"] = HandlePurchaseCompletedEventAsync
        };
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        _channel = _connection.CreateModel();

        // Declare exchanges and queues
        _channel.ExchangeDeclare("events", ExchangeType.Topic, durable: true);
        _channel.QueueDeclare("notification.events", durable: true, exclusive: false, autoDelete: false);
        
        // Bind to specific event types
        _channel.QueueBind("notification.events", "events", "match.created");
        _channel.QueueBind("notification.events", "events", "purchase.completed");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                var eventType = routingKey switch
                {
                    "match.created" => "MatchCreatedEvent",
                    "purchase.completed" => "PurchaseCompletedEvent",
                    _ => null
                };

                if (eventType != null && _eventHandlers.TryGetValue(eventType, out var handler))
                {
                    await handler(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _logger.LogWarning("Unknown event type: {RoutingKey}", routingKey);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event message");
                _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue on error
            }
        };

        _channel.BasicConsume("notification.events", false, consumer);
        _logger.LogInformation("Started consuming events from RabbitMQ");

        await Task.CompletedTask;
    }

    private async Task HandleMatchCreatedEventAsync(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var matchEvent = JsonSerializer.Deserialize<MatchCreatedEvent>(message);
        if (matchEvent == null) return;

        // Create notifications for both users
        var user1NotificationCommand = new CreateNotificationCommand(new()
        {
            UserId = matchEvent.User1Id.ToString(),
            Type = "match_created",
            Title = "New Match! 🎉",
            Message = "You have a new match! Start chatting now.",
            Priority = "high",
            DeliveryChannels = new List<string> { "push", "in-app" },
            Variables = new Dictionary<string, string>
            {
                ["matchId"] = matchEvent.MatchId.ToString(),
                ["partnerId"] = matchEvent.User2Id.ToString()
            }
        });

        var user2NotificationCommand = new CreateNotificationCommand(new()
        {
            UserId = matchEvent.User2Id.ToString(),
            Type = "match_created",
            Title = "New Match! 🎉",
            Message = "You have a new match! Start chatting now.",
            Priority = "high",
            DeliveryChannels = new List<string> { "push", "in-app" },
            Variables = new Dictionary<string, string>
            {
                ["matchId"] = matchEvent.MatchId.ToString(),
                ["partnerId"] = matchEvent.User1Id.ToString()
            }
        });

        await mediator.Send(user1NotificationCommand);
        await mediator.Send(user2NotificationCommand);

        _logger.LogInformation("Created match notifications for users {User1Id} and {User2Id}", 
            matchEvent.User1Id, matchEvent.User2Id);
    }

    private async Task HandlePurchaseCompletedEventAsync(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var purchaseEvent = JsonSerializer.Deserialize<PurchaseCompletedEvent>(message);
        if (purchaseEvent == null) return;

        var notificationCommand = new CreateNotificationCommand(new()
        {
            UserId = purchaseEvent.UserId.ToString(),
            Type = "purchase_completed",
            Title = "Purchase Confirmed! ✅",
            Message = $"Your {purchaseEvent.ProductType} purchase has been completed successfully.",
            Priority = "normal",
            DeliveryChannels = new List<string> { "push", "in-app", "email" },
            Variables = new Dictionary<string, string>
            {
                ["purchaseId"] = purchaseEvent.PurchaseId.ToString(),
                ["productType"] = purchaseEvent.ProductType,
                ["amount"] = purchaseEvent.Amount.ToString(),
                ["currency"] = purchaseEvent.Currency
            }
        });

        await mediator.Send(notificationCommand);

        _logger.LogInformation("Created purchase notification for user {UserId} for purchase {PurchaseId}", 
            purchaseEvent.UserId, purchaseEvent.PurchaseId);
    }

    public async Task StopConsumingAsync()
    {
        _channel?.Close();
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}