using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;

public interface IRabbitMqConnectionFactory
{
    IConnection? GetConnection();
    bool IsConnected { get; }
}

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;
    private IConnection? _connection;
    private readonly object _lock = new object();

    public RabbitMqConnectionFactory(IConfiguration configuration, ILogger<RabbitMqConnectionFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsConnected => _connection?.IsOpen == true;

    public IConnection? GetConnection()
    {
        if (IsConnected)
            return _connection;

        lock (_lock)
        {
            if (IsConnected)
                return _connection;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMQ:User"] ?? "guest",
                    Password = _configuration["RabbitMQ:Pass"] ?? "guest",
                    VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/",
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                _connection = factory.CreateConnection();
                _logger.LogInformation("Successfully connected to RabbitMQ at {Host}:{Port}", 
                    factory.HostName, factory.Port);
                
                return _connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ. Service will continue without event publishing.");
                return null;
            }
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}