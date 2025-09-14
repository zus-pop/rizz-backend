using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.API.Data;
using NotificationService.API.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using IModel = RabbitMQ.Client.IModel;

namespace NotificationService.API.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IServiceProvider _services;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqListener(IServiceProvider services)
        {
            _services = services;

            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq", // matches docker-compose service name
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "matches_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<MatchCreatedMessage>(json);
                if (message != null)
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                    var notification = new Notification
                    {
                        UserId = message.User1Id,
                        Type = "match",
                        Content = $"You matched with user {message.User2Id}"
                    };
                    db.Notifications.Add(notification);

                    var notification2 = new Notification
                    {
                        UserId = message.User2Id,
                        Type = "match",
                        Content = $"You matched with user {message.User1Id}"
                    };
                    db.Notifications.Add(notification2);

                    await db.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "matches_queue",
                                  autoAck: false,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public record MatchCreatedMessage(int MatchId, int User1Id, int User2Id, DateTime MatchedAt);
}
