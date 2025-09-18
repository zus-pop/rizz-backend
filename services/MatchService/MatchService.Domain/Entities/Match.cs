using Common.Domain;

namespace MatchService.Domain.Entities
{
    public class Match : BaseEntity
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public DateTime? UnmatchedAt { get; set; }
        public Guid? UnmatchedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        
        // Business logic methods
        public void Unmatch(Guid userId)
        {
            IsActive = false;
            UnmatchedAt = DateTime.UtcNow;
            UnmatchedByUserId = userId;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public bool IsValidMatch()
        {
            return User1Id != User2Id && User1Id != Guid.Empty && User2Id != Guid.Empty;
        }
        
        public bool InvolvesUser(Guid userId)
        {
            return User1Id == userId || User2Id == userId;
        }
    }
}