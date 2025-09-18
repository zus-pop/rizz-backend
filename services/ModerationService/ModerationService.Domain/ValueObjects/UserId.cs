namespace ModerationService.Domain.ValueObjects;

public class UserId : IEquatable<UserId>
{
    public int Value { get; }

    private UserId(int value)
    {
        Value = value;
    }

    public static UserId Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("User ID must be greater than zero", nameof(value));

        return new UserId(value);
    }

    public bool Equals(UserId? other)
    {
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as UserId);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(UserId userId) => userId.Value;
    public static explicit operator UserId(int value) => Create(value);
}