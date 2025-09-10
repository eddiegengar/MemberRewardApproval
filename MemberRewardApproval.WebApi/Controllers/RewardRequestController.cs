using MemberRewardApproval.WebApi.Hubs;
using MemberRewardApproval.WebApi.Models;
using MemberRewardApproval.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MemberRewardApproval.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RewardRequestsController : ControllerBase
    {
        private readonly RewardService _rewardService;
        private readonly GraphUserService _graphService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<RequestHub> _hubContext;

        public RewardRequestsController(RewardService rewardService, GraphUserService graphUserService, INotificationService notificationService, IHubContext<RequestHub> hubContext)
        {
            _rewardService = rewardService;
            _graphService = graphUserService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _rewardService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RewardRequestDto requestDto)
        {
            try
            {
                // 1. Create reward request (ensures no pending duplicate)
                var request = await _rewardService.CreateRequestAsync(
                    requestDto.WynnId, requestDto.RewardType, requestDto.RequestedValue);

                // 2. Retrieve supervisor ID and member performance
                var supervisorId = await _graphService.GetUserAadIdByEmailAsync("eddie.pang@winson-group.com");
                var performanceData = await _rewardService.GetMemberPerformanceAsync(requestDto.WynnId);

                // 3. Send Adaptive Card via notification service
                await _notificationService.SendApprovalCardAsync(supervisorId, request, performanceData);

                return Ok(request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("[action]")]
        public async Task TestSingalR()
        {
            await _hubContext.Clients.All.SendAsync("RequestStatusUpdated", "hello from controller");
            return;
        }
    }
}
