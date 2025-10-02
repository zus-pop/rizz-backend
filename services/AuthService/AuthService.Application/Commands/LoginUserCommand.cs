using MediatR;

namespace AuthService.Application.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<string?>;
