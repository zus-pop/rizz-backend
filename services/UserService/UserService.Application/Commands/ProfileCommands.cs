using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands
{
    public class CreateProfileCommand : IRequest<ProfileDto>
    {
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool ShowOnlyVerified { get; set; }
    }

    public class UpdateProfileCommand : IRequest<ProfileDto>
    {
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Job { get; set; }
        public string? School { get; set; }
        public int? InterestedInAgeMin { get; set; }
        public int? InterestedInAgeMax { get; set; }
        public string? InterestedInGender { get; set; }
        public double? MaxDistanceKm { get; set; }
        public bool? ShowOnlyVerified { get; set; }
    }

    public class DeleteProfileCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}