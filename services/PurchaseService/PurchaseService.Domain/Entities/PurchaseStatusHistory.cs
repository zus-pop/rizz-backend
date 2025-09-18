using Common.Domain;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Domain.Entities;

public class PurchaseStatusHistory : BaseEntity
{
    public new Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PurchaseId { get; private set; }
    public PurchaseStatus Status { get; private set; }
    public DateTime ChangedAt { get; private set; }

    // Navigation property
    public Purchase Purchase { get; private set; } = null!;

    private PurchaseStatusHistory()
    {
        Status = PurchaseStatus.Pending();
    }

    public PurchaseStatusHistory(Guid purchaseId, PurchaseStatus status)
    {
        if (purchaseId == Guid.Empty)
            throw new ArgumentException("Purchase ID cannot be empty", nameof(purchaseId));

        PurchaseId = purchaseId;
        Status = status;
        ChangedAt = DateTime.UtcNow;
    }
}