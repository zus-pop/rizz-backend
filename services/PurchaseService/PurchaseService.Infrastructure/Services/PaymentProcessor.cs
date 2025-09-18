using PurchaseService.Domain.Entities;
using PurchaseService.Domain.Services;
using PurchaseService.Domain.ValueObjects;
using Serilog;

namespace PurchaseService.Infrastructure.Services;

public class PaymentProcessor : IPaymentProcessor
{
    private readonly ILogger _logger;

    public PaymentProcessor(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        Purchase purchase, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Processing payment for purchase {PurchaseId} with amount {Amount} {Currency}",
                purchase.Id, purchase.Amount.Amount, purchase.Amount.Currency);

            // Simulate payment processing
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            // Simulate different payment scenarios based on amount
            if (purchase.Amount.Amount <= 0)
            {
                return PaymentResult.Failure("Invalid amount");
            }

            if (purchase.Amount.Amount > 10000)
            {
                return PaymentResult.Failure("Amount exceeds limit");
            }

            // Simulate random failures for testing (10% failure rate)
            var random = new Random();
            if (random.NextDouble() < 0.1)
            {
                return PaymentResult.Failure("Payment gateway error");
            }

            // Simulate successful payment
            var transactionId = $"txn_{Guid.NewGuid():N}";
            var metadata = new Dictionary<string, string>
            {
                { "provider", purchase.PaymentMethod.Provider ?? "Unknown" },
                { "type", purchase.PaymentMethod.Type.ToString() }
            };
            
            _logger.Information("Payment processed successfully for purchase {PurchaseId} with transaction ID {TransactionId}",
                purchase.Id, transactionId);

            return PaymentResult.Success(transactionId, metadata);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error processing payment for purchase {PurchaseId}", purchase.Id);
            return PaymentResult.Failure("Payment processing failed");
        }
    }

    public async Task<RefundResult> ProcessRefundAsync(
        Purchase purchase, 
        Money refundAmount, 
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Processing refund for purchase {PurchaseId} with amount {Amount} {Currency}",
                purchase.Id, refundAmount.Amount, refundAmount.Currency);

            // Simulate refund processing
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            // Simulate refund scenarios
            if (refundAmount.Amount <= 0)
            {
                return RefundResult.Failure("Invalid refund amount");
            }

            if (refundAmount.Amount > purchase.Amount.Amount)
            {
                return RefundResult.Failure("Refund amount exceeds purchase amount");
            }

            // Simulate random failures for testing (5% failure rate)
            var random = new Random();
            if (random.NextDouble() < 0.05)
            {
                return RefundResult.Failure("Refund gateway error");
            }

            // Simulate successful refund
            var refundId = $"ref_{Guid.NewGuid():N}";
            
            _logger.Information("Refund processed successfully for purchase {PurchaseId} with refund ID {RefundId}",
                purchase.Id, refundId);

            return RefundResult.Success(refundId, refundAmount);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error processing refund for purchase {PurchaseId}", purchase.Id);
            return RefundResult.Failure("Refund processing failed");
        }
    }

    public async Task<bool> ValidatePaymentMethodAsync(
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Validating payment method {Provider} of type {Type}",
                paymentMethod.Provider, paymentMethod.Type);

            // Simulate validation
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

            // Basic validation rules
            if (string.IsNullOrWhiteSpace(paymentMethod.Provider))
            {
                return false;
            }

            // Simulate provider-specific validation
            return paymentMethod.Type switch
            {
                PaymentMethodType.CreditCard => !string.IsNullOrWhiteSpace(paymentMethod.ExternalTransactionId),
                PaymentMethodType.MobileMoney => !string.IsNullOrWhiteSpace(paymentMethod.ExternalTransactionId),
                PaymentMethodType.BankTransfer => !string.IsNullOrWhiteSpace(paymentMethod.Provider),
                PaymentMethodType.DigitalWallet => !string.IsNullOrWhiteSpace(paymentMethod.ExternalTransactionId),
                PaymentMethodType.Cryptocurrency => !string.IsNullOrWhiteSpace(paymentMethod.ExternalTransactionId),
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error validating payment method {Provider}", paymentMethod.Provider);
            return false;
        }
    }
}