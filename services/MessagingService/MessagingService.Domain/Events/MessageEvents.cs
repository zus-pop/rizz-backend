using Common.Domain;

namespace MessagingService.Domain.Events
{
    public class MessageSentEvent : IDomainEvent
    {
        public int MessageId { get; }
        public int MatchId { get; }
        public int SenderId { get; }
        public string Content { get; }
        public DateTime SentAt { get; }
        public DateTime OccurredOn { get; }

        public MessageSentEvent(int messageId, int matchId, int senderId, string content, DateTime sentAt)
        {
            MessageId = messageId;
            MatchId = matchId;
            SenderId = senderId;
            Content = content;
            SentAt = sentAt;
            OccurredOn = DateTime.UtcNow;
        }
    }

    public class MessageReadEvent : IDomainEvent
    {
        public int MessageId { get; }
        public int MatchId { get; }
        public int ReadById { get; }
        public DateTime ReadAt { get; }
        public DateTime OccurredOn { get; }

        public MessageReadEvent(int messageId, int matchId, int readById, DateTime readAt)
        {
            MessageId = messageId;
            MatchId = matchId;
            ReadById = readById;
            ReadAt = readAt;
            OccurredOn = DateTime.UtcNow;
        }
    }
}