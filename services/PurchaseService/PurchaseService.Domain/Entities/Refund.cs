using Common.Domain;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Domain.Entities;

public class Refund : BaseEntity
{
    public new Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PurchaseId { get; private set; }
    public Money Amount { get; private set; }
    public string? Reason { get; private set; }
    public RefundStatus Status { get; private set; }
    public string? ExternalRefundId { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // Navigation property
    public Purchase Purchase { get; private set; } = null!;

    private Refund()
    {
        Amount = Money.Zero("USD");
        Status = RefundStatus.Pending;
    }

    public Refund(Guid purchaseId, Money amount, string? reason = null, string? externalRefundId = null)
    {
        if (purchaseId == Guid.Empty)
            throw new ArgumentException("Purchase ID cannot be empty", nameof(purchaseId));

        if (amount.IsZero)
            throw new ArgumentException("Refund amount cannot be zero", nameof(amount));

        PurchaseId = purchaseId;
        Amount = amount;
        Reason = reason;
        Status = RefundStatus.Pending;
        ExternalRefundId = externalRefundId;
    }

    public void MarkAsProcessed(string? externalRefundId = null)
    {
        Status = RefundStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(externalRefundId))
        {
            ExternalRefundId = externalRefundId;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string? reason = null)
    {
        Status = RefundStatus.Failed;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Reason = reason;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExternalRefundId(string externalRefundId)
    {
        if (string.IsNullOrWhiteSpace(externalRefundId))
            throw new ArgumentException("External refund ID cannot be null or empty", nameof(externalRefundId));

        ExternalRefundId = externalRefundId;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum RefundStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}