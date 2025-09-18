using Microsoft.EntityFrameworkCore;
using PurchaseService.Application.Repositories;
using PurchaseService.Domain.Entities;
using PurchaseService.Domain.ValueObjects;
using PurchaseService.Infrastructure.Persistence;

namespace PurchaseService.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly PurchaseDbContext _context;

    public PurchaseRepository(PurchaseDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetByUserIdAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetByUserIdPaginatedAsync(
        string userId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetActiveSubscriptionsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.UserId == userId 
                && p.SubscriptionPeriod != null
                && p.SubscriptionPeriod.StartDate <= now
                && p.SubscriptionPeriod.EndDate >= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetByStatusAsync(
        PurchaseStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.Status.Status == status.Status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetByStatusPaginatedAsync(
        PurchaseStatus status, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.Status.Status == status.Status)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Purchase?> GetByUserAndProductAsync(
        string userId, 
        string productId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ProductId == productId, cancellationToken);
    }

    public async Task<bool> HasActivePurchaseAsync(
        string userId, 
        string productId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .AnyAsync(p => p.UserId == userId 
                && p.ProductId == productId 
                && (p.Status.Status == PurchaseStatusType.Completed 
                    || p.Status.Status == PurchaseStatusType.Processing), 
                cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Purchases.CountAsync(cancellationToken);
    }

    public async Task<int> GetUserPurchaseCountAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .CountAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        await _context.Purchases.AddAsync(purchase, cancellationToken);
    }

    public void Update(Purchase purchase)
    {
        _context.Purchases.Update(purchase);
    }

    public void Remove(Purchase purchase)
    {
        _context.Purchases.Remove(purchase);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.SubscriptionPeriod != null && 
                       p.SubscriptionPeriod.EndDate < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetPurchaseHistoryAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Include(p => p.StatusHistory)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalPurchaseAmountAsync(string userId, string currency, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .Where(p => p.UserId == userId && p.Amount.Currency == currency)
            .SumAsync(p => p.Amount.Amount, cancellationToken);
    }

    public async Task<int> GetPurchaseCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Purchases
            .CountAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        _context.Purchases.Update(purchase);
        await _context.SaveChangesAsync(cancellationToken);
    }
}