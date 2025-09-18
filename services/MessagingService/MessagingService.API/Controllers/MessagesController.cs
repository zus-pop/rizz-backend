using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessagingService.Application.Commands;
using MessagingService.Application.Queries;
using MessagingService.Application.DTOs;

namespace MessagingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "MessagingService", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetAllMessages(CancellationToken cancellationToken)
        {
            var query = new GetMessagesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> GetMessageById(int id, CancellationToken cancellationToken)
        {
            var query = new GetMessageByIdQuery { MessageId = id };
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result == null)
                return NotFound(new { message = $"Message with id {id} not found" });
                
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetMessageById), new { id = result.Id }, result);
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesByMatch(
            int matchId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50, 
            CancellationToken cancellationToken = default)
        {
            var query = new GetConversationQuery 
            { 
                MatchId = matchId, 
                Limit = pageSize * page 
            };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult<MessageDto>> MarkAsRead(int id, CancellationToken cancellationToken)
        {
            var command = new MarkMessageAsReadCommand { MessageId = id };
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result == null)
                return NotFound(new { message = $"Message with id {id} not found" });
                
            return Ok(result);
        }

        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetUnreadMessages([FromQuery] int userId, CancellationToken cancellationToken)
        {
            var query = new GetUnreadMessagesQuery { UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MessageDto>> UpdateMessage(int id, [FromBody] UpdateMessageRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateMessageCommand { MessageId = id, Content = request.Content };
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result == null)
                return NotFound(new { message = $"Message with id {id} not found" });
                
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteMessageCommand { MessageId = id };
            var result = await _mediator.Send(command, cancellationToken);
            
            if (!result)
                return NotFound(new { message = $"Message with id {id} not found" });
                
            return Ok(new { message = $"Message {id} deleted successfully" });
        }
    }

    public class UpdateMessageRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}
