namespace MatchService.API.Models
{
    public class Swipe
    {
        public int Id { get; set; }
        public int SwiperId { get; set; }
        public int SwipeeId { get; set; }
        public string Direction { get; set; } // left, right, superlike
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
