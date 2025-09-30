using Common.Domain;

namespace Common.Contracts.Events
{
    public class MatchCreatedEvent : IDomainEvent
    {
        public int MatchId { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}