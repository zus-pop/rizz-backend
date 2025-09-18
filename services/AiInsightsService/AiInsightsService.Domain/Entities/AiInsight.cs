using System.Text.Json;

namespace AiInsightsService.Domain.Entities;

public class AiInsight
{
    public int UserId { get; private set; } // Primary key, matches user from other services
    public string SummaryText { get; private set; } = string.Empty;
    public float CompatibilityScore { get; private set; }
    public List<string> PersonalityTags { get; private set; } = new();
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private AiInsight() { }

    public AiInsight(int userId, string summaryText, float compatibilityScore, List<string>? personalityTags = null)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(summaryText))
            throw new ArgumentException("Summary text cannot be empty", nameof(summaryText));

        if (compatibilityScore < 0 || compatibilityScore > 100)
            throw new ArgumentException("Compatibility score must be between 0 and 100", nameof(compatibilityScore));

        UserId = userId;
        SummaryText = summaryText;
        CompatibilityScore = compatibilityScore;
        PersonalityTags = personalityTags ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInsight(string summaryText, float compatibilityScore, List<string>? personalityTags = null)
    {
        if (string.IsNullOrWhiteSpace(summaryText))
            throw new ArgumentException("Summary text cannot be empty", nameof(summaryText));

        if (compatibilityScore < 0 || compatibilityScore > 100)
            throw new ArgumentException("Compatibility score must be between 0 and 100", nameof(compatibilityScore));

        SummaryText = summaryText;
        CompatibilityScore = compatibilityScore;
        PersonalityTags = personalityTags ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPersonalityTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag cannot be empty", nameof(tag));

        if (!PersonalityTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            PersonalityTags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemovePersonalityTag(string tag)
    {
        if (PersonalityTags.RemoveAll(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)) > 0)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public bool HasPersonalityTag(string tag) =>
        PersonalityTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));

    public string GetPersonalityTagsAsJson() =>
        JsonSerializer.Serialize(PersonalityTags);

    public void SetPersonalityTagsFromJson(string json)
    {
        PersonalityTags = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
    }
}