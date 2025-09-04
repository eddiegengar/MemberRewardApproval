using MemberRewardApproval.WebApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Graph.Models;
using MemberRewardApproval.WebApi.Options;

namespace MemberRewardApproval.WebApi.Services
{
    public class TeamsNotificationService : INotificationService
    {
        private readonly GraphServiceClient _graph;

        public TeamsNotificationService(IOptions<BotOptions> options)
        {
            var tenantId = options.Value.TenantId;
            var clientId = options.Value.AppId;
            var clientSecret = options.Value.AppPassword;

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _graph = new GraphServiceClient(credential);
        }

        /// <summary>
        /// Send an Adaptive Card to the supervisor via Teams using supervisor AAD ID.
        /// </summary>
        public async Task SendApprovalCardAsync(string supervisorAadId, RewardRequest request, Dictionary<string, string> performanceData)
        {
            // 1. Create or get 1:1 chat with supervisor
            var chat = new Chat
            {
                ChatType = ChatType.OneOnOne,
                Members = new List<ConversationMember>
                {
                    new AadUserConversationMember
                    {
                        Roles = new List<string> { "owner" },
                        AdditionalData = new Dictionary<string, object>
                        {
                            { "user@odata.bind", $"https://graph.microsoft.com/v1.0/users/{supervisorAadId}" }
                        }
                    }
                }
            };

            var createdChat = await _graph.Chats.PostAsync(chat);

            // 2. Create Adaptive Card JSON
            var adaptiveCardJson = AdaptiveCardFactory.CreateRewardApprovalCard(request, performanceData);

            // 3. Send the adaptive card as a chat message
            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = "You have a reward request pending approval."
                },
                Attachments = new List<ChatMessageAttachment>
                {
                    new ChatMessageAttachment
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = adaptiveCardJson
                    }
                }
            };

            await _graph.Chats[createdChat.Id].Messages.PostAsync(chatMessage);
        }
    }
}
