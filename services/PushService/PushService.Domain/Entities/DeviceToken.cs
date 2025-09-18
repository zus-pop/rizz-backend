using Common.Domain;
using PushService.Domain.ValueObjects;

namespace PushService.Domain.Entities
{
    public class DeviceToken : BaseEntity
    {
        public int UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DeviceType DeviceType { get; private set; }
        public string? DeviceId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastUsedAt { get; private set; }

        // EF Core constructor
        private DeviceToken() { }

        public DeviceToken(int userId, string token, DeviceType deviceType, string? deviceId = null)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than 0", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            UserId = userId;
            Token = token;
            DeviceType = deviceType;
            DeviceId = deviceId;
            IsActive = true;
            UpdateLastUsed();
        }

        public void UpdateToken(string newToken)
        {
            if (string.IsNullOrWhiteSpace(newToken))
                throw new ArgumentException("Token cannot be empty", nameof(newToken));
            
            Token = newToken;
            UpdateLastUsed();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdateLastUsed();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLastUsed()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        public bool IsExpired(TimeSpan expirationTime)
        {
            return LastUsedAt.HasValue && 
                   DateTime.UtcNow - LastUsedAt.Value > expirationTime;
        }
    }
}