using Microsoft.AspNetCore.Mvc;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MemberRewardApproval.WebApi.Hubs;

namespace MemberRewardApproval.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsWebhookController : ControllerBase
    {
        private readonly RewardService _rewardService;
        private readonly IHubContext<RequestHub> _hubContext;

        public TeamsWebhookController(RewardService rewardService, IHubContext<RequestHub> hubContext)
        {
            _rewardService = rewardService;
            _hubContext = hubContext;
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] CardActionPayload payload)
        {
            if (payload == null ||
                string.IsNullOrEmpty(payload.Action) ||
                string.IsNullOrEmpty(payload.RequestId))
            {
                return BadRequest();
            }

            var action = payload.Action;
            var requestId = payload.RequestId;

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

            await _hubContext.Clients.All.SendAsync("RequestStatusUpdated", new
            {
                requestId = payload.RequestId,
                // wynnId = payload.WynnId,
                status = action.Equals("approve", StringComparison.OrdinalIgnoreCase)
                        ? RewardStatus.Approved
                        : RewardStatus.Rejected
            });


            return Ok(new { requestId, action });
        }

    }
}
