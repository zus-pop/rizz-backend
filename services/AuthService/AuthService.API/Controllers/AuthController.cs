using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.API.Data;
using AuthService.API.Models;
using BCryptNet = BCrypt.Net.BCrypt; // <-- avoids naming collisions

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Email and password are required.");

            var exists = _context.AuthUsers.Any(u => u.Email == req.Email);
            if (exists) return Conflict("Email already registered.");

            var user = new AuthUser
            {
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
                PasswordHash = BCryptNet.HashPassword(req.Password),
                IsVerified = false
            };

            _context.AuthUsers.Add(user);
            _context.SaveChanges();
            return Ok(new { message = "Registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _context.AuthUsers.SingleOrDefault(u => u.Email == req.Email);
            if (user == null || !BCryptNet.Verify(req.Password, user.PasswordHash))
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] SendOtpRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.PhoneNumber))
                return BadRequest("Phone number required.");

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
            return Ok(new { phone = req.PhoneNumber, code });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            var otp = _context.OtpCodes
                .FirstOrDefault(o => o.PhoneNumber == req.PhoneNumber && o.Code == req.Code && !o.IsUsed);

            if (otp == null || otp.Expiration < DateTime.UtcNow)
                return BadRequest("Invalid or expired OTP");

            otp.IsUsed = true;
            _context.SaveChanges();

            // Optionally mark linked AuthUser.IsVerified = true if you tie OTP to a user.
            return Ok(new { message = "Phone verified" });
        }
    }
}
