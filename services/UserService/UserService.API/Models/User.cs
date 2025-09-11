using NetTopologySuite.Geometries;

namespace UserService.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public DateTime? Birthday { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public double? Height { get; set; }
        public required string Personality { get; set; }
        public Point? Location { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
