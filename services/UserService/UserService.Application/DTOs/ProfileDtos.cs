namespace UserService.Application.DTOs
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProfileDto
    {
        public string? Bio { get; set; }
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
    }

    public class UpdateProfileDto
    {
        public string? Bio { get; set; }
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool? ShowOnlyVerified { get; set; }
    }

    public class ProfileWithUserDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDto? User { get; set; }
        public double? DistanceKm { get; set; }
    }
}