using System.Text.Json;
using MemberRewardApproval.WebApi.Models;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.AspNetCore.SignalR;
using MemberRewardApproval.WebApi.Services.Bots;
using MemberRewardApproval.WebApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MemberRewardApproval.WebApi.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class BotController : ControllerBase
    {
        private readonly IBot _bot;
        private readonly CloudAdapter _adapter;
        private readonly RewardService _rewardService;
        private readonly IHubContext<RequestHub> _requestHub;
        private readonly string _botAppId;
        private readonly ConversationReferenceService _conversationService;
        private readonly ILogger<BotController> _logger;

        public BotController(
            IBot bot,
            CloudAdapter adapter,
            RewardService rewardService,
            IHubContext<RequestHub> requestHub,
            IOptions<BotOptions> botOptions,
            ConversationReferenceService conversationService,
            ILogger<BotController> logger)
        {
            _bot = bot;
            _adapter = adapter;
            _rewardService = rewardService;
            _requestHub = requestHub;
            _botAppId = botOptions.Value.MicrosoftAppId;
            _conversationService = conversationService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task PostAsync([FromBody] Activity activity)
        {
            if (activity == null)
                return;

            // Save or update and get the ConversationReference in one step
            var conversationRef = await _conversationService.SaveOrUpdateConversationReferenceAsync(activity);
            if (conversationRef == null)
            {
                _logger.LogWarning("ConversationReference not saved because Activity.From.AadObjectId is missing. Activity type: {ActivityType}, From.Id: {FromId}",
                    activity.Type, activity.From?.Id);
                return;
            }

            // Handle Adaptive Card button click
            if (activity.Type == ActivityTypes.Message && activity.Value != null)
            {
                var jsonElement = (JsonElement)activity.Value;

                // Deserialize card action payload
                var payload = JsonSerializer.Deserialize<CardActionPayload>(
                    jsonElement.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payload != null)
                {
                    var action = payload.Action?.ToLowerInvariant();
                    var requestId = payload.RequestId;

                    // Update reward status
                    if (action == "approve")
                        await _rewardService.UpdateRequestStatusAsync(requestId, RewardStatus.Approved);
                    else if (action == "reject")
                        await _rewardService.UpdateRequestStatusAsync(requestId, RewardStatus.Rejected);

                    // Send confirmation back to Teams
                    await _adapter.ContinueConversationAsync(
                        botAppId: _botAppId,
                        reference: conversationRef,
                        callback: async (turnContext, cancellationToken) =>
                        {
                            await turnContext.SendActivityAsync(
                                MessageFactory.Text("Your action has been processed."),
                                cancellationToken);
                        },
                        cancellationToken: default);

                    // Trigger SignalR event to update Angular UI
                    await _requestHub.Clients.All.SendAsync("RewardRequestUpdated", new
                    {
                        requestId,
                        status = action.Equals("approve", StringComparison.OrdinalIgnoreCase)
                        ? RewardStatus.Approved
                        : RewardStatus.Rejected
                    });
                }
            }
            else
            {
                await _adapter.ContinueConversationAsync(
                    botAppId: _botAppId,
                    reference: conversationRef,
                    callback: async (turnContext, cancellationToken) =>
                    {
                        await turnContext.SendActivityAsync(
                            MessageFactory.Text("Conversation reference saved."),
                            cancellationToken);
                    },
                    cancellationToken: default);
            }
            return;
        }
    }
}