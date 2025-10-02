using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurchaseService.Domain.Entities;

namespace PurchaseService.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.ProductId)
            .HasMaxLength(100);
            
        builder.Property(p => p.ProductName)
            .HasMaxLength(200);
            
        // Configure Money value object
        builder.OwnsOne(p => p.Amount, amount =>
        {
            amount.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();
                
            amount.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // Configure PaymentMethod value object
        builder.OwnsOne(p => p.PaymentMethod, payment =>
        {
            payment.Property(pm => pm.Type)
                .HasColumnName("PaymentMethodType")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
                
            payment.Property(pm => pm.Provider)
                .HasColumnName("PaymentProvider")
                .HasMaxLength(50);
                
            payment.Property(pm => pm.ExternalTransactionId)
                .HasColumnName("ExternalTransactionId")
                .HasMaxLength(100);
                
            payment.Property(pm => pm.Metadata)
                .HasColumnName("PaymentMetadata")
                .HasColumnType("text")
                .HasConversion(
                    v => v != null ? System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null) : null,
                    v => v != null ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) : null);
        });
        
        // Configure SubscriptionPeriod value object
        builder.OwnsOne(p => p.SubscriptionPeriod, subscription =>
        {
            subscription.Property(sp => sp.StartDate)
                .HasColumnName("SubscriptionStartDate");
                
            subscription.Property(sp => sp.EndDate)
                .HasColumnName("SubscriptionEndDate");
                
            subscription.Property(sp => sp.Duration)
                .HasColumnName("SubscriptionDuration");
                
            subscription.Property(sp => sp.Type)
                .HasColumnName("SubscriptionType")
                .HasConversion<string>();
        });
        
        // Configure PurchaseStatus value object
        builder.OwnsOne(p => p.Status, status =>
        {
            status.Property(s => s.Status)
                .HasColumnName("Status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
                
            status.Property(s => s.Reason)
                .HasColumnName("StatusReason")
                .HasMaxLength(500);
                
            status.Property(s => s.Timestamp)
                .HasColumnName("StatusTimestamp")
                .IsRequired();
        });
        
        // Configure Metadata
        builder.Property(p => p.Metadata)
            .HasColumnType("text")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new());
            
        builder.Property(p => p.CreatedAt)
            .IsRequired();
            
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
            
        // Configure relationships
        builder.HasMany(p => p.StatusHistory)
            .WithOne(sh => sh.Purchase)
            .HasForeignKey(sh => sh.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure Refund as owned entity
        builder.OwnsOne(p => p.Refund, refund =>
        {
            refund.Property<Guid>("Id");
            
            refund.OwnsOne(r => r.Amount, amount =>
            {
                amount.Property(m => m.Amount)
                    .HasColumnName("RefundAmount")
                    .HasPrecision(18, 2);
                    
                amount.Property(m => m.Currency)
                    .HasColumnName("RefundCurrency")
                    .HasMaxLength(3);
            });
            
            refund.Property(r => r.Reason)
                .HasColumnName("RefundReason")
                .HasMaxLength(500);
                
            refund.Property(r => r.Status)
                .HasColumnName("RefundStatus")
                .HasConversion<string>()
                .HasMaxLength(20);
                
            refund.Property(r => r.ExternalRefundId)
                .HasColumnName("RefundExternalId")
                .HasMaxLength(100);
                
            refund.Property(r => r.ProcessedAt)
                .HasColumnName("RefundProcessedAt");
                
            refund.Property(r => r.CreatedAt)
                .HasColumnName("RefundCreatedAt");
        });
            
        // Indexes
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Purchase_UserId");
            
        builder.HasIndex(p => p.ProductId)
            .HasDatabaseName("IX_Purchase_ProductId");
            
        builder.HasIndex(p => new { p.UserId, p.ProductId })
            .HasDatabaseName("IX_Purchase_UserId_ProductId");
    }
}