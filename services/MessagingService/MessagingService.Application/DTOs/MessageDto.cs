namespace MessagingService.Application.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? ExtraData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class CreateMessageDto
    {
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "text";
        public string? ExtraData { get; set; }
    }

    public class UpdateMessageDto
    {
        public string? Content { get; set; }
        public string? ExtraData { get; set; }
    }
}