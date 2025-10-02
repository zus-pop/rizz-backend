using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpPost("register")]
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
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var command = new LoginUserCommand(req.Email, req.Password);
                var token = await _mediator.Send(command);
                
                if (token == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                var userQuery = new GetUserByEmailQuery(req.Email);
                var user = await _mediator.Send(userQuery);

                return Ok(new { 
                    message = "Login successful", 
                    token = token,
                    userId = user?.Id,
                    email = user?.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }
    }
}
