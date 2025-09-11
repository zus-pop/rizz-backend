using Common.Domain;

namespace Common.Contracts.Events
{
    public class MessageSentEvent : IDomainEvent
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int MatchId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}
