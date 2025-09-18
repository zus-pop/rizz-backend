namespace PurchaseService.Domain.ValueObjects;

public class SubscriptionPeriod : IEquatable<SubscriptionPeriod>
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public SubscriptionType Type { get; }
    public int Duration { get; }

    public SubscriptionPeriod(DateTime startDate, SubscriptionType type, int duration)
    {
        if (duration <= 0)
            throw new ArgumentException("Duration must be positive", nameof(duration));

        StartDate = startDate;
        Type = type;
        Duration = duration;
        EndDate = CalculateEndDate(startDate, type, duration);
    }

    public SubscriptionPeriod(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date");

        StartDate = startDate;
        EndDate = endDate;
        Type = SubscriptionType.Custom;
        Duration = (endDate - startDate).Days;
    }

    private static DateTime CalculateEndDate(DateTime startDate, SubscriptionType type, int duration)
    {
        return type switch
        {
            SubscriptionType.Daily => startDate.AddDays(duration),
            SubscriptionType.Weekly => startDate.AddDays(duration * 7),
            SubscriptionType.Monthly => startDate.AddMonths(duration),
            SubscriptionType.Yearly => startDate.AddYears(duration),
            SubscriptionType.Custom => startDate.AddDays(duration),
            _ => throw new ArgumentException($"Unknown subscription type: {type}")
        };
    }

    public static SubscriptionPeriod Daily(DateTime startDate, int days) 
        => new(startDate, SubscriptionType.Daily, days);

    public static SubscriptionPeriod Weekly(DateTime startDate, int weeks)
        => new(startDate, SubscriptionType.Weekly, weeks);

    public static SubscriptionPeriod Monthly(DateTime startDate, int months)
        => new(startDate, SubscriptionType.Monthly, months);

    public static SubscriptionPeriod Yearly(DateTime startDate, int years)
        => new(startDate, SubscriptionType.Yearly, years);

    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    public bool IsExpired => DateTime.UtcNow > EndDate;
    public bool IsUpcoming => DateTime.UtcNow < StartDate;

    public TimeSpan TotalDuration => EndDate - StartDate;
    public TimeSpan RemainingTime => IsActive ? EndDate - DateTime.UtcNow : TimeSpan.Zero;

    public SubscriptionPeriod Extend(int additionalDuration)
    {
        return new SubscriptionPeriod(StartDate, Type, Duration + additionalDuration);
    }

    protected IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
        yield return Type;
        yield return Duration;
    }

    public bool Equals(SubscriptionPeriod? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj) => Equals(obj as SubscriptionPeriod);

    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(1, (current, obj) => current * 23 + obj.GetHashCode());
    }

    public static bool operator ==(SubscriptionPeriod? left, SubscriptionPeriod? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(SubscriptionPeriod? left, SubscriptionPeriod? right) => !(left == right);

    public override string ToString() => 
        $"{Type} subscription: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} ({Duration} {Type.ToString().ToLower()})";
}

public enum SubscriptionType
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
    Custom
}