using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;
using System.Text.Json;

namespace NotificationService.Infrastructure.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Configure NotificationType as conversion
        builder.Property(x => x.NotificationType)
            .HasConversion(
                v => v.Value,
                v => NotificationType.FromString(v))
            .HasColumnName("NotificationType")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TitleTemplate)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.BodyTemplate)
            .HasMaxLength(5000)
            .IsRequired();

        // Configure NotificationPriority as conversion
        builder.Property(x => x.DefaultPriority)
            .HasConversion(
                v => v.Value,
                v => NotificationPriority.FromString(v))
            .HasColumnName("DefaultPriority")
            .HasMaxLength(20)
            .IsRequired();

        // Configure DefaultChannels as JSON
        builder.Property(x => x.DefaultChannels)
            .HasConversion(
                v => JsonSerializer.Serialize(v.Select(c => c.ToString()), (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<IEnumerable<string>>(v, (JsonSerializerOptions?)null) != null
                    ? JsonSerializer.Deserialize<IEnumerable<string>>(v, (JsonSerializerOptions?)null)!
                        .Select(c => Enum.Parse<DeliveryChannelType>(c))
                        .ToList() 
                    : new List<DeliveryChannelType>()
            );

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Configure Metadata as JSON
        builder.Property(x => x.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
            );

        // Configure base entity properties
        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // Unique constraint on name
        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Index on notification type
        builder.HasIndex("NotificationType");
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.CreatedAt);
    }
}