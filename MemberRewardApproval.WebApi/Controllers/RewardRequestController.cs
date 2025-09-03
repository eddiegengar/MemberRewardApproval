using Microsoft.AspNetCore.Mvc;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Models;

namespace MemberRewardApproval.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardRequestsController : ControllerBase
    {
        private readonly RewardService _rewardService;
        private readonly INotificationService _notificationService;

        public RewardRequestsController(RewardService rewardService, INotificationService notificationService)
        {
            _rewardService = rewardService;
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
                var supervisorId = await _rewardService.GetSupervisorAadIdAsync("eddiegengar@gmail.com");
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
