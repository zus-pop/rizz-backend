namespace UserService.Domain.Events
{
    public class UserVerifiedEvent : IDomainEvent
    {
        public int UserId { get; }
        public string Email { get; }
        public DateTime OccurredOn { get; }

        public UserVerifiedEvent(int userId, string email)
        {
            UserId = userId;
            Email = email;
            OccurredOn = DateTime.UtcNow;
        }
    }
}