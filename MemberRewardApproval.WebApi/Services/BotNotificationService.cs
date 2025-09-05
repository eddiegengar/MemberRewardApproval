using MemberRewardApproval.WebApi.Models;
using MemberRewardApproval.WebApi.Options;
using MemberRewardApproval.WebApi.Services.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace MemberRewardApproval.WebApi.Services
{
    public class BotNotificationService : INotificationService
    {
        private readonly string _appId;
        private readonly CloudAdapter _adapter;
        private readonly ConversationReferenceService _conversationService;

        public BotNotificationService(IOptions<BotOptions> options, CloudAdapter adapter, ConversationReferenceService conversationService)
        {
            _appId = options.Value.MicrosoftAppId;
            _adapter = adapter;
            _conversationService = conversationService;
        }

        /// <summary>
        /// Sends an Adaptive Card proactively to the supervisor.
        /// </summary>
        public async Task SendApprovalCardAsync(
            string supervisorAadId,
            RewardRequest request,
            Dictionary<string, string> performanceData)
        {
            // 1. Get the conversation reference
            var conversationRef = await _conversationService.GetConversationReferenceAsync(supervisorAadId);
            if (conversationRef == null)
            {
                throw new InvalidOperationException(
                    "No conversation reference found for the supervisor. " +
                    "Ensure the bot has been installed and started a conversation.");
            }
            // if (!ConversationReferenceStore.TryGet(supervisorAadId, out var conversationRef))
            // {
            //     throw new InvalidOperationException(
            //         "No conversation reference found for the supervisor. " +
            //         "Ensure the bot has been installed and started a conversation.");
            // }

            // 2. Create Adaptive Card JSON
            var adaptiveCardJson = AdaptiveCardFactory.CreateRewardApprovalCard(request, performanceData);

            // 3. Send proactively via the CloudAdapter
            await _adapter.ContinueConversationAsync(
                _appId,
                conversationRef,
                async (turnContext, cancellationToken) =>
                {
                    // Parse JSON safely using Newtonsoft JObject
                    var attachment = new Attachment
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JObject.Parse(adaptiveCardJson)
                    };

                    // Create activity
                    var activity = MessageFactory.Attachment(attachment);

                    // Optional: log for debugging
                    Console.WriteLine($"Sending Adaptive Card to ConvId={conversationRef.Conversation.Id}, Channel={conversationRef.ChannelId}");

                    // Send activity
                    await turnContext.SendActivityAsync(activity, cancellationToken);
                },
                cancellationToken: default);
        }
    }
}