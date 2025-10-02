using NotificationService.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;

/// <summary>
/// NoOp implementation of IEventConsumer that disables RabbitMQ event consumption
/// </summary>
public class NoOpEventConsumer : IEventConsumer
{
    private readonly ILogger<NoOpEventConsumer> _logger;

    public NoOpEventConsumer(ILogger<NoOpEventConsumer> logger)
    {
        _logger = logger;
        _logger.LogInformation("NoOpEventConsumer created - Event consumption is disabled");
    }

    public Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("NoOpEventConsumer.StartConsumingAsync called - Event consumption is disabled");
        return Task.CompletedTask;
    }

    public Task StopConsumingAsync()
    {
        _logger.LogInformation("NoOpEventConsumer.StopConsumingAsync called - Event consumption is disabled");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogInformation("NoOpEventConsumer disposed - Event consumption was disabled");
    }
}