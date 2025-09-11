using Common.Domain;

namespace Common.Contracts.Events
{
    public class MatchFoundEvent : IDomainEvent
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public int MatchId { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}
