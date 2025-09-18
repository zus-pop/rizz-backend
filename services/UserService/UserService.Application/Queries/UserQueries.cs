using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto?>
    {
        public int Id { get; set; }
    }

    public class GetUserByEmailQuery : IRequest<UserDto?>
    {
        public string Email { get; set; } = string.Empty;
    }

    public class GetUserByPhoneNumberQuery : IRequest<UserDto?>
    {
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        public bool? Verified { get; set; }
        public string? Gender { get; set; }
    }

    public class GetNearbyUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusKm { get; set; } = 50;
    }
}