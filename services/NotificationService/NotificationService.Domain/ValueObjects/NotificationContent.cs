namespace NotificationService.Domain.ValueObjects;

public class NotificationContent : IEquatable<NotificationContent>
{
    public string Title { get; }
    public string Body { get; }
    public Dictionary<string, string> Data { get; }

    public NotificationContent(string title, string body, Dictionary<string, string>? data = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be null or empty", nameof(body));

        if (title.Length > 100)
            throw new ArgumentException("Title cannot exceed 100 characters", nameof(title));

        if (body.Length > 1000)
            throw new ArgumentException("Body cannot exceed 1000 characters", nameof(body));

        Title = title.Trim();
        Body = body.Trim();
        Data = data ?? new Dictionary<string, string>();
    }

    public static NotificationContent Create(string title, string body, Dictionary<string, string>? data = null)
    {
        return new NotificationContent(title, body, data);
    }

    public NotificationContent WithData(string key, string value)
    {
        var newData = new Dictionary<string, string>(Data) { [key] = value };
        return new NotificationContent(Title, Body, newData);
    }

    public NotificationContent WithData(Dictionary<string, string> additionalData)
    {
        var newData = new Dictionary<string, string>(Data);
        foreach (var kvp in additionalData)
        {
            newData[kvp.Key] = kvp.Value;
        }
        return new NotificationContent(Title, Body, newData);
    }

    public bool HasData => Data.Count > 0;
    public bool IsLongContent => Body.Length > 200;

    public bool Equals(NotificationContent? other)
    {
        return other is not null &&
               Title == other.Title &&
               Body == other.Body &&
               Data.SequenceEqual(other.Data);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as NotificationContent);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Body, Data.Count);
    }

    public override string ToString() => $"{Title}: {Body}";
}