namespace PurchaseService.Application.DTOs;

public class PurchaseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethodType { get; set; } = string.Empty;
    public string PaymentProvider { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StatusReason { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public SubscriptionDto? Subscription { get; set; }
    public RefundDto? Refund { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SubscriptionDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Duration { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public TimeSpan? RemainingTime { get; set; }
}

public class RefundDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? ExternalRefundId { get; set; }
}

public class CreatePurchaseDto
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

public class CreateSubscriptionDto
{
    public DateTime StartDate { get; set; }
    public string Type { get; set; } = string.Empty; // Daily, Weekly, Monthly, Yearly
    public int Duration { get; set; }
}

public class PaymentMethodDto
{
    public string Type { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? ExternalTransactionId { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class PurchaseHistoryDto
{
    public List<PurchaseDto> Purchases { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
}

public class UserPurchaseStatsDto
{
    public string UserId { get; set; } = string.Empty;
    public int TotalPurchases { get; set; }
    public decimal TotalSpent { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int ActiveSubscriptions { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public DateTime? FirstPurchaseDate { get; set; }
}