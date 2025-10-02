using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MessagingService.Domain.Events;

namespace MessagingService.API.Services
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly ILogger<RabbitMqPublisher> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed;

        public RabbitMqPublisher(IConfiguration config, ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;
            
            var host = config["RabbitMq:Host"] ?? "localhost";
            var user = config["RabbitMq:User"] ?? "guest";
            var pass = config["RabbitMq:Pass"] ?? "guest";

            _factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pass
            };

            InitializeConnectionAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeConnectionAsync()
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                _logger.LogInformation("RabbitMQ connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ. Service will continue without messaging.");
            }
        }

        public async Task PublishMessageEventAsync(MessageSentEvent messageEvent)
        {
            await PublishAsync("message.events", messageEvent);
        }

        public async Task PublishAsync(string queue, object message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqPublisher));

            if (_channel == null)
            {
                _logger.LogWarning("RabbitMQ channel not available. Message not published.");
                return;
            }

            try
            {
                // Declare queue as durable
                await _channel.QueueDeclareAsync(queue: queue,
                                              durable: true,
                                              exclusive: false,
                                              autoDelete: false,
                                              arguments: null);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var props = new BasicProperties
                {
                    Persistent = true
                };

                await _channel.BasicPublishAsync(exchange: "",
                                              routingKey: queue,
                                              mandatory: false,
                                              basicProperties: props,
                                              body: body);

                _logger.LogDebug("Message published to queue {Queue}", queue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to queue {Queue}", queue);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            try 
            { 
                _channel?.CloseAsync().GetAwaiter().GetResult(); 
                _channel?.Dispose();
            } 
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error closing RabbitMQ channel");
            }
            
            try 
            { 
                _connection?.CloseAsync().GetAwaiter().GetResult();
                _connection?.Dispose();
            } 
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error closing RabbitMQ connection");
            }
            
            _disposed = true;
        }
    }
}
