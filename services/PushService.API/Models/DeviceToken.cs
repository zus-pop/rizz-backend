namespace PushService.API.Models
{
    public class DeviceToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Platform { get; set; } = "Android"; // iOS, Android
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    }
}
