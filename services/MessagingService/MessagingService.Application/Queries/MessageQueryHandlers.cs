using AutoMapper;
using MediatR;
using MessagingService.Application.DTOs;
using MessagingService.Application.Interfaces;
using MessagingService.Application.Queries;

namespace MessagingService.Application.Queries
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public GetMessagesQueryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetMessagesByMatchIdAsync(request.MatchId, request.Page, request.PageSize, cancellationToken);
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }

    public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, MessageDto?>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public GetMessageByIdQueryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<MessageDto?> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
            return message != null ? _mapper.Map<MessageDto>(message) : null;
        }
    }

    public class GetUnreadMessagesQueryHandler : IRequestHandler<GetUnreadMessagesQuery, IEnumerable<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public GetUnreadMessagesQueryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetUnreadMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetUnreadMessagesByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }

    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, IEnumerable<MessageDto>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public GetConversationQueryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetConversationAsync(request.MatchId, request.Since, request.Limit, cancellationToken);
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }
}