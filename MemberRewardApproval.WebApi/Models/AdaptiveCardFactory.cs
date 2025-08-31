using System.Text.Json;
using System.Text.Json.Serialization;

namespace MemberRewardApproval.WebApi.Models
{
    public static class AdaptiveCardFactory
    {
        private class AdaptiveCard
        {
            [JsonPropertyName("$schema")]
            public string Schema { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; } = "AdaptiveCard";
            [JsonPropertyName("version")]
            public string Version { get; set; } = "1.4";
            [JsonPropertyName("body")]
            public object[] Body { get; set; }
            [JsonPropertyName("actions")]
            public object[] Actions { get; set; }
        }

        /// <summary>
        /// Creates an adaptive card dynamically.
        /// </summary>
        /// <param name="title">Header title for the card</param>
        /// <param name="facts">Dictionary of key/value pairs to display in FactSet</param>
        /// <param name="buttons">List of buttons: (actionValue, buttonTitle)</param>
        /// <returns>JSON string of the adaptive card</returns>
        public static string CreateCard(
            string title,
            Dictionary<string, string> facts,
            List<(string action, string title, string requestId)> buttons,
            bool useCustomSchema = false)
        {
            var body = new List<object>
            {
                new { type = "TextBlock",  size = "Large", weight = "Bolder", text = title },
                new { type = "FactSet", facts = facts.Select(kvp => new { title = kvp.Key, value = kvp.Value }).ToArray() },
                new { type = "TextBlock", text = "Please confirm whether to approve this member's chip redemption / reward request.", wrap = "true", spacing = "Medium" },
            };

            // Actions array
            var actions = buttons.Select(b => new
            {
                type = "Action.Submit",
                title = b.title,
                data = new { action = b.action, requestId = b.requestId }
            }).ToArray();

            var card = new AdaptiveCard
            {
                  Schema = useCustomSchema 
                    ? "http://mma.io/adaptivecards/schemas/adaptive-card.json" 
                    : "http://adaptivecards.io/schemas/adaptive-card.json",
                Body = body.ToArray(),
                Actions = actions
            };

            return JsonSerializer.Serialize(card, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// Helper method for reward approval cards
        /// </summary>
        public static string CreateRewardApprovalCard(RewardRequest request, Dictionary<string,string> performanceData)
        {
            var title = request.RewardType;
            var facts = new Dictionary<string, string>(performanceData)
            {
                { "Amount", request.Amount.ToString("C") },
            };

            var buttons = new List<(string action, string title, string requestId)>
            {
                ("approve", "✅ Approve", request.RequestId),
                ("reject", "❌ Reject", request.RequestId)
            };

            return CreateCard(title, facts, buttons);
        }
    }
}
