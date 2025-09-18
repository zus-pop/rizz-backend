using MediatR;
using PurchaseService.Application.DTOs;

namespace PurchaseService.Application.Queries;

public class GetPurchaseByIdQuery : IRequest<PurchaseDto?>
{
    public Guid Id { get; set; }
}

public class GetUserPurchasesQuery : IRequest<List<PurchaseDto>>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetPurchasesByStatusQuery : IRequest<List<PurchaseDto>>
{
    public string Status { get; set; } = string.Empty;
}

public class GetActiveSubscriptionsQuery : IRequest<List<PurchaseDto>>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetPurchaseHistoryQuery : IRequest<PurchaseHistoryDto>
{
    public string UserId { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetUserPurchaseStatsQuery : IRequest<UserPurchaseStatsDto>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetExpiredSubscriptionsQuery : IRequest<List<PurchaseDto>>
{
    // No parameters needed - returns all expired subscriptions
}