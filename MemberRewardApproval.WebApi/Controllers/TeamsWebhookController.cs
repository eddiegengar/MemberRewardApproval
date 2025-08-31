using Microsoft.AspNetCore.Mvc;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Models;
using System.Threading.Tasks;

namespace MemberRewardApproval.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsWebhookController : ControllerBase
    {
        private readonly RewardService _rewardService;

        public TeamsWebhookController(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

       [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] CardActionPayload payload)
        {
            if (payload?.Data == null ||
                string.IsNullOrEmpty(payload.Data.Action) ||
                string.IsNullOrEmpty(payload.Data.RequestId))
            {
                return BadRequest();
            }

            var action = payload.Data.Action;
            var requestId = payload.Data.RequestId;

            if (action.Equals("approve", StringComparison.OrdinalIgnoreCase))
            {
                await _rewardService.UpdateRequestStatusAsync(requestId, RewardStatus.Approved);
            }
            else if (action.Equals("reject", StringComparison.OrdinalIgnoreCase))
            {
                await _rewardService.UpdateRequestStatusAsync(requestId, RewardStatus.Rejected);
            }
            else
            {
                return BadRequest(new { message = "Unknown action" });
            }

            return Ok(new { requestId, action, supervisorId = payload.SupervisorId });
        }

    }
}
