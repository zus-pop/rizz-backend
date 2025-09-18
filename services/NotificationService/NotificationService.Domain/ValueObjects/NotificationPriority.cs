namespace NotificationService.Domain.ValueObjects;

public class NotificationPriority : IEquatable<NotificationPriority>
{
    public string Value { get; }
    public int Level { get; }

    private NotificationPriority(string value, int level)
    {
        Value = value;
        Level = level;
    }

    public static NotificationPriority Low => new("Low", 1);
    public static NotificationPriority Normal => new("Normal", 2);
    public static NotificationPriority High => new("High", 3);
    public static NotificationPriority Urgent => new("Urgent", 4);
    public static NotificationPriority Critical => new("Critical", 5);

    public static NotificationPriority Create(string value)
    {
        return value?.ToLowerInvariant() switch
        {
            "low" => Low,
            "normal" => Normal,
            "high" => High,
            "urgent" => Urgent,
            "critical" => Critical,
            _ => Normal // Default to normal priority
        };
    }

    public static NotificationPriority FromString(string value) => Create(value);

    public bool IsHighPriority => Level >= High.Level;
    public bool RequiresImmediateDelivery => Level >= Urgent.Level;

    public bool Equals(NotificationPriority? other)
    {
        return other is not null && Level == other.Level;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as NotificationPriority);
    }

    public override int GetHashCode()
    {
        return Level.GetHashCode();
    }

    public override string ToString() => Value;

    public static implicit operator string(NotificationPriority priority) => priority.Value;
    public static implicit operator NotificationPriority(string value) => Create(value);
}