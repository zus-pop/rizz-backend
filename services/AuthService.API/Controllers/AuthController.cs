using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.API.Data;
using AuthService.API.Models;
using Common.Application.Controllers;
using Common.Application.Models;
using BCryptNet = BCrypt.Net.BCrypt; // <-- avoids naming collisions

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            var response = ApiResponse<object>.SuccessResult(new { status = "healthy", service = "AuthService", timestamp = DateTime.UtcNow });
            return HandleResult(response);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet("simple")]
        public IActionResult Simple()
        {
            return Ok(new { message = "AuthService is running", timestamp = DateTime.UtcNow });
        }

        [HttpGet("test")]
        public IActionResult TestGet()
        {
            var response = ApiResponse<object>.SuccessResult(new { message = "GET endpoint is working", timestamp = DateTime.UtcNow });
            return HandleResult(response);
        }

        [HttpPost("test")]
        public IActionResult Test([FromBody] object data)
        {
            var response = ApiResponse<object>.SuccessResult(new { message = "POST endpoint is working", receivedData = data, timestamp = DateTime.UtcNow });
            return HandleResult(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                {
                    var errorResponse = ApiResponse<object>.Failure("Email and password are required.");
                    return HandleResult(errorResponse);
                }

                var exists = _context.AuthUsers.Any(u => u.Email == req.Email);
                if (exists)
                {
                    var conflictResponse = ApiResponse<object>.Failure("Email already registered.");
                    return HandleResult(conflictResponse);
                }

                var user = new AuthUser
                {
                    Email = req.Email,
                    PhoneNumber = req.PhoneNumber ?? string.Empty,
                    PasswordHash = BCryptNet.HashPassword(req.Password),
                    IsVerified = false
                };

                _context.AuthUsers.Add(user);
                _context.SaveChanges();
                
                var successResponse = ApiResponse<object>.SuccessResult(new { message = "Registered successfully" });
                return HandleResult(successResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                var user = _context.AuthUsers.SingleOrDefault(u => u.Email == req.Email);
                if (user == null || !BCryptNet.Verify(req.Password, user.PasswordHash))
                {
                    var errorResponse = ApiResponse<object>.Failure("Invalid email or password");
                    return HandleResult(errorResponse);
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? "default-fallback-key");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(12),
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var successResponse = ApiResponse<object>.SuccessResult(new { token = tokenHandler.WriteToken(token) });
                return HandleResult(successResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] SendOtpRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.PhoneNumber))
                {
                    var errorResponse = ApiResponse<object>.Failure("Phone number required.");
                    return HandleResult(errorResponse);
                }

                var code = new Random().Next(100000, 999999).ToString();
                _context.OtpCodes.Add(new OtpCode
                {
                    PhoneNumber = req.PhoneNumber,
                    Code = code,                     // For production: store a hash of this.
                    Expiration = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false
                });
                _context.SaveChanges();

                // TODO: integrate SMS provider. For now we return the code for dev.
                var successResponse = ApiResponse<object>.SuccessResult(new { phone = req.PhoneNumber, code });
                return HandleResult(successResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            try
            {
                var otp = _context.OtpCodes
                    .FirstOrDefault(o => o.PhoneNumber == req.PhoneNumber && o.Code == req.Code && !o.IsUsed);

                if (otp == null || otp.Expiration < DateTime.UtcNow)
                {
                    var errorResponse = ApiResponse<object>.Failure("Invalid or expired OTP");
                    return HandleResult(errorResponse);
                }

                otp.IsUsed = true;
                _context.SaveChanges();

                // Optionally mark linked AuthUser.IsVerified = true if you tie OTP to a user.
                var successResponse = ApiResponse<object>.SuccessResult(new { message = "Phone verified" });
                return HandleResult(successResponse);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
