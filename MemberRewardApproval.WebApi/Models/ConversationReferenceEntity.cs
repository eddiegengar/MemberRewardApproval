namespace MemberRewardApproval.WebApi.Models;

public class ConversationReferenceEntity
{
    public int Id { get; set; }

    // Business key
    public string? AadObjectId { get; set; }

    // User-specific
    public string UserId { get; set; } = default!; // Teams user ID
    public string? UserName { get; set; }

    // Conversation info
    public string ConversationId { get; set; } = default!;
    public string? ConversationName { get; set; }
    public string ConversationType { get; set; } = default!; // personal, channel, groupChat

    // Bot info
    public string BotId { get; set; } = default!;
    public string BotName { get; set; } = default!;

    // Channel + service URL
    public string ChannelId { get; set; } = default!;
    public string ServiceUrl { get; set; } = default!;
}
