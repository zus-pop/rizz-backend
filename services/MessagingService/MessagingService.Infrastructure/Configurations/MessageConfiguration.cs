using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MessagingService.Domain.Entities;

namespace MessagingService.Infrastructure.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.MatchId)
                .IsRequired();

            builder.Property(m => m.SenderId)
                .IsRequired();

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(m => m.ExtraData)
                .HasMaxLength(2000);

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(m => m.ReadAt)
                .IsRequired(false);

            // Indexes for performance
            builder.HasIndex(m => m.MatchId)
                .HasDatabaseName("IX_Messages_MatchId");

            builder.HasIndex(m => m.SenderId)
                .HasDatabaseName("IX_Messages_SenderId");

            builder.HasIndex(m => m.CreatedAt)
                .HasDatabaseName("IX_Messages_CreatedAt");

            builder.HasIndex(m => new { m.MatchId, m.CreatedAt })
                .HasDatabaseName("IX_Messages_MatchId_CreatedAt");
        }
    }
}