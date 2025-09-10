using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Text.Json;
using MemberRewardApproval.WebApi.Models;
using Microsoft.AspNetCore.SignalR;
using MemberRewardApproval.WebApi.Hubs;

namespace MemberRewardApproval.WebApi.Services.Bots
{
    public class RewardBot : ActivityHandler
    {
        private RewardService _rewardService;
        private readonly ConversationReferenceService _conversationService;
        private readonly IHubContext<RequestHub> _requestHub;
        private readonly ILogger<RewardBot> _logger;
        public RewardBot(
            RewardService rewardService,
            ConversationReferenceService conversationService,
            IHubContext<RequestHub> requestHub,
            ILogger<RewardBot> logger)
        {
            _rewardService = rewardService;
            _conversationService = conversationService;
            _requestHub = requestHub;
            _logger = logger;
        }

        /// <summary>
        /// Handles incoming messages: saves the conversation reference and
        /// delegates adaptive card actions if present.
        /// </summary>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message received: {Text}", turnContext.Activity.Text);

            // Save conversation reference
            var reference = await _conversationService.SaveOrUpdateConversationReferenceAsync(turnContext.Activity);

            if (turnContext.Activity.Value != null)
            {
                await HandleCardActionAsync(turnContext, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text($"Conversation reference for ConversationId={reference?.Conversation.Id} saved."),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Processes an adaptive card action: updates the reward status,
        /// notifies SignalR clients, and sends a confirmation message to the user.
        /// </summary>
        private async Task<RewardRequest?> HandleCardActionAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Deserialize<CardActionPayload>(
                turnContext.Activity.Value.ToString(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null) throw new InvalidOperationException("Invalid card payload.");

            // Update reward
            var status = payload.Action?.ToLowerInvariant() == "approve"
                     ? RewardStatus.Approved
                     : RewardStatus.Rejected;
            var updatedRequest = await _rewardService.UpdateRequestStatusAsync(payload.RequestId, status);

            // Notify Hub and user
            await _requestHub.Clients.All.SendAsync("RewardRequestUpdated", updatedRequest, cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text(
                $"Request {updatedRequest.RequestId} has been {updatedRequest.Status}."),
                cancellationToken);

            return updatedRequest;
        }
    }
}



