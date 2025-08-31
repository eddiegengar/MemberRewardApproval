using MemberRewardApproval.WebApi.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Graph;
using Azure.Identity;
using System.Text.Json;

namespace MemberRewardApproval.WebApi.Services
{
    public class BotNotificationService : INotificationService
    {
        private readonly string _appId;
        private readonly string _appPassword;
        private readonly string _serviceUrl;

        private readonly GraphServiceClient _graph; // to resolve email → AAD user

        public BotNotificationService(IConfiguration config)
        {
            _appId = config["Bot:AppId"];
            _appPassword = config["Bot:AppPassword"];
            _serviceUrl = config["Bot:ServiceUrl"];

            // Graph client (app permissions)
            var credential = new ClientSecretCredential(
                config["AzureAd:TenantId"],
                config["AzureAd:ClientId"],
                config["AzureAd:ClientSecret"]);
            _graph = new GraphServiceClient(credential);
        }

        /// <summary>
        /// Send an Adaptive Card to the supervisor via Bot Framework.
        /// Accepts supervisor email, resolves to AAD ID.
        /// </summary>
        public async Task SendApprovalCardAsync(string supervisorAadId, RewardRequest request, Dictionary<string, string> performanceData)
        {
            // 1. Create connector client
            var credentials = new MicrosoftAppCredentials(_appId, _appPassword);
            using var connector = new ConnectorClient(new Uri(_serviceUrl), credentials);

            // 2. Create a 1:1 conversation with the supervisor
            var conversation = await connector.Conversations.CreateConversationAsync(new ConversationParameters
            {
                Bot = new ChannelAccount(id: _appId),
                Members = new List<ChannelAccount>
                {
                    new ChannelAccount(id: supervisorAadId)
                }
            });

            // 3. Create Adaptive Card JSON
            var adaptiveCardJson = AdaptiveCardFactory.CreateRewardApprovalCard(request, performanceData);

            // 4. Create activity
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                Conversation = new ConversationAccount(id: conversation.Id),
                From = new ChannelAccount(id: _appId),
                Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonSerializer.Deserialize<object>(adaptiveCardJson)
                    }
                }
            };

            // 5. Send message
            await connector.Conversations.SendToConversationAsync(activity);
        }

    }
}
