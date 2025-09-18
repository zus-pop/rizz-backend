namespace ModerationService.Domain.ValueObjects;

public class ModerationAction : IEquatable<ModerationAction>
{
    public string Value { get; }
    public int Severity { get; }

    private ModerationAction(string value, int severity)
    {
        Value = value;
        Severity = severity;
    }

    public static ModerationAction NoAction => new("No Action", 0);
    public static ModerationAction Warning => new("Warning", 1);
    public static ModerationAction TemporaryRestriction => new("Temporary Restriction", 2);
    public static ModerationAction ContentRemoval => new("Content Removal", 3);
    public static ModerationAction AccountSuspension => new("Account Suspension", 4);
    public static ModerationAction PermanentBan => new("Permanent Ban", 5);

    public static ModerationAction Create(string value, int severity)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Moderation action cannot be null or empty", nameof(value));

        if (severity < 0 || severity > 5)
            throw new ArgumentException("Severity must be between 0 and 5", nameof(severity));

        return new ModerationAction(value.Trim(), severity);
    }

    public bool IsActionRequired => Severity > 0;
    public bool IsSerious => Severity >= 3;
    public bool IsPermanent => Severity == 5;

    public bool Equals(ModerationAction? other)
    {
        return other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ModerationAction);
    }

    public override int GetHashCode()
    {
        return Value.ToLowerInvariant().GetHashCode();
    }

    public override string ToString() => Value;
}