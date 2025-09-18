using Common.Domain;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Domain.Entities;

public class Purchase : BaseEntity
{
    public new Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PurchaseStatus Status { get; private set; }
    public SubscriptionPeriod? SubscriptionPeriod { get; private set; }
    public string? ProductId { get; private set; }
    public string? ProductName { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; }
    
    // Navigation properties for related entities
    public List<PurchaseStatusHistory> StatusHistory { get; private set; } = new();
    public Refund? Refund { get; private set; }

    // Private constructor for EF Core
    private Purchase() 
    {
        UserId = string.Empty;
        Amount = Money.Zero("USD");
        PaymentMethod = PaymentMethod.CreditCard("unknown", string.Empty);
        Status = PurchaseStatus.Pending();
        Metadata = new Dictionary<string, string>();
    }

    public Purchase(
        string userId,
        Money amount,
        PaymentMethod paymentMethod,
        string? productId = null,
        string? productName = null,
        SubscriptionPeriod? subscriptionPeriod = null,
        Dictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        if (amount.IsZero)
            throw new ArgumentException("Purchase amount cannot be zero", nameof(amount));

        UserId = userId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = PurchaseStatus.Pending();
        SubscriptionPeriod = subscriptionPeriod;
        ProductId = productId;
        ProductName = productName;
        Metadata = metadata ?? new Dictionary<string, string>();

        AddStatusToHistory(Status);
    }

    public void StartProcessing()
    {
        if (!Status.CanBeCancelled)
            throw new InvalidOperationException($"Cannot start processing purchase in status: {Status.Status}");

        ChangeStatus(PurchaseStatus.Processing());
    }

    public void Complete()
    {
        if (Status.Status != PurchaseStatusType.Processing)
            throw new InvalidOperationException($"Cannot complete purchase in status: {Status.Status}");

        ChangeStatus(PurchaseStatus.Completed());
    }

    public void Fail(string reason)
    {
        if (Status.IsTerminal && Status.Status != PurchaseStatusType.Processing)
            throw new InvalidOperationException($"Cannot fail purchase in status: {Status.Status}");

        ChangeStatus(PurchaseStatus.Failed(reason));
    }

    public void Cancel(string? reason = null)
    {
        if (!Status.CanBeCancelled)
            throw new InvalidOperationException($"Cannot cancel purchase in status: {Status.Status}");

        ChangeStatus(PurchaseStatus.Cancelled(reason));
    }

    public void ProcessRefund(Money refundAmount, string? reason = null)
    {
        if (!Status.CanBeRefunded)
            throw new InvalidOperationException($"Cannot refund purchase in status: {Status.Status}");

        if (refundAmount.IsGreaterThan(Amount))
            throw new InvalidOperationException("Refund amount cannot be greater than purchase amount");

        if (refundAmount.Currency != Amount.Currency)
            throw new InvalidOperationException("Refund currency must match purchase currency");

        Refund = new Refund(Id, refundAmount, reason);
        ChangeStatus(PurchaseStatus.Refunded(reason));
    }

    public void UpdatePaymentMethod(PaymentMethod newPaymentMethod)
    {
        if (Status.Status != PurchaseStatusType.Pending)
            throw new InvalidOperationException("Cannot update payment method after purchase has started processing");

        PaymentMethod = newPaymentMethod;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));

        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }

    public string? GetMetadata(string key) => Metadata.TryGetValue(key, out var value) ? value : null;

    public bool IsSubscription => SubscriptionPeriod != null;
    public bool IsActive => IsSubscription && SubscriptionPeriod!.IsActive && Status.IsSuccessful;
    public bool IsExpired => IsSubscription && SubscriptionPeriod!.IsExpired;

    public TimeSpan? GetRemainingSubscriptionTime() => SubscriptionPeriod?.RemainingTime;

    private void ChangeStatus(PurchaseStatus newStatus)
    {
        Status = newStatus;
        AddStatusToHistory(newStatus);
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddStatusToHistory(PurchaseStatus status)
    {
        StatusHistory.Add(new PurchaseStatusHistory(Id, status));
    }
}