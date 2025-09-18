using Common.Domain;

namespace MatchService.Domain.Events
{
    public class SwipePerformedEvent : IDomainEvent
    {
        public int SwipeId { get; }
        public int SwiperId { get; }
        public int SwipeeId { get; }
        public string Direction { get; }
        public DateTime SwipedAt { get; }
        public DateTime OccurredOn { get; }

        public SwipePerformedEvent(int swipeId, int swiperId, int swipeeId, string direction, DateTime swipedAt)
        {
            SwipeId = swipeId;
            SwiperId = swiperId;
            SwipeeId = swipeeId;
            Direction = direction;
            SwipedAt = swipedAt;
            OccurredOn = DateTime.UtcNow;
        }
    }

    public class MatchCreatedEvent : IDomainEvent
    {
        public int MatchId { get; }
        public int User1Id { get; }
        public int User2Id { get; }
        public DateTime MatchedAt { get; }
        public DateTime OccurredOn { get; }

        public MatchCreatedEvent(int matchId, int user1Id, int user2Id, DateTime matchedAt)
        {
            MatchId = matchId;
            User1Id = user1Id;
            User2Id = user2Id;
            MatchedAt = matchedAt;
            OccurredOn = DateTime.UtcNow;
        }
    }

    public class MatchUnmatchedEvent : IDomainEvent
    {
        public int MatchId { get; }
        public int User1Id { get; }
        public int User2Id { get; }
        public DateTime UnmatchedAt { get; }
        public DateTime OccurredOn { get; }

        public MatchUnmatchedEvent(int matchId, int user1Id, int user2Id, DateTime unmatchedAt)
        {
            MatchId = matchId;
            User1Id = user1Id;
            User2Id = user2Id;
            UnmatchedAt = unmatchedAt;
            OccurredOn = DateTime.UtcNow;
        }
    }
}