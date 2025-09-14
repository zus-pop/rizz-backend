namespace MessagingService.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "text"; // text, image, voice, sticker
        public string? ExtraData { get; set; } // optional metadata JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
    }
}
