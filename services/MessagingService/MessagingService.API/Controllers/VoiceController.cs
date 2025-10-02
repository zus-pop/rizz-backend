using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessagingService.Application.Commands;
using MessagingService.Application.DTOs;
using MessagingService.Application.Queries;
using System.Security.Claims;

namespace MessagingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VoiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<VoiceController> _logger;
        private readonly IWebHostEnvironment _environment;

        public VoiceController(IMediator mediator, ILogger<VoiceController> logger, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Upload voice message for a conversation/match
        /// </summary>
        [HttpPost("match/{matchId}/upload")]
        public async Task<IActionResult> UploadVoiceMessage(int matchId, [FromForm] IFormFile voiceFile)
        {
            try
            {
                if (voiceFile == null || voiceFile.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No voice file provided" });
                }

                // Validate file type (audio files only)
                var allowedTypes = new[] { "audio/mpeg", "audio/wav", "audio/mp3", "audio/m4a", "audio/aac" };
                if (!allowedTypes.Contains(voiceFile.ContentType.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only audio files are allowed." });
                }

                // Validate file size (max 5MB for voice messages)
                if (voiceFile.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "File size too large. Maximum 5MB allowed." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var senderId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var command = new SendVoiceMessageCommand
                {
                    MatchId = matchId,
                    SenderId = senderId,
                    VoiceFile = voiceFile
                };

                var result = await _mediator.Send(command);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading voice message for match {MatchId}", matchId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get voice file for a message
        /// </summary>
        [HttpGet("message/{messageId}/file")]
        public async Task<IActionResult> GetVoiceMessageFile(int messageId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var query = new GetVoiceMessageFileQuery 
                { 
                    MessageId = messageId,
                    RequesterId = userId
                };

                var result = await _mediator.Send(query);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Voice message not found or access denied" });
                }

                return File(result.FileStream, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving voice file for message {MessageId}", messageId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get voice messages for a match/conversation
        /// </summary>
        [HttpGet("match/{matchId}/messages")]
        public async Task<IActionResult> GetVoiceMessages(int matchId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var query = new GetVoiceMessagesQuery 
                { 
                    MatchId = matchId,
                    RequesterId = userId
                };

                var result = await _mediator.Send(query);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voice messages for match {MatchId}", matchId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}