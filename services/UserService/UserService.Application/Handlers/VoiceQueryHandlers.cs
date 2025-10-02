using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Queries;
using UserService.Application.Services;
using Microsoft.Extensions.Logging;

namespace UserService.Application.Handlers
{
    public class VoiceQueryHandlers : 
        IRequestHandler<GetUserVoiceQuery, VoiceFileDto?>
    {
        private readonly IFileService _fileService;
        private readonly ILogger<VoiceQueryHandlers> _logger;

        public VoiceQueryHandlers(IFileService fileService, ILogger<VoiceQueryHandlers> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<VoiceFileDto?> Handle(GetUserVoiceQuery request, CancellationToken cancellationToken)
        {
            var fileInfo = await _fileService.GetVoiceFileAsync(request.UserId);
            
            if (fileInfo == null)
            {
                return null;
            }

            // We don't need the stream here, just the metadata
            await fileInfo.Value.FileStream.DisposeAsync();

            return new VoiceFileDto
            {
                FileName = fileInfo.Value.FileName,
                FilePath = $"/api/voice/user/{request.UserId}",
                FileSize = 0, // We would need to calculate this
                UploadedAt = DateTime.UtcNow, // We would need to get this from file metadata
                UserId = request.UserId
            };
        }
    }
}