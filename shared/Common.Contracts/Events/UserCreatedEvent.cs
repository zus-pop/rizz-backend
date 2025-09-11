using Common.Domain;

namespace Common.Contracts.Events
{
    public class UserCreatedEvent : IDomainEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}
