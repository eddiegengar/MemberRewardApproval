using MemberRewardApproval.WebApi.Models;
using MemberRewardApproval.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public RewardRequestsController(RewardService rewardService, GraphUserService graphUserService, INotificationService notificationService)
        {
            _rewardService = rewardService;
            _graphService = graphUserService;
            _notificationService = notificationService;
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
                var supervisorId = await _graphService.GetUserAadIdByEmailAsync("it@winson-group.com");
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


    }
}
