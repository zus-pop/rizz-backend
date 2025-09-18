using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MatchService.Domain.Entities;

namespace MatchService.Infrastructure.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            // Table configuration
            builder.ToTable("matches");
            builder.HasKey(m => m.Id);

            // Primary key
            builder.Property(m => m.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            // User IDs
            builder.Property(m => m.User1Id)
                .HasColumnName("user1_id")
                .IsRequired();

            builder.Property(m => m.User2Id)
                .HasColumnName("user2_id")
                .IsRequired();

            // Match status
            builder.Property(m => m.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);

            // Timestamps
            builder.Property(m => m.MatchedAt)
                .HasColumnName("matched_at")
                .IsRequired();

            builder.Property(m => m.UnmatchedAt)
                .HasColumnName("unmatched_at");

            builder.Property(m => m.UnmatchedByUserId)
                .HasColumnName("unmatched_by_user_id");

            // Base entity properties
            builder.Property(m => m.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(m => m.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(m => m.IsDeleted)
                .HasColumnName("is_deleted")
                .IsRequired()
                .HasDefaultValue(false);

            // Constraints
            builder.HasCheckConstraint("CK_Match_DifferentUsers", "user1_id != user2_id");
            builder.HasCheckConstraint("CK_Match_UnmatchLogic", 
                "(is_active = true AND unmatched_at IS NULL AND unmatched_by_user_id IS NULL) OR " +
                "(is_active = false AND unmatched_at IS NOT NULL AND unmatched_by_user_id IS NOT NULL)");

            // Indexes are defined in DbContext for better visibility
        }
    }

    public class SwipeConfiguration : IEntityTypeConfiguration<Swipe>
    {
        public void Configure(EntityTypeBuilder<Swipe> builder)
        {
            // Table configuration
            builder.ToTable("swipes");
            builder.HasKey(s => s.Id);

            // Primary key
            builder.Property(s => s.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            // User IDs
            builder.Property(s => s.SwiperId)
                .HasColumnName("swiper_id")
                .IsRequired();

            builder.Property(s => s.TargetUserId)
                .HasColumnName("target_user_id")
                .IsRequired();

            // Swipe direction
            builder.Property(s => s.Direction)
                .HasColumnName("direction")
                .HasConversion<string>()
                .IsRequired();

            // Timestamp
            builder.Property(s => s.SwipedAt)
                .HasColumnName("swiped_at")
                .IsRequired();

            // Base entity properties
            builder.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(s => s.IsDeleted)
                .HasColumnName("is_deleted")
                .IsRequired()
                .HasDefaultValue(false);

            // Constraints
            builder.HasCheckConstraint("CK_Swipe_DifferentUsers", "swiper_id != target_user_id");

            // Indexes are defined in DbContext for better visibility
        }
    }
}