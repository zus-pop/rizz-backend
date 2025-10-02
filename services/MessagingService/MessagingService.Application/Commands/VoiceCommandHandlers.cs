using MediatR;
using Microsoft.Extensions.Logging;
using MessagingService.Application.Commands;
using MessagingService.Application.DTOs;
using MessagingService.Application.Interfaces;
using MessagingService.Domain.Entities;
using MessagingService.Domain.Enums;
using AutoMapper;

namespace MessagingService.Application.Commands
{
    public class VoiceCommandHandlers : IRequestHandler<SendVoiceMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VoiceCommandHandlers> _logger;

        public VoiceCommandHandlers(
            IMessageRepository messageRepository,
            IMapper mapper,
            ILogger<VoiceCommandHandlers> logger)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MessageDto> Handle(SendVoiceMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create upload directory if it doesn't exist
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploaded-voice", "messages");
                Directory.CreateDirectory(uploadDir);

                // Generate unique filename
                var fileExtension = Path.GetExtension(request.VoiceFile.FileName);
                var fileName = $"voice_{request.MatchId}_{request.SenderId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadDir, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.VoiceFile.CopyToAsync(stream, cancellationToken);
                }

                // Create message entity
                var message = new Message
                {
                    MatchId = request.MatchId,
                    SenderId = request.SenderId,
                    Content = fileName, // Store filename in content for voice messages
                    Type = MessageType.Voice,
                    ExtraData = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        originalFileName = request.VoiceFile.FileName,
                        fileSize = request.VoiceFile.Length,
                        duration = (int?)null // Could be populated later if needed
                    })
                };

                var savedMessage = await _messageRepository.CreateAsync(message);

                _logger.LogInformation("Voice message uploaded successfully for match {MatchId} by user {SenderId}: {FileName}", 
                    request.MatchId, request.SenderId, fileName);

                return _mapper.Map<MessageDto>(savedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading voice message for match {MatchId}", request.MatchId);
                throw;
            }
        }
    }
}