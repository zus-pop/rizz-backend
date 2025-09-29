using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AuthService.Application.Commands;
using AuthService.Application.Queries;
using AuthService.API.Models;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "AuthService", timestamp = DateTime.UtcNow });
        }

        [HttpPost("register")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                var command = new RegisterUserCommand(req.Email, req.Password, req.PhoneNumber);
                var userId = await _mediator.Send(command);
                return Ok(new { message = "User registered successfully", userId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", error = ex.Message });
            }
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var command = new LoginUserCommand(req.Email, req.Password);
                var result = await _mediator.Send(command);
                
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                return Ok(new { 
                    message = "Login successful", 
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    userId = result.UserId,
                    email = result.Email,
                    isVerified = result.IsVerified
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }

        [HttpPost("send-email-otp")]
        [EnableRateLimiting("OtpPolicy")]
        [Authorize]
        public async Task<IActionResult> SendEmailOtp([FromBody] GenerateEmailOtpRequest req)
        {
            try
            {
                var command = new SendEmailOtpCommand(req.Email);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to send OTP. Please try again later." });
                }

                return Ok(new { message = "OTP sent to your email successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send email OTP", error = ex.Message });
            }
        }

        [HttpPost("send-phone-otp")]
        [EnableRateLimiting("OtpPolicy")]
        [Authorize]
        public async Task<IActionResult> SendPhoneOtp([FromBody] GeneratePhoneOtpRequest req)
        {
            try
            {
                var command = new SendPhoneOtpCommand(req.PhoneNumber);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to send OTP. Please try again later." });
                }

                return Ok(new { message = "OTP sent to your phone successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send phone OTP", error = ex.Message });
            }
        }

        [HttpPost("verify-email")]
        [EnableRateLimiting("OtpPolicy")]
        [Authorize]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest req)
        {
            try
            {
                var command = new VerifyEmailOtpCommand(req.Email, req.Code);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { message = "Invalid or expired verification code" });
                }

                return Ok(new { message = "Email verified successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Email verification failed", error = ex.Message });
            }
        }

        [HttpPost("verify-phone")]
        [EnableRateLimiting("OtpPolicy")]
        [Authorize]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequest req)
        {
            try
            {
                var command = new VerifyPhoneOtpCommand(req.PhoneNumber, req.Code);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { message = "Invalid or expired verification code" });
                }

                return Ok(new { message = "Phone number verified successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Phone verification failed", error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            try
            {
                var command = new RefreshTokenCommand(req.Token, req.RefreshToken);
                var combined = await _mediator.Send(command);
                
                if (combined == null)
                {
                    return Unauthorized(new { message = "Invalid refresh token" });
                }
                string accessToken = combined;
                string? newRefresh = null;
                if (combined.Contains("||"))
                {
                    var parts = combined.Split("||", StringSplitOptions.RemoveEmptyEntries);
                    accessToken = parts[0];
                    newRefresh = parts.Length > 1 ? parts[1] : null;
                }
                return Ok(new {
                    message = "Token refreshed successfully",
                    accessToken,
                    refreshToken = newRefresh // may be null if rotation not applied
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Token refresh failed", error = ex.Message });
            }
        }

        [HttpPost("revoke-token/{userId}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> RevokeToken(int userId)
        {
            try
            {
                var command = new RevokeTokenCommand(userId);
                var result = await _mediator.Send(command);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to revoke token" });
                }

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Token revocation failed", error = ex.Message });
            }
        }
    }

    // Request models
    public record RefreshTokenRequest(string Token, string RefreshToken);
}
