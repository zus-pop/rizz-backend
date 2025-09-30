using MediatR;
using MessagingService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace MessagingService.Application.EventHandlers
{
    public class MessageSentEventHandler : INotificationHandler<MessageSentEvent>
    {
        private readonly ILogger<MessageSentEventHandler> _logger;

        public MessageSentEventHandler(ILogger<MessageSentEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(MessageSentEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Message {MessageId} sent event processed for AI Insights and Moderation", notification.MessageId);
                
                // Note: RabbitMQ publishing will be handled at the API layer
                // This handler processes the domain event within the application layer
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MessageSentEvent for message {MessageId}", notification.MessageId);
            }
        }
    }

    public class MessageReadEventHandler : INotificationHandler<MessageReadEvent>
    {
        private readonly ILogger<MessageReadEventHandler> _logger;

        public MessageReadEventHandler(ILogger<MessageReadEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(MessageReadEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Message {MessageId} read event processed", notification.MessageId);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MessageReadEvent for message {MessageId}", notification.MessageId);
            }
        }
    }
}