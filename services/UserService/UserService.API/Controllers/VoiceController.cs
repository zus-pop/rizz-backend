using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Application.Services;
using System.Security.Claims;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VoiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<VoiceController> _logger;
        private readonly IFileService _fileService;

        public VoiceController(IMediator mediator, ILogger<VoiceController> logger, IFileService fileService)
        {
            _mediator = mediator;
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Upload voice file for user profile
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadVoice([FromForm] IFormFile voiceFile)
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

                // Validate file size (max 10MB)
                if (voiceFile.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "File size too large. Maximum 10MB allowed." });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var command = new UploadVoiceCommand
                {
                    UserId = int.Parse(userId),
                    VoiceFile = voiceFile
                };

                var result = await _mediator.Send(command);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading voice file");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get voice file URL for a user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserVoice(int userId)
        {
            try
            {
                var query = new GetUserVoiceQuery { UserId = userId };
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { success = false, message = "Voice file not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user voice");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get voice file stream for a user
        /// </summary>
        [HttpGet("user/{userId}/file")]
        public async Task<IActionResult> GetUserVoiceFile(int userId)
        {
            try
            {
                var fileInfo = await _fileService.GetVoiceFileAsync(userId);
                
                if (fileInfo == null)
                {
                    return NotFound(new { success = false, message = "Voice file not found" });
                }

                return File(fileInfo.Value.FileStream, fileInfo.Value.ContentType, fileInfo.Value.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving voice file for user {UserId}", userId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete voice file for authenticated user
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteVoice()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var command = new DeleteVoiceCommand { UserId = int.Parse(userId) };
                await _mediator.Send(command);

                return Ok(new { success = true, message = "Voice file deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voice file");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}