using MediatR;
using AuthService.Application.Commands;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using BCrypt.Net;

namespace AuthService.Application.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IAuthUserRepository _userRepo;

    public RegisterUserCommandHandler(IAuthUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("User already exists");

        var user = new AuthUser
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsVerified = false,
            IsEmailVerified = false,
            IsPhoneVerified = false
        };
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return user.Id;
    }
}
