using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MatchService.API.Services
{
    public class RabbitMqService : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly ILogger<RabbitMqService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed;

        public RabbitMqService(IConfiguration config, ILogger<RabbitMqService> logger)
        {
            _logger = logger;
            
            // Ensure fallback to default host/user/pass if not provided
            var host = config["RabbitMQ:Host"] ?? "localhost";
            var user = config["RabbitMQ:User"] ?? "guest";
            var pass = config["RabbitMQ:Pass"] ?? "guest";

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

        /// <summary>
        /// Publish a POCO object as JSON to the given queue.
        /// </summary>
        public async Task PublishAsync(string queue, object message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqService));

            if (_channel == null)
            {
                _logger.LogWarning("RabbitMQ channel not available. Message not published.");
                return;
            }

            try
            {
                // Declare queue as durable for production
                await _channel.QueueDeclareAsync(queue: queue,
                                              durable: true,
                                              exclusive: false,
                                              autoDelete: false,
                                              arguments: null);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var props = new BasicProperties
                {
                    Persistent = true // mark message as persistent
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
