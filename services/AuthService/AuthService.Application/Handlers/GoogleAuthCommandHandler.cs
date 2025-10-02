using MediatR;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using System.Security.Cryptography;

namespace AuthService.Application.Handlers;

public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, LoginUserResult?>
{
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleAuthCommandHandler> _logger;

    public GoogleAuthCommandHandler(
        IAuthUserRepository authUserRepository,
        IJwtProvider jwtProvider,
        IConfiguration configuration,
        ILogger<GoogleAuthCommandHandler> logger)
    {
        _authUserRepository = authUserRepository;
        _jwtProvider = jwtProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginUserResult?> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify Google ID token
            var googleUser = await VerifyGoogleTokenAsync(request.IdToken);
            if (googleUser == null)
            {
                _logger.LogWarning("Invalid Google token provided");
                return null; // Return null for failure, consistent with LoginUserCommandHandler
            }

            // Check if user exists by email
            var existingUser = await _authUserRepository.GetByEmailAsync(googleUser.Email);

            AuthUser user;
            bool isNewUser = false;

            if (existingUser == null)
            {
                // Create new user for Google authentication
                user = new AuthUser
                {
                    Email = googleUser.Email,
                    PhoneNumber = "", // Empty for Google users initially
                    PasswordHash = "", // No password for Google users
                    IsEmailVerified = true, // Google email is already verified
                    IsPhoneVerified = false,
                    IsVerified = true, // Consider Google users as verified
                    VerifiedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _authUserRepository.AddAsync(user);
                await _authUserRepository.SaveChangesAsync();
                isNewUser = true;
                _logger.LogInformation("Created new user from Google authentication: {Email}", googleUser.Email);
            }
            else
            {
                user = existingUser;
                // Update verification status if not already verified
                if (!user.IsEmailVerified)
                {
                    user.VerifyEmail();
                    await _authUserRepository.UpdateAsync(user);
                    await _authUserRepository.SaveChangesAsync();
                }
                _logger.LogInformation("Existing user authenticated with Google: {Email}", googleUser.Email);
            }

            // Generate tokens
            var accessToken = _jwtProvider.GenerateJwtToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "30"));

            // Store refresh token
            user.SetRefreshToken(refreshToken, refreshTokenExpiry);
            await _authUserRepository.UpdateAsync(user);
            await _authUserRepository.SaveChangesAsync();

            return new LoginUserResult(
                accessToken,
                refreshToken,
                user.Id,
                user.Email,
                user.IsVerified);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return null; // Return null for failure
        }
    }

    private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _configuration["GoogleAuth:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Google token");
            return null;
        }
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}