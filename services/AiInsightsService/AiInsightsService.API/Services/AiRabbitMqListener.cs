using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AiInsightsService.API.Data;
using AiInsightsService.API.Models;

namespace AiInsightsService.API.Services
{
    public class AiRabbitMqListener : BackgroundService
    {
        private readonly IServiceProvider _services;
        private IConnection? _connection;
        private IModel? _channel;

        public AiRabbitMqListener(IServiceProvider services)
        {
            _services = services;

            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "ai_insights_queue",
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

                var evt = JsonSerializer.Deserialize<UserEvent>(json);
                if (evt != null)
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AiDbContext>();

                    // TODO: Replace with real AI analysis
                    var insight = await db.AiInsights.FindAsync(evt.UserId)
                                  ?? new AiInsight { UserId = evt.UserId };

                    insight.SummaryText = $"Auto-generated summary for user {evt.UserId}";
                    insight.PersonalityTags = JsonSerializer.Serialize(new[] { "friendly", "curious" });
                    insight.CompatibilityScore = new Random().Next(50, 100); // placeholder
                    insight.UpdatedAt = DateTime.UtcNow;

                    db.AiInsights.Update(insight);
                    await db.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "ai_insights_queue",
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

    public record UserEvent(int UserId, string EventType, string Data);
}
