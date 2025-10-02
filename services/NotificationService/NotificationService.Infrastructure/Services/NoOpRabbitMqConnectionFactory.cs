using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;

/// <summary>
/// NoOp implementation of IRabbitMqConnectionFactory that prevents RabbitMQ connection attempts
/// </summary>
public class NoOpRabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly ILogger<NoOpRabbitMqConnectionFactory> _logger;

    public NoOpRabbitMqConnectionFactory(ILogger<NoOpRabbitMqConnectionFactory> logger)
    {
        _logger = logger;
        _logger.LogInformation("NoOpRabbitMqConnectionFactory created - RabbitMQ connections are disabled");
    }

    public bool IsConnected => false;

    public RabbitMQ.Client.IConnection? GetConnection()
    {
        _logger.LogInformation("NoOpRabbitMqConnectionFactory.GetConnection called - returning null (RabbitMQ disabled)");
        return null;
    }
}