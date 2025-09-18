using Common.Domain;
using MatchService.Domain.Entities;

namespace MatchService.Domain.Entities
{
    public class Swipe : BaseEntity
    {
        public Guid SwiperId { get; set; }
        public Guid TargetUserId { get; set; }
        public SwipeDirection Direction { get; set; }
        public DateTime SwipedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        
        // Business logic methods
        public bool IsLike()
        {
            return Direction == SwipeDirection.Like || Direction == SwipeDirection.SuperLike;
        }
        
        public bool IsPass()
        {
            return Direction == SwipeDirection.Pass;
        }
        
        public bool IsSuperLike()
        {
            return Direction == SwipeDirection.SuperLike;
        }
        
        public bool IsValidSwipe()
        {
            return SwiperId != TargetUserId && SwiperId != Guid.Empty && TargetUserId != Guid.Empty;
        }
    }
}