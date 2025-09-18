namespace NotificationService.Domain.ValueObjects;

public class NotificationType : IEquatable<NotificationType>
{
    public string Value { get; }

    private NotificationType(string value)
    {
        Value = value;
    }

    // Common notification types
    public static NotificationType Match => new("Match");
    public static NotificationType Message => new("Message");
    public static NotificationType Like => new("Like");
    public static NotificationType SuperLike => new("SuperLike");
    public static NotificationType Purchase => new("Purchase");
    public static NotificationType Subscription => new("Subscription");
    public static NotificationType Security => new("Security");
    public static NotificationType System => new("System");
    public static NotificationType Promotion => new("Promotion");

    public static NotificationType Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Notification type cannot be null or empty", nameof(value));

        return new NotificationType(value.Trim());
    }

    public static NotificationType FromString(string value) => Create(value);

    public bool IsUserInteraction => Value is "Match" or "Message" or "Like" or "SuperLike";
    public bool IsSystem => Value is "System" or "Security";
    public bool IsCommercial => Value is "Purchase" or "Subscription" or "Promotion";

    public bool Equals(NotificationType? other)
    {
        return other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as NotificationType);
    }

    public override int GetHashCode()
    {
        return Value.ToLowerInvariant().GetHashCode();
    }

    public override string ToString() => Value;

    public static implicit operator string(NotificationType type) => type.Value;
    public static implicit operator NotificationType(string value) => Create(value);
}