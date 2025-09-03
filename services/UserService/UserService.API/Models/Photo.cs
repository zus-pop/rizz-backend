using NetTopologySuite.Geometries;

namespace UserService.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int SortOrder { get; set; }
        public bool IsProfilePicture { get; set; }
    }
}
