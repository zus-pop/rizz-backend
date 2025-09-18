using MediatR;

namespace AuthService.Application.Commands;

public record RegisterUserCommand(string Email, string Password, string? PhoneNumber) : IRequest<int>;
