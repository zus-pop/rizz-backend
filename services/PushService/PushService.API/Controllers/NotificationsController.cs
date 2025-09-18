using MediatR;
using Microsoft.AspNetCore.Mvc;
using PushService.Application.Commands;
using PushService.Application.DTOs;

namespace PushService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto)
        {
            try
            {
                var command = new SendPushNotificationCommand
                {
                    Token = dto.Token,
                    UserId = dto.UserId,
                    UserIds = dto.UserIds,
                    Notification = dto.Notification,
                    SendToAll = dto.SendToAll
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result,
                    message = result ? "Notification sent successfully" : "Failed to send notification"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("send-to-user/{userId}")]
        public async Task<IActionResult> SendNotificationToUser(int userId, [FromBody] PushNotificationDto notification)
        {
            try
            {
                var command = new SendPushNotificationCommand
                {
                    UserId = userId,
                    Notification = notification
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result,
                    message = result ? $"Notification sent to user {userId}" : "Failed to send notification"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("send-to-users")]
        public async Task<IActionResult> SendNotificationToUsers([FromBody] SendNotificationDto dto)
        {
            try
            {
                if (dto.UserIds == null || !dto.UserIds.Any())
                {
                    return BadRequest(new { success = false, message = "UserIds cannot be empty" });
                }

                var command = new SendPushNotificationCommand
                {
                    UserIds = dto.UserIds,
                    Notification = dto.Notification
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result,
                    message = result ? $"Notification sent to {dto.UserIds.Count()} users" : "Failed to send notification"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> BroadcastNotification([FromBody] PushNotificationDto notification)
        {
            try
            {
                var command = new SendPushNotificationCommand
                {
                    Notification = notification,
                    SendToAll = true
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result,
                    message = result ? "Broadcast notification sent successfully" : "Failed to send broadcast notification"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}