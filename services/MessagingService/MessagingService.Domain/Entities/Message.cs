using Common.Domain;
using MessagingService.Domain.Enums;

namespace MessagingService.Domain.Entities
{
    public class Message : BaseEntity
    {
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public string? ExtraData { get; set; } // optional metadata JSON
        public DateTime? ReadAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        // Navigation properties (will be handled by EF Core configurations)
        // public virtual Match? Match { get; set; }
        // public virtual User? Sender { get; set; }
        
        public void MarkAsRead()
        {
            if (ReadAt == null)
            {
                ReadAt = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateContent(string newContent)
        {
            Content = newContent;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}