using PurchaseService.Domain.Entities;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Domain.Repositories;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Purchase>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Purchase>> GetByStatusAsync(PurchaseStatusType status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Purchase>> GetActiveSubscriptionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Purchase>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Purchase>> GetPurchaseHistoryAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPurchaseAmountAsync(int userId, string currency, CancellationToken cancellationToken = default);
    Task<int> GetPurchaseCountAsync(int userId, CancellationToken cancellationToken = default);
    
    Task<Purchase> AddAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasActiveSubscriptionAsync(int userId, string? productId = null, CancellationToken cancellationToken = default);
}