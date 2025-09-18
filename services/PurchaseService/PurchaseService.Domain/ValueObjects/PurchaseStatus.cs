namespace PurchaseService.Domain.ValueObjects;

public class PurchaseStatus : IEquatable<PurchaseStatus>
{
    public PurchaseStatusType Status { get; }
    public string? Reason { get; }
    public DateTime Timestamp { get; }

    public PurchaseStatus(PurchaseStatusType status, string? reason = null, DateTime? timestamp = null)
    {
        Status = status;
        Reason = reason;
        Timestamp = timestamp ?? DateTime.UtcNow;
    }

    // Private constructor for EF Core
    private PurchaseStatus()
    {
        Status = PurchaseStatusType.Pending;
        Reason = null;
        Timestamp = DateTime.UtcNow;
    }

    public static PurchaseStatus Pending() => new(PurchaseStatusType.Pending);
    public static PurchaseStatus Processing() => new(PurchaseStatusType.Processing);
    public static PurchaseStatus Completed() => new(PurchaseStatusType.Completed);
    public static PurchaseStatus Failed(string reason) => new(PurchaseStatusType.Failed, reason);
    public static PurchaseStatus Cancelled(string? reason = null) => new(PurchaseStatusType.Cancelled, reason);
    public static PurchaseStatus Refunded(string? reason = null) => new(PurchaseStatusType.Refunded, reason);

    public bool IsTerminal => Status is PurchaseStatusType.Completed 
        or PurchaseStatusType.Failed 
        or PurchaseStatusType.Cancelled 
        or PurchaseStatusType.Refunded;

    public bool IsSuccessful => Status == PurchaseStatusType.Completed;
    public bool CanBeRefunded => Status == PurchaseStatusType.Completed;
    public bool CanBeCancelled => Status is PurchaseStatusType.Pending or PurchaseStatusType.Processing;

    public PurchaseStatus MoveTo(PurchaseStatusType newStatus, string? reason = null)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");

        return new PurchaseStatus(newStatus, reason);
    }

    private bool CanTransitionTo(PurchaseStatusType newStatus)
    {
        return Status switch
        {
            PurchaseStatusType.Pending => newStatus is PurchaseStatusType.Processing 
                or PurchaseStatusType.Cancelled 
                or PurchaseStatusType.Failed,
            
            PurchaseStatusType.Processing => newStatus is PurchaseStatusType.Completed 
                or PurchaseStatusType.Failed 
                or PurchaseStatusType.Cancelled,
            
            PurchaseStatusType.Completed => newStatus == PurchaseStatusType.Refunded,
            
            PurchaseStatusType.Failed => false,
            PurchaseStatusType.Cancelled => false,
            PurchaseStatusType.Refunded => false,
            
            _ => false
        };
    }

    protected IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
        yield return Reason ?? string.Empty;
        yield return Timestamp;
    }

    public bool Equals(PurchaseStatus? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj) => Equals(obj as PurchaseStatus);

    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(1, (current, obj) => current * 23 + obj.GetHashCode());
    }

    public static bool operator ==(PurchaseStatus? left, PurchaseStatus? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(PurchaseStatus? left, PurchaseStatus? right) => !(left == right);

    public override string ToString() => 
        string.IsNullOrWhiteSpace(Reason) ? Status.ToString() : $"{Status}: {Reason}";
}

public enum PurchaseStatusType
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    Refunded
}