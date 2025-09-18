using MediatR;
using AuthService.Application.DTOs;
using AuthService.Application.Queries;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Handlers;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, AuthUserDto?>
{
    private readonly IAuthUserRepository _userRepo;
    public GetUserByEmailQueryHandler(IAuthUserRepository userRepo) => _userRepo = userRepo;

    public async Task<AuthUserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null) return null;
        return new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsVerified = user.IsVerified
        };
    }
}
