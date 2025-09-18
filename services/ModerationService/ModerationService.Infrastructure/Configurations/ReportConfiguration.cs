using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModerationService.Domain.Entities;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Infrastructure.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.ReporterId)
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v))
            .HasColumnName("ReporterId")
            .IsRequired();

        builder.Property(r => r.ReportedUserId)
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v))
            .HasColumnName("ReportedUserId")
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasConversion(
                v => v.Value,
                v => ReportReason.Create(v))
            .HasColumnName("Reason")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(r => r.ReviewedAt);

        builder.Property(r => r.ReviewedBy)
            .HasConversion(
                v => v != null ? v.Value : (int?)null,
                v => v != null ? UserId.Create(v.Value) : null)
            .HasColumnName("ReviewedBy");

        builder.Property(r => r.Action)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? ModerationAction.Create(v, 0) : null)
            .HasColumnName("Action")
            .HasMaxLength(100);

        builder.Property(r => r.ReviewNotes)
            .HasMaxLength(1000);

        // Indexes for performance
        builder.HasIndex(r => r.ReportedUserId)
            .HasDatabaseName("IX_Reports_ReportedUserId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reports_Status");

        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_Reports_CreatedAt");

        builder.ToTable("Reports");
    }
}