using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MemberRewardApproval.WebApi.Hubs
{
    public class RewardHub : Hub
    {
        // Map logical supervisorId -> SignalR connectionId
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public override Task OnConnectedAsync()
        {
            // Get supervisorId from frontend query string
            var supervisorId = Context.GetHttpContext()?.Request.Query["supervisorId"].ToString();

            if (!string.IsNullOrEmpty(supervisorId))
            {
                _connections[supervisorId] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var supervisorId = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (supervisorId != null)
            {
                _connections.TryRemove(supervisorId, out _); // discard the value
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendCardToSupervisor(string supervisorId, string cardJson)
        {
            if (_connections.TryGetValue(supervisorId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveCard", cardJson);
            }
            else
            {
                // Supervisor is not connected, handle as needed
                Console.WriteLine($"Supervisor {supervisorId} not connected.");
            }
        }

            // Helper for WebNotificationService to get connectionId
        public static bool TryGetConnection(string supervisorId, out string connectionId)
            => _connections.TryGetValue(supervisorId, out connectionId);


    }
}
