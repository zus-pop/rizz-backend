namespace AiInsightsService.Domain.ValueObjects;

public record CompatibilityScore
{
    public float Value { get; }

    public CompatibilityScore(float value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Compatibility score must be between 0 and 100", nameof(value));
        
        Value = value;
    }

    public static implicit operator float(CompatibilityScore score) => score.Value;
    public static implicit operator CompatibilityScore(float value) => new(value);

    public bool IsHighCompatibility() => Value >= 80;
    public bool IsModerateCompatibility() => Value >= 50 && Value < 80;
    public bool IsLowCompatibility() => Value < 50;

    public string GetCompatibilityLevel() => Value switch
    {
        >= 90 => "Excellent",
        >= 80 => "Very Good",
        >= 70 => "Good", 
        >= 50 => "Average",
        >= 30 => "Below Average",
        _ => "Poor"
    };
}