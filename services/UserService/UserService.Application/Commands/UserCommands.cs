using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Personality { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public double? Height { get; set; }
        public LocationDto? Location { get; set; }
    }

    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Personality { get; set; }
        public DateTime? Birthday { get; set; }
        public double? Height { get; set; }
        public LocationDto? Location { get; set; }
    }

    public class DeleteUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class VerifyUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
    }

    public class UnverifyUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
    }
}