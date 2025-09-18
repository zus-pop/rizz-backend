using Common.Domain;

namespace MatchService.API.Models
{
    public class Match : BaseEntity
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    }
}
