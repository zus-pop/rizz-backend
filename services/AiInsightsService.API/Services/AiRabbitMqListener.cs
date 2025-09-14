using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AiInsightsService.API.Data;
using AiInsightsService.API.Models;

namespace AiInsightsService.API.Services
{
    public class AiRabbitMqListener : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<AiRabbitMqListener> _logger;
        private IConnection? _connection;
        private IModel? _channel;
        private readonly string _hostname;
        private readonly int _maxRetries = 10;
        private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(5);

        public AiRabbitMqListener(IServiceProvider services, ILogger<AiRabbitMqListener> logger)
        {
            _services = services;
            _logger = logger;
            _hostname = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "rabbitmq";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitForRabbitMqAsync(stoppingToken);
            
            if (stoppingToken.IsCancellationRequested)
                return;

            await SetupRabbitMqAsync();
            await StartConsumingAsync(stoppingToken);
        }

        private async Task WaitForRabbitMqAsync(CancellationToken stoppingToken)
        {
            var retryCount = 0;
            
            while (retryCount < _maxRetries && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting to connect to RabbitMQ at {Hostname} (attempt {RetryCount}/{MaxRetries})", 
                        _hostname, retryCount + 1, _maxRetries);

                    var factory = new ConnectionFactory
                    {
                        HostName = _hostname,
                        UserName = "guest",
                        Password = "guest",
                        DispatchConsumersAsync = true,
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(10),
                        SocketReadTimeout = TimeSpan.FromSeconds(10),
                        SocketWriteTimeout = TimeSpan.FromSeconds(10)
                    };

                    _connection = factory.CreateConnection();
                    _logger.LogInformation("Successfully connected to RabbitMQ");
                    return;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning("Failed to connect to RabbitMQ (attempt {RetryCount}/{MaxRetries}): {Error}", 
                        retryCount, _maxRetries, ex.Message);
                    
                    if (retryCount < _maxRetries)
                    {
                        await Task.Delay(_retryDelay, stoppingToken);
                    }
                }
            }
            
            if (retryCount >= _maxRetries)
            {
                _logger.LogError("Failed to connect to RabbitMQ after {MaxRetries} attempts", _maxRetries);
                throw new InvalidOperationException($"Could not connect to RabbitMQ after {_maxRetries} attempts");
            }
        }

        private Task SetupRabbitMqAsync()
        {
            if (_connection == null)
                throw new InvalidOperationException("RabbitMQ connection is not established");

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "ai_insights_queue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            
            _logger.LogInformation("RabbitMQ queue 'ai_insights_queue' declared successfully");
            return Task.CompletedTask;
        }

        private async Task StartConsumingAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not established");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received message: {Message}", json);

                    var evt = JsonSerializer.Deserialize<UserEvent>(json);
                    if (evt != null)
                    {
                        using var scope = _services.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AiDbContext>();

                        // TODO: Replace with real AI analysis
                        var insight = await db.AiInsights.FindAsync(evt.UserId);
                        
                        if (insight == null)
                        {
                            insight = new AiInsight 
                            { 
                                UserId = evt.UserId,
                                SummaryText = $"Auto-generated summary for user {evt.UserId}",
                                PersonalityTags = JsonSerializer.Serialize(new[] { "friendly", "curious" }),
                                CompatibilityScore = new Random().Next(50, 100), // placeholder
                                UpdatedAt = DateTime.UtcNow
                            };
                            db.AiInsights.Add(insight);
                        }
                        else
                        {
                            insight.SummaryText = $"Auto-generated summary for user {evt.UserId}";
                            insight.PersonalityTags = JsonSerializer.Serialize(new[] { "friendly", "curious" });
                            insight.CompatibilityScore = new Random().Next(50, 100); // placeholder
                            insight.UpdatedAt = DateTime.UtcNow;
                        }
                        await db.SaveChangesAsync();
                        
                        _logger.LogInformation("Processed AI insight for user {UserId}", evt.UserId);
                    }

                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel?.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "ai_insights_queue",
                                  autoAck: false,
                                  consumer: consumer);

            _logger.LogInformation("Started consuming messages from 'ai_insights_queue'");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connection");
            }
            base.Dispose();
        }
    }

    public record UserEvent(int UserId, string EventType, string Data);
}
