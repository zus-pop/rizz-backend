using MediatR;
using PurchaseService.Application.DTOs;

namespace PurchaseService.Application.Commands;

public class CreatePurchaseCommand : IRequest<PurchaseDto>
{
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethodType { get; set; } = string.Empty;
    public string PaymentProvider { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public CreateSubscriptionDto? Subscription { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class ProcessPaymentCommand : IRequest<PurchaseDto>
{
    public Guid PurchaseId { get; set; }
}

public class CancelPurchaseCommand : IRequest<PurchaseDto>
{
    public Guid PurchaseId { get; set; }
    public string? Reason { get; set; }
}

public class RefundPurchaseCommand : IRequest<PurchaseDto>
{
    public Guid PurchaseId { get; set; }
    public decimal? RefundAmount { get; set; } // If null, refund full amount
    public string? Reason { get; set; }
}

public class UpdatePaymentMethodCommand : IRequest<PurchaseDto>
{
    public Guid PurchaseId { get; set; }
    public string PaymentMethodType { get; set; } = string.Empty;
    public string PaymentProvider { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}