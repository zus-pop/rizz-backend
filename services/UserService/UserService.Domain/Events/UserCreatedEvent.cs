namespace UserService.Domain.Events
{
    public class UserCreatedEvent : IDomainEvent
    {
        public int UserId { get; }
        public string Email { get; }
        public string FullName { get; }
        public DateTime OccurredOn { get; }

        public UserCreatedEvent(int userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            OccurredOn = DateTime.UtcNow;
        }
    }
}