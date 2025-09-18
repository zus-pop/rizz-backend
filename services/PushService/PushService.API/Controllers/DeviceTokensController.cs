using MediatR;
using Microsoft.AspNetCore.Mvc;
using PushService.Application.Commands;
using PushService.Application.DTOs;
using PushService.Application.Queries;

namespace PushService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceTokensController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceTokensController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] RegisterDeviceTokenDto dto)
        {
            try
            {
                var command = new RegisterDeviceTokenCommand
                {
                    UserId = dto.UserId,
                    Token = dto.Token,
                    DeviceType = dto.DeviceType,
                    DeviceId = dto.DeviceId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Device token registered successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDeviceTokens(int userId, [FromQuery] bool activeOnly = true)
        {
            try
            {
                var query = new GetDeviceTokensByUserIdQuery
                {
                    UserId = userId,
                    ActiveOnly = activeOnly
                };

                var tokens = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = tokens,
                    count = tokens.Count()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeviceToken(int id, [FromBody] UpdateDeviceTokenDto dto)
        {
            try
            {
                var command = new UpdateDeviceTokenCommand
                {
                    Id = id,
                    Token = dto.Token,
                    DeviceId = dto.DeviceId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Device token updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateDeviceToken(int id)
        {
            try
            {
                var command = new DeactivateDeviceTokenCommand { Id = id };
                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result,
                    message = result ? "Device token deactivated successfully" : "Device token not found"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceToken(int id)
        {
            try
            {
                var command = new DeleteDeviceTokenCommand { Id = id };
                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result,
                    message = "Device token deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("cleanup-expired")]
        public async Task<IActionResult> CleanupExpiredTokens([FromQuery] int expirationDays = 30)
        {
            try
            {
                var command = new CleanupExpiredTokensCommand
                {
                    ExpirationTime = TimeSpan.FromDays(expirationDays)
                };

                var count = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    message = $"Cleaned up {count} expired tokens",
                    count = count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}