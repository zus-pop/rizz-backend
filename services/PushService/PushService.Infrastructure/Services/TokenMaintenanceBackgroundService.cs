using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PushService.Application.Commands;

namespace PushService.Infrastructure.Services;

public class TokenMaintenanceBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenMaintenanceBackgroundService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6); // Run every 6 hours
    private readonly TimeSpan _expirationTime = TimeSpan.FromDays(30); // Tokens expire after 30 days of inactivity

    public TokenMaintenanceBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TokenMaintenanceBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Token Maintenance Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformMaintenanceAsync(stoppingToken);
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Token Maintenance Background Service");
                // Wait a shorter time before retrying on error
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        _logger.LogInformation("Token Maintenance Background Service stopped");
    }

    private async Task PerformMaintenanceAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            _logger.LogInformation("Starting token maintenance tasks");

            // Clean up expired tokens
            var cleanupCommand = new CleanupExpiredTokensCommand
            {
                ExpirationTime = _expirationTime
            };

            var cleanedCount = await mediator.Send(cleanupCommand, cancellationToken);
            
            if (cleanedCount > 0)
            {
                _logger.LogInformation("Cleaned up {Count} expired device tokens", cleanedCount);
            }
            else
            {
                _logger.LogDebug("No expired tokens found during maintenance");
            }

            // TODO: Add more maintenance tasks:
            // - Validate token health periodically
            // - Update token statistics
            // - Send health metrics to monitoring service

            _logger.LogDebug("Token maintenance completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token maintenance");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Token Maintenance Background Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}