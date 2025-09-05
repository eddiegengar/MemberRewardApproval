using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using MemberRewardApproval.WebApi.Models;

namespace MemberRewardApproval.WebApi.Services.Bots
{
    public class RewardBot : ActivityHandler
    {
        private RewardService _rewardService;

        public RewardBot(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

        protected override async Task<InvokeResponse> OnInvokeActivityAsync(
            ITurnContext<IInvokeActivity> turnContext,
            CancellationToken cancellationToken)
        {
            // Handle Adaptive Card submit actions
            var value = turnContext.Activity.Value;

            if (value != null)
            {
                var payload = JsonSerializer.Deserialize<CardActionPayload>(value.ToString());

                if (payload != null)
                {
                    string requestId = payload.RequestId;
                    string action = payload.Action;

                    if (action == "approve")
                    {
                        await _rewardService.UpdateRequestStatusAsync(requestId, RewardStatus.Approved);
                    }
                    else if (action == "reject")
                    {
                        await _rewardService.UpdateRequestStatusAsync(requestId, RewardStatus.Rejected);
                    }
                }
            }

            return new InvokeResponse { Status = 200 };
        }
    }
}



