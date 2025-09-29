using MediatR;
using MessagingService.Domain.Events;
using MessagingService.API.Services;
using Microsoft.AspNetCore.SignalR;
using MessagingService.API.Hubs;
using Microsoft.Extensions.Logging;

namespace MessagingService.API.EventHandlers
{
    public class MessageEventPublisher : INotificationHandler<MessageSentEvent>, INotificationHandler<MessageReadEvent>
    {
        private readonly RabbitMqPublisher _rabbitMqPublisher;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<MessageEventPublisher> _logger;

        public MessageEventPublisher(
            RabbitMqPublisher rabbitMqPublisher,
            IHubContext<ChatHub> hubContext,
            ILogger<MessageEventPublisher> logger)
        {
            _rabbitMqPublisher = rabbitMqPublisher;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(MessageSentEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Publish to RabbitMQ for AI Insights and Moderation
                var messageEvent = new
                {
                    MessageId = notification.MessageId,
                    MatchId = notification.MatchId,
                    SenderId = notification.SenderId,
                    Content = notification.Content,
                    SentAt = notification.SentAt,
                    EventType = "MessageSent"
                };

                await _rabbitMqPublisher.PublishAsync("message.events", messageEvent);
                await _rabbitMqPublisher.PublishAsync("ai.insights.queue", messageEvent);
                await _rabbitMqPublisher.PublishAsync("moderation.queue", messageEvent);

                // Send real-time notification via SignalR
                await _hubContext.Clients.Group($"match_{notification.MatchId}")
                    .SendAsync("NewMessage", new
                    {
                        messageId = notification.MessageId,
                        matchId = notification.MatchId,
                        senderId = notification.SenderId,
                        content = notification.Content,
                        sentAt = notification.SentAt
                    }, cancellationToken);

                _logger.LogInformation("Message {MessageId} events published successfully", notification.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish events for message {MessageId}", notification.MessageId);
            }
        }

        public async Task Handle(MessageReadEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Send read receipt via SignalR
                await _hubContext.Clients.Group($"match_{notification.MessageId}")
                    .SendAsync("MessageRead", new
                    {
                        messageId = notification.MessageId,
                        readById = notification.ReadById,
                        readAt = notification.ReadAt
                    }, cancellationToken);

                _logger.LogInformation("Message {MessageId} read receipt sent", notification.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send read receipt for message {MessageId}", notification.MessageId);
            }
        }
    }
}