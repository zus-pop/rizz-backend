namespace PurchaseService.Domain.ValueObjects;

public class PaymentMethod : IEquatable<PaymentMethod>
{
    public PaymentMethodType Type { get; }
    public string Provider { get; }
    public string? ExternalTransactionId { get; }
    public Dictionary<string, string> Metadata { get; }

    public PaymentMethod(PaymentMethodType type, string provider, string? externalTransactionId = null, Dictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider cannot be null or empty", nameof(provider));

        Type = type;
        Provider = provider;
        ExternalTransactionId = externalTransactionId;
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    public static PaymentMethod CreditCard(string provider, string externalTransactionId) 
        => new(PaymentMethodType.CreditCard, provider, externalTransactionId);

    public static PaymentMethod MobileMoney(string provider, string externalTransactionId)
        => new(PaymentMethodType.MobileMoney, provider, externalTransactionId);

    public static PaymentMethod BankTransfer(string provider, string externalTransactionId)
        => new(PaymentMethodType.BankTransfer, provider, externalTransactionId);

    public static PaymentMethod Wallet(string provider, string externalTransactionId)
        => new(PaymentMethodType.DigitalWallet, provider, externalTransactionId);

    public PaymentMethod WithMetadata(string key, string value)
    {
        var newMetadata = new Dictionary<string, string>(Metadata) { [key] = value };
        return new PaymentMethod(Type, Provider, ExternalTransactionId, newMetadata);
    }

    protected IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Provider;
        yield return ExternalTransactionId ?? string.Empty;
        
        foreach (var kvp in Metadata.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }

    public bool Equals(PaymentMethod? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj) => Equals(obj as PaymentMethod);

    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(1, (current, obj) => current * 23 + obj.GetHashCode());
    }

    public static bool operator ==(PaymentMethod? left, PaymentMethod? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(PaymentMethod? left, PaymentMethod? right) => !(left == right);

    public override string ToString() => $"{Type} via {Provider}";
}

public enum PaymentMethodType
{
    CreditCard,
    MobileMoney,
    BankTransfer,
    DigitalWallet,
    Cryptocurrency
}