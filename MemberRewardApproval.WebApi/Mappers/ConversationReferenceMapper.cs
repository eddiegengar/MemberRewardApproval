using MemberRewardApproval.WebApi.Models;
using Microsoft.Bot.Schema;

namespace MemberRewardApproval.WebApi.Mappers;

public static class ConversationReferenceMapper
{
    public static ConversationReferenceEntity ToEntity(ConversationReference reference)
    {
        return new ConversationReferenceEntity
        {
            UserId = reference.User.Id,
            UserName = reference.User.Name,
            ConversationId = reference.Conversation.Id,
            ConversationName = reference.Conversation.Name,
            ConversationType = reference.Conversation.ConversationType,
            BotId = reference.Bot.Id,
            BotName = reference.Bot.Name,
            ChannelId = reference.ChannelId,
            ServiceUrl = reference.ServiceUrl
        };
    }

    public static ConversationReference ToConversationReference(ConversationReferenceEntity entity)
    {
        return new ConversationReference
        {
            User = new ChannelAccount(entity.UserId, entity.UserName),
            Conversation = new ConversationAccount(
                id: entity.ConversationId,
                name: entity.ConversationName,
                conversationType: entity.ConversationType),
            Bot = new ChannelAccount(entity.BotId, entity.BotName),
            ChannelId = entity.ChannelId,
            ServiceUrl = entity.ServiceUrl
        };
    }
}
