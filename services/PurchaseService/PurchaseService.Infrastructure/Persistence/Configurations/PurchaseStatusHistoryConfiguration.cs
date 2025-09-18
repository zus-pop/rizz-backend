using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseService.Domain.Entities;

namespace PurchaseService.Infrastructure.Persistence.Configurations;

public class PurchaseStatusHistoryConfiguration : IEntityTypeConfiguration<PurchaseStatusHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.PurchaseId)
            .IsRequired();
            
        // Configure PurchaseStatus value object
        builder.OwnsOne(h => h.Status, status =>
        {
            status.Property(s => s.Status)
                .HasColumnName("Status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
                
            status.Property(s => s.Reason)
                .HasColumnName("Reason")
                .HasMaxLength(500);
        });
        
        builder.Property(h => h.ChangedAt)
            .IsRequired();
            
        // Indexes
        builder.HasIndex(h => h.PurchaseId)
            .HasDatabaseName("IX_PurchaseStatusHistory_PurchaseId");
            
        builder.HasIndex(h => h.ChangedAt)
            .HasDatabaseName("IX_PurchaseStatusHistory_ChangedAt");
    }
}