namespace MatchService.Application.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public DateTime MatchedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UnmatchedAt { get; set; }
        public Guid? UnmatchedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SwipeDto
    {
        public int Id { get; set; }
        public Guid SwiperId { get; set; }
        public Guid TargetUserId { get; set; }
        public string Direction { get; set; } = string.Empty;
        public DateTime SwipedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateSwipeDto
    {
        public Guid SwiperId { get; set; }
        public Guid TargetUserId { get; set; }
        public string Direction { get; set; } = string.Empty;
    }

    public class CreateMatchDto
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
    }

    public class MatchSummaryDto
    {
        public int TotalMatches { get; set; }
        public int ActiveMatches { get; set; }
        public IEnumerable<MatchDto> RecentMatches { get; set; } = new List<MatchDto>();
    }
}