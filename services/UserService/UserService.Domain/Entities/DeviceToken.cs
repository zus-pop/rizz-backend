namespace UserService.Domain.Entities
{
    public class DeviceToken : BaseEntity
    {
        public int UserId { get; private set; }
        public string Token { get; private set; }
        public string DeviceType { get; private set; }
        public string? DeviceId { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime? LastUsedAt { get; private set; }

        private DeviceToken() { } // For EF Core

        public DeviceToken(int userId, string token, string deviceType, string? deviceId = null)
        {
            SetUserId(userId);
            SetToken(token);
            SetDeviceType(deviceType);
            SetDeviceId(deviceId);
            LastUsedAt = DateTime.UtcNow;
        }

        public void SetUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));
            
            UserId = userId;
            SetUpdatedAt();
        }

        public void SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            Token = token.Trim();
            SetUpdatedAt();
        }

        public void SetDeviceType(string deviceType)
        {
            if (string.IsNullOrWhiteSpace(deviceType))
                throw new ArgumentException("Device type cannot be empty", nameof(deviceType));

            var validDeviceTypes = new[] { "ios", "android", "web" };
            if (!validDeviceTypes.Contains(deviceType.ToLower()))
                throw new ArgumentException("Invalid device type", nameof(deviceType));

            DeviceType = deviceType.ToLower();
            SetUpdatedAt();
        }

        public void SetDeviceId(string? deviceId)
        {
            DeviceId = deviceId?.Trim();
            SetUpdatedAt();
        }

        public void Activate()
        {
            IsActive = true;
            LastUsedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }

        public void UpdateLastUsed()
        {
            LastUsedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }
}