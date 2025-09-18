using Microsoft.EntityFrameworkCore;
using AiInsightsService.Domain.Entities;
using System.Text.Json;

namespace AiInsightsService.Infrastructure.Data;

public class AiInsightsDbContext : DbContext
{
    public AiInsightsDbContext(DbContextOptions<AiInsightsDbContext> options) : base(options) { }

    public DbSet<AiInsight> AiInsights { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiInsight>(entity =>
        {
            entity.HasKey(ai => ai.UserId);
            
            entity.Property(ai => ai.UserId)
                .IsRequired();

            entity.Property(ai => ai.SummaryText)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(ai => ai.CompatibilityScore)
                .IsRequired()
                .HasPrecision(5, 2); // Allows values like 99.99

            entity.Property(ai => ai.UpdatedAt)
                .IsRequired();

            // Convert PersonalityTags list to JSON string for storage
            entity.Property(ai => ai.PersonalityTags)
                .HasConversion(
                    tags => JsonSerializer.Serialize(tags, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null) ?? new List<string>()
                )
                .HasColumnType("text");

            // Index for faster queries
            entity.HasIndex(ai => ai.CompatibilityScore);
            entity.HasIndex(ai => ai.UpdatedAt);
        });

        base.OnModelCreating(modelBuilder);
    }
}