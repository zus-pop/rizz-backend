namespace ModerationService.Domain.ValueObjects;

public class ReportReason : IEquatable<ReportReason>
{
    public string Value { get; }

    private ReportReason(string value)
    {
        Value = value;
    }

    // Common report reasons
    public static ReportReason Spam => new("Spam");
    public static ReportReason InappropriateContent => new("Inappropriate Content");
    public static ReportReason Harassment => new("Harassment");
    public static ReportReason FakeProfile => new("Fake Profile");
    public static ReportReason UnderAge => new("Under Age");
    public static ReportReason Violence => new("Violence");
    public static ReportReason HateSpeech => new("Hate Speech");
    public static ReportReason Impersonation => new("Impersonation");
    public static ReportReason IntellectualProperty => new("Intellectual Property");
    public static ReportReason Other => new("Other");

    public static ReportReason Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Report reason cannot be null or empty", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Report reason cannot exceed 100 characters", nameof(value));

        return new ReportReason(value.Trim());
    }

    public bool IsSerious => Value is "Harassment" or "Violence" or "HateSpeech" or "UnderAge";
    public bool RequiresImmediateAction => Value is "Violence" or "UnderAge" or "HateSpeech";

    public bool Equals(ReportReason? other)
    {
        return other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReportReason);
    }

    public override int GetHashCode()
    {
        return Value.ToLowerInvariant().GetHashCode();
    }

    public override string ToString() => Value;
}