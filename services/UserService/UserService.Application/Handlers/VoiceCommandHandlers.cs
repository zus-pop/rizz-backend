using MediatR;
using UserService.Application.Commands;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Application.Services;
using Microsoft.Extensions.Logging;

namespace UserService.Application.Handlers
{
    public class VoiceCommandHandlers : 
        IRequestHandler<UploadVoiceCommand, VoiceUploadResultDto>,
        IRequestHandler<DeleteVoiceCommand>
    {
        private readonly IFileService _fileService;
        private readonly ILogger<VoiceCommandHandlers> _logger;

        public VoiceCommandHandlers(IFileService fileService, ILogger<VoiceCommandHandlers> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<VoiceUploadResultDto> Handle(UploadVoiceCommand request, CancellationToken cancellationToken)
        {
            using var stream = request.VoiceFile.OpenReadStream();
            var fileName = await _fileService.SaveVoiceFileAsync(request.UserId, stream, request.VoiceFile.FileName);

            _logger.LogInformation("Voice file uploaded successfully for user {UserId}: {FileName}", request.UserId, fileName);

            return new VoiceUploadResultDto
            {
                FileName = fileName,
                FilePath = $"/api/voice/user/{request.UserId}",
                FileSize = request.VoiceFile.Length,
                UploadedAt = DateTime.UtcNow,
                UserId = request.UserId
            };
        }

        public async Task Handle(DeleteVoiceCommand request, CancellationToken cancellationToken)
        {
            await _fileService.DeleteVoiceFileAsync(request.UserId);
            _logger.LogInformation("Voice file deleted for user {UserId}", request.UserId);
        }
    }
}