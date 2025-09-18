using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModerationService.Domain.Entities;
using ModerationService.Domain.ValueObjects;

namespace ModerationService.Infrastructure.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.BlockerId)
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v))
            .HasColumnName("BlockerId")
            .IsRequired();

        builder.Property(b => b.BlockedUserId)
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v))
            .HasColumnName("BlockedUserId")
            .IsRequired();

        builder.Property(b => b.Reason)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(b => b.RevokedAt);

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        // Index for performance
        builder.HasIndex(b => new { b.BlockerId, b.BlockedUserId, b.IsActive })
            .HasDatabaseName("IX_Blocks_BlockerId_BlockedUserId_IsActive");

        builder.HasIndex(b => b.BlockedUserId)
            .HasDatabaseName("IX_Blocks_BlockedUserId");

        builder.ToTable("Blocks");
    }
}