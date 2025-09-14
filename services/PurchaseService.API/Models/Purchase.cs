namespace PurchaseService.API.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "pending"; // pending, completed, failed
    }
}
