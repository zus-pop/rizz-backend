using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModerationService.Domain.Entities;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Infrastructure.Configurations;

public class ModerationCaseConfiguration : IEntityTypeConfiguration<ModerationCase>
{
    public void Configure(EntityTypeBuilder<ModerationCase> builder)
    {
        builder.HasKey(mc => mc.Id);

        builder.Property(mc => mc.Id)
            .ValueGeneratedNever();

        builder.Property(mc => mc.TargetUserId)
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v))
            .HasColumnName("TargetUserId")
            .IsRequired();

        builder.Property(mc => mc.ReportIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(Guid.Parse)
                     .ToList())
            .HasColumnName("ReportIds");

        builder.Property(mc => mc.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(mc => mc.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(mc => mc.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(mc => mc.AssignedAt);

        builder.Property(mc => mc.AssignedTo)
            .HasConversion(
                v => v != null ? v.Value : (int?)null,
                v => v != null ? UserId.Create(v.Value) : null)
            .HasColumnName("AssignedTo");

        builder.Property(mc => mc.ResolvedAt);

        builder.Property(mc => mc.FinalAction)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? ModerationAction.Create(v, 0) : null)
            .HasColumnName("FinalAction")
            .HasMaxLength(100);

        builder.Property(mc => mc.Resolution)
            .HasMaxLength(1000);

        // Indexes for performance
        builder.HasIndex(mc => mc.TargetUserId)
            .HasDatabaseName("IX_ModerationCases_TargetUserId");

        builder.HasIndex(mc => mc.Status)
            .HasDatabaseName("IX_ModerationCases_Status");

        builder.HasIndex(mc => mc.AssignedTo)
            .HasDatabaseName("IX_ModerationCases_AssignedTo");

        builder.HasIndex(mc => mc.Priority)
            .HasDatabaseName("IX_ModerationCases_Priority");

        builder.ToTable("ModerationCases");
    }
}