using Microsoft.EntityFrameworkCore;
using PushService.Domain.Entities;
using PushService.Domain.Repositories;
using PushService.Domain.ValueObjects;
using PushService.Infrastructure.Data;

namespace PushService.Infrastructure.Repositories
{
    public class DeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly PushDbContext _context;

        public DeviceTokenRepository(PushDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.Id == id, cancellationToken);
        }

        public async Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.Token == token, cancellationToken);
        }

        public async Task<IEnumerable<DeviceToken>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .Where(dt => dt.UserId == userId)
                .OrderByDescending(dt => dt.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DeviceToken>> GetActiveTokensByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .Where(dt => dt.UserId == userId && dt.IsActive)
                .OrderByDescending(dt => dt.LastUsedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DeviceToken>> GetByDeviceTypeAsync(DeviceType deviceType, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .Where(dt => dt.DeviceType == deviceType && dt.IsActive)
                .OrderByDescending(dt => dt.LastUsedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<DeviceToken> AddAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
        {
            _context.DeviceTokens.Add(deviceToken);
            await _context.SaveChangesAsync(cancellationToken);
            return deviceToken;
        }

        public async Task<DeviceToken> UpdateAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
        {
            _context.DeviceTokens.Update(deviceToken);
            await _context.SaveChangesAsync(cancellationToken);
            return deviceToken;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var deviceToken = await GetByIdAsync(id, cancellationToken);
            if (deviceToken != null)
            {
                _context.DeviceTokens.Remove(deviceToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> TokenExistsAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .AnyAsync(dt => dt.Token == token, cancellationToken);
        }

        public async Task<IEnumerable<DeviceToken>> GetExpiredTokensAsync(TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow - expirationTime;
            return await _context.DeviceTokens
                .Where(dt => dt.LastUsedAt.HasValue && dt.LastUsedAt.Value < cutoffDate)
                .ToListAsync(cancellationToken);
        }

        public async Task DeactivateExpiredTokensAsync(TimeSpan expirationTime, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow - expirationTime;
            await _context.DeviceTokens
                .Where(dt => dt.LastUsedAt.HasValue && dt.LastUsedAt.Value < cutoffDate && dt.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(dt => dt.IsActive, false)
                    .SetProperty(dt => dt.UpdatedAt, DateTime.UtcNow), cancellationToken);
        }

        public async Task<int> GetActiveTokenCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .CountAsync(dt => dt.UserId == userId && dt.IsActive, cancellationToken);
        }
    }
}