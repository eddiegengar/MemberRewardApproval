using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Models;

namespace MemberRewardApproval.WebApi.Services.Bots
{
    public class RewardBot : ActivityHandler
    {
        private readonly RewardService _rewardService;

        public RewardBot(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationRef = turnContext.Activity.GetConversationReference();

            // Store this somewhere persistent keyed by user ID
            ConversationReferenceStore.Save(turnContext.Activity.From.Id, conversationRef);

            await turnContext.SendActivityAsync("Hi! I got your conversation reference.", cancellationToken: cancellationToken);
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

                if (payload?.Data != null)
                {
                    string requestId = payload.Data.RequestId;
                    string action = payload.Data.Action;

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

            // Return 200 OK to Teams to acknowledge the invoke
            return new InvokeResponse { Status = 200 };
        }
    }
}



