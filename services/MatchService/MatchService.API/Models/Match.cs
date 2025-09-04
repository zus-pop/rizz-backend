namespace MatchService.API.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    }
}
