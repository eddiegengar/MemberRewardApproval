using Microsoft.Graph;
using Azure.Identity;
using MemberRewardApproval.WebApi.Models;
using Microsoft.Graph.Models;

namespace MemberRewardApproval.WebApi.Services
{
    public class TeamsNotificationService: INotificationService
    {
        private readonly GraphServiceClient _graph;

        public TeamsNotificationService(IConfiguration config)
        {
            var clientId = config["AzureAd:ClientId"];
            var tenantId = config["AzureAd:TenantId"];
            var clientSecret = config["AzureAd:ClientSecret"];

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _graph = new GraphServiceClient(credential);
        }

        /// <summary>
        /// Send an Adaptive Card to the supervisor via Teams using supervisor AAD ID.
        /// </summary>
        public async Task SendApprovalCardAsync(string supervisorAadId, RewardRequest request, Dictionary<string, string> performanceData)
        {
            var chat = await _graph.Chats.PostAsync(new Chat
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
            });

            var adaptiveCardJson = AdaptiveCardFactory.CreateRewardApprovalCard(request, performanceData);

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

            await _graph.Chats[chat.Id].Messages.PostAsync(chatMessage);
        }
    }
}
