using MediatR;
using AutoMapper;
using PurchaseService.Application.Queries;
using PurchaseService.Application.DTOs;
using PurchaseService.Application.Repositories;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Application.Handlers;

public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, PurchaseDto?>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public GetPurchaseByIdQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseDto?> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.Id, cancellationToken);
        return purchase != null ? _mapper.Map<PurchaseDto>(purchase) : null;
    }
}

public class GetUserPurchasesQueryHandler : IRequestHandler<GetUserPurchasesQuery, List<PurchaseDto>>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public GetUserPurchasesQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<List<PurchaseDto>> Handle(GetUserPurchasesQuery request, CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<List<PurchaseDto>>(purchases);
    }
}

public class GetPurchasesByStatusQueryHandler : IRequestHandler<GetPurchasesByStatusQuery, List<PurchaseDto>>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public GetPurchasesByStatusQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<List<PurchaseDto>> Handle(GetPurchasesByStatusQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PurchaseStatusType>(request.Status, true, out var statusType))
            throw new ArgumentException($"Invalid status: {request.Status}");

        var status = new PurchaseStatus(statusType);
        var purchases = await _purchaseRepository.GetByStatusAsync(status, cancellationToken);
        return _mapper.Map<List<PurchaseDto>>(purchases);
    }
}

public class GetActiveSubscriptionsQueryHandler : IRequestHandler<GetActiveSubscriptionsQuery, List<PurchaseDto>>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public GetActiveSubscriptionsQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<List<PurchaseDto>> Handle(GetActiveSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _purchaseRepository.GetActiveSubscriptionsAsync(request.UserId, cancellationToken);
        return _mapper.Map<List<PurchaseDto>>(subscriptions);
    }
}

public class GetPurchaseHistoryQueryHandler : IRequestHandler<GetPurchaseHistoryQuery, PurchaseHistoryDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public GetPurchaseHistoryQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseHistoryDto> Handle(GetPurchaseHistoryQuery request, CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.GetPurchaseHistoryAsync(
            request.UserId, 
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var totalCount = await _purchaseRepository.GetPurchaseCountAsync(request.UserId, cancellationToken);
        var totalAmount = await _purchaseRepository.GetTotalPurchaseAmountAsync(request.UserId, "USD", cancellationToken);

        return new PurchaseHistoryDto
        {
            Purchases = _mapper.Map<List<PurchaseDto>>(purchases),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalAmount = totalAmount,
            Currency = "USD"
        };
    }
}

public class GetUserPurchaseStatsQueryHandler : IRequestHandler<GetUserPurchaseStatsQuery, UserPurchaseStatsDto>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public GetUserPurchaseStatsQueryHandler(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<UserPurchaseStatsDto> Handle(GetUserPurchaseStatsQuery request, CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var activeSubscriptions = await _purchaseRepository.GetActiveSubscriptionsAsync(request.UserId, cancellationToken);
        var totalAmount = await _purchaseRepository.GetTotalPurchaseAmountAsync(request.UserId, "USD", cancellationToken);
        var totalCount = await _purchaseRepository.GetPurchaseCountAsync(request.UserId, cancellationToken);

        var purchasesList = purchases.ToList();
        
        return new UserPurchaseStatsDto
        {
            UserId = request.UserId,
            TotalPurchases = totalCount,
            TotalSpent = totalAmount,
            Currency = "USD",
            ActiveSubscriptions = activeSubscriptions.Count(),
            LastPurchaseDate = purchasesList.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.CreatedAt,
            FirstPurchaseDate = purchasesList.OrderBy(p => p.CreatedAt).FirstOrDefault()?.CreatedAt
        };
    }
}

public class GetExpiredSubscriptionsQueryHandler : IRequestHandler<GetExpiredSubscriptionsQuery, List<PurchaseDto>>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public GetExpiredSubscriptionsQueryHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<List<PurchaseDto>> Handle(GetExpiredSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var expiredSubscriptions = await _purchaseRepository.GetExpiredSubscriptionsAsync(cancellationToken);
        return _mapper.Map<List<PurchaseDto>>(expiredSubscriptions);
    }
}