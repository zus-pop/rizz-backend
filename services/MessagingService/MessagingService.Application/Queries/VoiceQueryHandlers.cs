using MediatR;
using Microsoft.Extensions.Logging;
using MessagingService.Application.Queries;
using MessagingService.Application.DTOs;
using MessagingService.Application.Interfaces;
using MessagingService.Domain.Enums;
using AutoMapper;

namespace MessagingService.Application.Queries
{
    public class VoiceQueryHandlers : 
        IRequestHandler<GetVoiceMessageFileQuery, VoiceFileResultDto?>,
        IRequestHandler<GetVoiceMessagesQuery, IEnumerable<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VoiceQueryHandlers> _logger;

        public VoiceQueryHandlers(
            IMessageRepository messageRepository,
            IMapper mapper,
            ILogger<VoiceQueryHandlers> logger)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<VoiceFileResultDto?> Handle(GetVoiceMessageFileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(request.MessageId);
                
                if (message == null || message.Type != MessageType.Voice || message.IsDeleted)
                {
                    return null;
                }

                // Verify user has access to this message (same match)
                // In a real implementation, you'd check if the user is part of the match
                // For now, we'll assume basic validation

                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploaded-voice", "messages");
                var filePath = Path.Combine(uploadDir, message.Content);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Voice file not found: {FilePath}", filePath);
                    return null;
                }

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var contentType = GetContentType(message.Content);

                return new VoiceFileResultDto
                {
                    FileStream = fileStream,
                    ContentType = contentType,
                    FileName = message.Content
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving voice file for message {MessageId}", request.MessageId);
                return null;
            }
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetVoiceMessagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var messages = await _messageRepository.GetVoiceMessagesByMatchIdAsync(request.MatchId);
                return _mapper.Map<IEnumerable<MessageDto>>(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voice messages for match {MatchId}", request.MatchId);
                throw;
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".m4a" => "audio/m4a",
                ".aac" => "audio/aac",
                _ => "application/octet-stream"
            };
        }
    }
}