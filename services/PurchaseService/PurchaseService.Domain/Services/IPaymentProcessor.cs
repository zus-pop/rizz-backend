using PurchaseService.Domain.Entities;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Domain.Services;

public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessPaymentAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task<RefundResult> ProcessRefundAsync(Purchase purchase, Money refundAmount, string? reason = null, CancellationToken cancellationToken = default);
    Task<bool> ValidatePaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
}

public class PaymentResult
{
    public bool IsSuccessful { get; init; }
    public string? ExternalTransactionId { get; init; }
    public string? FailureReason { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();

    public static PaymentResult Success(string externalTransactionId, Dictionary<string, string>? metadata = null) =>
        new() { IsSuccessful = true, ExternalTransactionId = externalTransactionId, Metadata = metadata ?? new() };

    public static PaymentResult Failure(string reason) =>
        new() { IsSuccessful = false, FailureReason = reason };
}

public class RefundResult
{
    public bool IsSuccessful { get; init; }
    public string? ExternalRefundId { get; init; }
    public string? FailureReason { get; init; }
    public Money? ProcessedAmount { get; init; }

    public static RefundResult Success(string externalRefundId, Money processedAmount) =>
        new() { IsSuccessful = true, ExternalRefundId = externalRefundId, ProcessedAmount = processedAmount };

    public static RefundResult Failure(string reason) =>
        new() { IsSuccessful = false, FailureReason = reason };
}