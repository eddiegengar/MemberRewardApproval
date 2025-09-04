using Microsoft.AspNetCore.SignalR;
using MemberRewardApproval.WebApi.Models;
using MemberRewardApproval.WebApi.Services;
using Microsoft.AspNetCore.Components;

namespace MemberRewardApproval.WebApi.Hubs
{
    [Route("hubs/request")]
    public class RequestHub : Hub
    {
        private readonly RewardService _rewardService;
        private readonly GraphUserService _graphService;
        private readonly INotificationService _notificationService;


        public RequestHub(RewardService rewardService, GraphUserService graphService, INotificationService notificationService)
        {
            _rewardService = rewardService;
            _graphService = graphService;
            _notificationService = notificationService;
        }

        // Client -> Server
        public async Task SubmitRewardRequest(RewardRequestDto requestDto)
        {
            try
            {
                // 1. Create reward request (generates RequestId)
                var request = await _rewardService.CreateRequestAsync(
                    requestDto.WynnId, requestDto.RewardType, requestDto.RequestedValue);

                // 2. Get supervisor ID and member performance
                var supervisorId = await _graphService.GetUserAadIdByEmailAsync("it@winson-group.com");
                var performanceData = await _rewardService.GetMemberPerformanceAsync(requestDto.WynnId);

                // 3. Send adaptive card to supervisor
                await _notificationService.SendApprovalCardAsync(supervisorId, request, performanceData);

                // 4. Send the created request back to the Angular client
                await Clients.Caller.SendAsync("RequestSubmitted", request);
            }
            catch (InvalidOperationException ex)
            {
                // Optionally, send error back to client
                await Clients.Caller.SendAsync("RequestFailed", ex.Message);
            }
        }

        public async Task NotifyRequestStatusChanged(string requestId, string wynnId, string status)
        {
            await Clients.All.SendAsync("RequestStatusUpdated", new { requestId, wynnId, status });
        }

    }
}
