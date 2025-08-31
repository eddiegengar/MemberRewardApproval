using MemberRewardApproval.WebApi.Hubs;
using MemberRewardApproval.WebApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace MemberRewardApproval.WebApi.Services
{
    public class WebNotificationService : INotificationService
    {
        private readonly IHubContext<RewardHub> _hub;
        public WebNotificationService(IHubContext<RewardHub> hub)
        {
            _hub = hub;
        }

        public async Task SendApprovalCardAsync(string supervisorId, RewardRequest request, Dictionary<string, string> performanceData)
        {
            // Generate the Adaptive Card JSON
            var adaptiveCardJson = AdaptiveCardFactory.CreateRewardApprovalCard(request, performanceData);

            // Save to file or log
            var fileName = $"Card_{request.RequestId}.json";
            System.IO.File.WriteAllText(fileName, adaptiveCardJson);

            Console.WriteLine($"Adaptive Card JSON generated: {fileName}");
            Console.WriteLine($"Supervisor {supervisorId} can approve/reject via web interface at /rewardrequests/card?requestId={request.RequestId}");

            // Send to supervisor via SignalR
            if (RewardHub.TryGetConnection(supervisorId, out var connectionId))
            {
                await _hub.Clients.Client(connectionId).SendAsync("ReceiveCard", adaptiveCardJson);
            }
            else
            {
                Console.WriteLine($"Supervisor {supervisorId} not connected.");
            }
        }
    }
}
