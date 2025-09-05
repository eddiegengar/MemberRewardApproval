using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Models;
using MemberRewardApproval.WebApi.Data;

namespace MemberRewardApproval.WebApi.Services.Bots
{
    public class RewardBot : ActivityHandler
    {
        // ngrok http http://localhost:5172
        private RewardService _rewardService;

        public RewardBot(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet only non-bot users
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        MessageFactory.Text($"Welcome to the Reward Approval Bot!"),
                        cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("OnMessageActivityAsync triggered");
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

            // Return 200 OK to Teams to acknowledge the invoke
            return new InvokeResponse { Status = 200 };
        }
    }
}



