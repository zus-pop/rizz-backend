using Common.Domain;

namespace Common.Contracts.Events
{
    public class PurchaseCompletedEvent : IDomainEvent
    {
        public int UserId { get; set; }
        public int PurchaseId { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}