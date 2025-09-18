using MediatR;
using AuthService.Application.DTOs;

namespace AuthService.Application.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<AuthUserDto?>;
