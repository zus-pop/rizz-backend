using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;
using NotificationService.Domain.ValueObjects;
using System.Text.Json;

namespace NotificationService.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasMaxLength(255)
            .IsRequired();

        // Configure NotificationType as conversion
        builder.Property(x => x.NotificationType)
            .HasConversion(
                v => v.Value,
                v => NotificationType.FromString(v))
            .HasColumnName("NotificationType")
            .HasMaxLength(50)
            .IsRequired();

        // Configure NotificationContent as separate properties
        builder.Ignore(x => x.Content);
        
        builder.Property<string>("Title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property<string>("Body")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property<string>("ContentData")
            .HasDefaultValue("{}");

        // Configure NotificationPriority as conversion
        builder.Property(x => x.Priority)
            .HasConversion(
                v => v.Value,
                v => NotificationPriority.FromString(v))
            .HasColumnName("Priority")
            .HasMaxLength(20)
            .IsRequired();

        // Configure Channels as JSON - simplified approach
        builder.Property(x => x.Channels)
            .HasConversion(
                v => JsonSerializer.Serialize(v.Select(c => new ChannelDto { 
                    Type = c.Type.ToString(), 
                    Address = c.Address, 
                    IsEnabled = c.IsEnabled 
                }), (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<ChannelDto>>(v, (JsonSerializerOptions?)null) != null
                    ? JsonSerializer.Deserialize<List<ChannelDto>>(v, (JsonSerializerOptions?)null)!
                        .Select(c => new DeliveryChannel(
                            Enum.Parse<DeliveryChannelType>(c.Type),
                            c.Address,
                            c.IsEnabled,
                            new Dictionary<string, string>()
                        )).ToList() 
                    : new List<DeliveryChannel>()
            );

        // Configure NotificationStatus as conversion
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.ScheduledAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.SentAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.ReadAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.ExpiredAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

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

        // Indexes
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.ScheduledAt);
        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}