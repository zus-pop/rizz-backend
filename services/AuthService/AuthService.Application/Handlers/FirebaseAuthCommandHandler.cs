using MediatR;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using System.Security.Cryptography;
using Google.Apis.Auth.OAuth2;

namespace AuthService.Application.Handlers;

public class FirebaseAuthCommandHandler : IRequestHandler<FirebaseAuthCommand, LoginUserResult?>
{
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseAuthCommandHandler> _logger;

    public FirebaseAuthCommandHandler(
        IAuthUserRepository authUserRepository,
        IJwtProvider jwtProvider,
        IConfiguration configuration,
        ILogger<FirebaseAuthCommandHandler> logger)
    {
        _authUserRepository = authUserRepository;
        _jwtProvider = jwtProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginUserResult?> Handle(FirebaseAuthCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if Firebase is initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                _logger.LogError("Firebase is not initialized. Please configure Firebase settings.");
                return null;
            }

            // Verify Firebase ID token
            var firebaseToken = await VerifyFirebaseTokenAsync(request.IdToken);
            if (firebaseToken == null)
            {
                _logger.LogWarning("Invalid Firebase token provided");
                return null;
            }

            // Check if user exists by email
            var email = firebaseToken.Claims.GetValueOrDefault("email")?.ToString() ?? string.Empty;
            var emailVerified = firebaseToken.Claims.GetValueOrDefault("email_verified") as bool? ?? false;
            
            var existingUser = await _authUserRepository.GetByEmailAsync(email);

            AuthUser user;
            bool isNewUser = false;

            if (existingUser == null)
            {
                // Create new user
                user = new AuthUser
                {
                    Email = email,
                    PhoneNumber = null,
                    PasswordHash = string.Empty, // No password for Firebase auth
                    IsEmailVerified = emailVerified,
                    IsPhoneVerified = false,
                    IsVerified = emailVerified
                };

                if (emailVerified)
                {
                    user.VerifyEmail();
                }

                await _authUserRepository.AddAsync(user);
                await _authUserRepository.SaveChangesAsync();
                isNewUser = true;
                _logger.LogInformation("Created new user from Firebase authentication: {Email}", email);
            }
            else
            {
                user = existingUser;
                
                // Update email verification status if it changed
                if (emailVerified && !user.IsEmailVerified)
                {
                    user.VerifyEmail();
                    await _authUserRepository.UpdateAsync(user);
                    await _authUserRepository.SaveChangesAsync();
                }
            }

            // Generate JWT tokens
            var accessToken = _jwtProvider.GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Update user's refresh token
            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(30));
            await _authUserRepository.UpdateAsync(user);
            await _authUserRepository.SaveChangesAsync();

            _logger.LogInformation("Firebase authentication successful for user: {Email}, IsNewUser: {IsNewUser}", 
                user.Email, isNewUser);

            return new LoginUserResult(accessToken, refreshToken, user.Id, user.Email, user.IsVerified);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Firebase authentication");
            return null;
        }
    }

    private async Task<FirebaseAdmin.Auth.FirebaseToken?> VerifyFirebaseTokenAsync(string idToken)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return decodedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase token");
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

public class VerifyFirebaseTokenCommandHandler : IRequestHandler<VerifyFirebaseTokenCommand, FirebaseTokenResult?>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<VerifyFirebaseTokenCommandHandler> _logger;

    public VerifyFirebaseTokenCommandHandler(
        IConfiguration configuration,
        ILogger<VerifyFirebaseTokenCommandHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<FirebaseTokenResult?> Handle(VerifyFirebaseTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if Firebase is initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                _logger.LogError("Firebase is not initialized. Please configure Firebase settings.");
                return null;
            }

            // Verify Firebase ID token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
            
            if (decodedToken == null)
            {
                _logger.LogWarning("Invalid Firebase token provided");
                return null;
            }

            var result = new FirebaseTokenResult
            {
                Uid = decodedToken.Uid,
                Email = decodedToken.Claims.GetValueOrDefault("email")?.ToString() ?? string.Empty,
                Name = decodedToken.Claims.GetValueOrDefault("name")?.ToString() ?? string.Empty,
                Picture = decodedToken.Claims.GetValueOrDefault("picture")?.ToString() ?? string.Empty,
                EmailVerified = decodedToken.Claims.GetValueOrDefault("email_verified") as bool? ?? false,
                Claims = decodedToken.Claims.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            _logger.LogInformation("Firebase token verified successfully for user: {Email}", result.Email);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Firebase token");
            return null;
        }
    }

    private async Task<FirebaseToken> VerifyFirebaseTokenAsync(string idToken)
    {
        try
        {
            // Check if Firebase is initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                _logger.LogError("Firebase is not initialized. Please configure Firebase settings.");
                throw new InvalidOperationException("Firebase is not initialized");
            }

            // Verify the Firebase token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return decodedToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase token");
            throw;
        }
    }
}