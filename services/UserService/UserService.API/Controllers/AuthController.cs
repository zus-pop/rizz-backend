using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Login with Google ID token
        /// </summary>
        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdToken))
                {
                    return BadRequest(new { success = false, message = "Google ID token is required" });
                }

                // Verify Google ID token
                var googleUser = await VerifyGoogleTokenAsync(request.IdToken);
                if (googleUser == null)
                {
                    return BadRequest(new { success = false, message = "Invalid Google token" });
                }

                // Check if user exists by email
                var existingUserQuery = new GetUserByEmailQuery { Email = googleUser.Email };
                var existingUser = await _mediator.Send(existingUserQuery);

                UserDto user;
                bool isNewUser = false;

                if (existingUser == null)
                {
                    // Create new user
                    var createUserCommand = new CreateUserCommand
                    {
                        FirstName = googleUser.GivenName ?? "User",
                        LastName = googleUser.FamilyName ?? "",
                        Email = googleUser.Email,
                        PhoneNumber = "0000000000", // Temporary placeholder for Google users
                        Gender = "other", // Default, can be updated later
                        Personality = "friendly" // Default
                    };

                    user = await _mediator.Send(createUserCommand);
                    isNewUser = true;
                    _logger.LogInformation("Created new user from Google login: {Email}", googleUser.Email);
                }
                else
                {
                    user = existingUser;
                    _logger.LogInformation("Existing user logged in with Google: {Email}", googleUser.Email);
                }

                // Generate JWT tokens
                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();

                // TODO: Store refresh token in database for future use
                
                return Ok(new
                {
                    success = true,
                    message = isNewUser ? "User created and authenticated successfully" : "User authenticated successfully",
                    data = new
                    {
                        user = new
                        {
                            id = user.Id,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            email = user.Email,
                            isVerified = user.IsVerified,
                            isNewUser = isNewUser
                        },
                        accessToken = accessToken,
                        refreshToken = refreshToken,
                        tokenType = "Bearer",
                        expiresIn = 3600 // 1 hour in seconds
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google authentication");
                return StatusCode(500, new { success = false, message = "Internal server error during authentication" });
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { success = false, message = "Refresh token is required" });
                }

                // TODO: Validate refresh token from database
                // For now, we'll generate a new access token
                
                // Extract user ID from refresh token or validate against stored tokens
                // This is a simplified version - in production, you should store and validate refresh tokens
                
                return Ok(new
                {
                    success = true,
                    message = "Token refreshed successfully",
                    data = new
                    {
                        accessToken = "new_access_token", // TODO: Generate actual token
                        refreshToken = request.RefreshToken, // Keep same refresh token or generate new one
                        tokenType = "Bearer",
                        expiresIn = 3600
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { success = false, message = "Internal server error during token refresh" });
            }
        }

        /// <summary>
        /// Validate current access token
        /// </summary>
        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            return Ok(new
            {
                success = true,
                message = "Token is valid",
                data = new
                {
                    userId = userId,
                    email = User.FindFirst(ClaimTypes.Email)?.Value,
                    name = User.FindFirst(ClaimTypes.Name)?.Value
                }
            });
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

        private string GenerateAccessToken(UserDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            // Generate a secure random string for refresh token
            var randomBytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string? DeviceToken { get; set; } // For push notifications
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}