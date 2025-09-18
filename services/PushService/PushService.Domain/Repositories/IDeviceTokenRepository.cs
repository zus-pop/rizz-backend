using PushService.Domain.Entities;
using PushService.Domain.ValueObjects;

namespace PushService.Domain.Repositories
{
    public interface IDeviceTokenRepository
    {
        Task<DeviceToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<IEnumerable<DeviceToken>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DeviceToken>> GetActiveTokensByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DeviceToken>> GetByDeviceTypeAsync(DeviceType deviceType, CancellationToken cancellationToken = default);
        Task<DeviceToken> AddAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
        Task<DeviceToken> UpdateAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> TokenExistsAsync(string token, CancellationToken cancellationToken = default);
        Task<IEnumerable<DeviceToken>> GetExpiredTokensAsync(TimeSpan expirationTime, CancellationToken cancellationToken = default);
        Task DeactivateExpiredTokensAsync(TimeSpan expirationTime, CancellationToken cancellationToken = default);
        Task<int> GetActiveTokenCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }

    public interface IPushNotificationService
    {
        Task<bool> SendNotificationAsync(string token, PushNotification notification, CancellationToken cancellationToken = default);
        Task<bool> SendNotificationToUserAsync(int userId, PushNotification notification, CancellationToken cancellationToken = default);
        Task<bool> SendNotificationToMultipleUsersAsync(IEnumerable<int> userIds, PushNotification notification, CancellationToken cancellationToken = default);
        Task<bool> SendNotificationToAllAsync(PushNotification notification, CancellationToken cancellationToken = default);
        Task<Dictionary<string, bool>> SendNotificationToTokensAsync(IEnumerable<string> tokens, PushNotification notification, CancellationToken cancellationToken = default);
    }
}