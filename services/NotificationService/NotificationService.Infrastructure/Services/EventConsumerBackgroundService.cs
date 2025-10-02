using NotificationService.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;

public class EventConsumerBackgroundService : BackgroundService
{
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<EventConsumerBackgroundService> _logger;

    public EventConsumerBackgroundService(
        IEventConsumer eventConsumer,
        ILogger<EventConsumerBackgroundService> logger)
    {
        _eventConsumer = eventConsumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Event Consumer Background Service starting");
            await _eventConsumer.StartConsumingAsync(stoppingToken);

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Event Consumer Background Service stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Event Consumer Background Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Consumer Background Service stopping");
        await _eventConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _eventConsumer?.Dispose();
        base.Dispose();
    }
}