using AutoMapper;
using MediatR;
using MessagingService.Application.Commands;
using MessagingService.Application.DTOs;
using MessagingService.Application.Interfaces;
using MessagingService.Domain.Entities;
using MessagingService.Domain.Enums;
using MessagingService.Domain.Events;

namespace MessagingService.Application.Commands
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public SendMessageCommandHandler(
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                MatchId = request.MatchId,
                SenderId = request.SenderId,
                Content = request.Content,
                Type = Enum.Parse<MessageType>(request.Type, true),
                ExtraData = request.ExtraData,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish domain event
            var domainEvent = new MessageSentEvent(message.Id, message.MatchId, message.SenderId, message.Content, message.CreatedAt);
            await _mediator.Publish(domainEvent, cancellationToken);

            return _mapper.Map<MessageDto>(message);
        }
    }

    public class MarkMessageAsReadCommandHandler : IRequestHandler<MarkMessageAsReadCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public MarkMessageAsReadCommandHandler(
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<MessageDto> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
            if (message == null)
                throw new KeyNotFoundException($"Message with ID {request.MessageId} not found");

            message.ReadAt = DateTime.UtcNow;
            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish domain event
            var domainEvent = new MessageReadEvent(message.Id, message.MatchId, request.ReaderId, message.ReadAt.Value);
            await _mediator.Publish(domainEvent, cancellationToken);

            return _mapper.Map<MessageDto>(message);
        }
    }

    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMessageCommandHandler(
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MessageDto> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
            if (message == null)
                throw new KeyNotFoundException($"Message with ID {request.MessageId} not found");

            if (!string.IsNullOrEmpty(request.Content))
                message.Content = request.Content;

            if (request.ExtraData != null)
                message.ExtraData = request.ExtraData;

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<MessageDto>(message);
        }
    }

    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMessageCommandHandler(
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
            if (message == null)
                return false;

            // Check if user has permission to delete (must be sender)
            if (message.SenderId != request.UserId)
                throw new UnauthorizedAccessException("You can only delete your own messages");

            _messageRepository.Delete(message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}