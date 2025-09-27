using MediatR;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using BCrypt.Net;

namespace AuthService.Application.Handlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult?>
{
    private readonly IAuthUserRepository _userRepo;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(IAuthUserRepository userRepo, IJwtProvider jwtProvider)
    {
        _userRepo = userRepo;
        _jwtProvider = jwtProvider;
    }

    public async Task<LoginUserResult?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null)
            return null;

        // Check if account is locked
        if (user.IsLockedOut)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.IncrementFailedLogins();
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();
            return null;
        }

        // Reset failed login attempts on successful login
        user.ResetFailedLogins();
        
        // Generate tokens
        var accessToken = _jwtProvider.GenerateJwtToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();
        
        // Set refresh token with expiry (30 days)
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(30));
        
        await _userRepo.UpdateAsync(user);
        await _userRepo.SaveChangesAsync();
        
        return new LoginUserResult(accessToken, refreshToken, user.Id, user.Email, user.IsVerified);
    }
}
