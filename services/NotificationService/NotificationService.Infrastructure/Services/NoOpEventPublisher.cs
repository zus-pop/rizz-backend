using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class NoOpEventPublisher : IEventPublisher
{
    private readonly ILogger<NoOpEventPublisher> _logger;

    public NoOpEventPublisher(ILogger<NoOpEventPublisher> logger)
    {
        _logger = logger;
        _logger.LogInformation("NoOpEventPublisher created successfully - RabbitMQ is disabled");
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("NoOpEventPublisher: Event {EventType} would be published (but is disabled for testing)", typeof(T).Name);
        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("NoOpEventPublisher: {EventCount} events of type {EventType} would be published (but is disabled for testing)", events.Count(), typeof(T).Name);
        return Task.CompletedTask;
    }
}