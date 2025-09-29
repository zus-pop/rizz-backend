using MediatR;
using AuthService.Application.DTOs;

namespace AuthService.Application.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResult?>;
