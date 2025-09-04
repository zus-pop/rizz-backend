using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace MatchService.API.Services
{
    public class RabbitMqService : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private bool _disposed;

        public RabbitMqService(IConfiguration config)
        {
            // Ensure fallback to default host/user/pass if not provided
            var host = config["RabbitMQ:Host"] ?? "localhost";
            var user = config["RabbitMQ:User"] ?? "guest";
            var pass = config["RabbitMQ:Pass"] ?? "guest";

            _factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pass,
                DispatchConsumersAsync = true // useful if you later use async consumers
            };

            // Create and keep one connection + channel for the life of this service
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        /// <summary>
        /// Publish a POCO object as JSON to the given queue.
        /// </summary>
        public void Publish(string queue, object message)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqService));

            // durable queue recommended for production
            _channel.QueueDeclare(queue: queue,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = _channel.CreateBasicProperties();
            props.Persistent = true; // mark message as persistent

            _channel.BasicPublish(exchange: "",
                                  routingKey: queue,
                                  basicProperties: props,
                                  body: body);
        }

        public void Dispose()
        {
            if (_disposed) return;
            try { _channel?.Close(); } catch { }
            try { _connection?.Close(); } catch { }
            _channel?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
