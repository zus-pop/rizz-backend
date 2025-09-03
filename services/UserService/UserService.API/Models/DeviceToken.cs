using NetTopologySuite.Geometries;

namespace UserService.API.Models
{
    public class DeviceToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Platform { get; set; }
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    }
}
