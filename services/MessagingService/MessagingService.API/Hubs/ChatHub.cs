using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MessagingService.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                _logger.LogInformation("User {UserId} connected to chat with connection {ConnectionId}", userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                _logger.LogInformation("User {UserId} disconnected from chat", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinMatch(string matchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"match_{matchId}");
            _logger.LogDebug("Connection {ConnectionId} joined match {MatchId}", Context.ConnectionId, matchId);
        }

        public async Task LeaveMatch(string matchId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"match_{matchId}");
            _logger.LogDebug("Connection {ConnectionId} left match {MatchId}", Context.ConnectionId, matchId);
        }

        public async Task SendTypingIndicator(string matchId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Clients.GroupExcept($"match_{matchId}", Context.ConnectionId)
                .SendAsync("UserTyping", new { userId, matchId });
        }

        public async Task StopTypingIndicator(string matchId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Clients.GroupExcept($"match_{matchId}", Context.ConnectionId)
                .SendAsync("UserStoppedTyping", new { userId, matchId });
        }
    }
}
