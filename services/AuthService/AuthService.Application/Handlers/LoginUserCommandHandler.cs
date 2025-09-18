using MediatR;
using AuthService.Application.Commands;
using AuthService.Domain.Interfaces;
using BCrypt.Net;

namespace AuthService.Application.Handlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string?>
{
    private readonly IAuthUserRepository _userRepo;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(IAuthUserRepository userRepo, IJwtProvider jwtProvider)
    {
        _userRepo = userRepo;
        _jwtProvider = jwtProvider;
    }

    public async Task<string?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;
        return _jwtProvider.GenerateJwtToken(user);
    }
}
