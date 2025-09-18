using PurchaseService.Domain.Entities;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Application.Repositories;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetByUserIdPaginatedAsync(string userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetActiveSubscriptionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetByStatusAsync(PurchaseStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetByStatusPaginatedAsync(PurchaseStatus status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Purchase?> GetByUserAndProductAsync(string userId, string productId, CancellationToken cancellationToken = default);
    Task<bool> HasActivePurchaseAsync(string userId, string productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetPurchaseHistoryAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPurchaseAmountAsync(string userId, string currency, CancellationToken cancellationToken = default);
    Task<int> GetPurchaseCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetUserPurchaseCountAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default);
    void Update(Purchase purchase);
    void Remove(Purchase purchase);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}