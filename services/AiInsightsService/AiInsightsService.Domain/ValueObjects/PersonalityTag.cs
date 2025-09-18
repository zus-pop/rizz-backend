namespace AiInsightsService.Domain.ValueObjects;

public record PersonalityTag
{
    public string Value { get; }

    public PersonalityTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Personality tag cannot be empty", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Personality tag cannot exceed 50 characters", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public static implicit operator string(PersonalityTag tag) => tag.Value;
    public static implicit operator PersonalityTag(string value) => new(value);

    public bool IsEmotionalTag() => Value.Contains("emotional") || Value.Contains("feeling") || Value.Contains("sensitive");
    public bool IsIntellectualTag() => Value.Contains("smart") || Value.Contains("intelligent") || Value.Contains("analytical");
    public bool IsSocialTag() => Value.Contains("social") || Value.Contains("outgoing") || Value.Contains("friendly");

    public override string ToString() => Value;
}