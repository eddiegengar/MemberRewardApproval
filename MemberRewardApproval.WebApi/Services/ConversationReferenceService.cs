using MemberRewardApproval.WebApi.Data;
using MemberRewardApproval.WebApi.Mappers;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;

namespace MemberRewardApproval.WebApi.Services;

public class ConversationReferenceService
{
    private readonly RewardsDbContext _db;
    public ConversationReferenceService(RewardsDbContext db)
    {
        _db = db;
    }

    public async Task<ConversationReference?> SaveOrUpdateConversationReferenceAsync(Activity activity)
    {
        var reference = activity.GetConversationReference();
        var entity = ConversationReferenceMapper.ToEntity(reference);

        // Capture AadObjectId for business lookup
        entity.AadObjectId = activity.From?.AadObjectId ?? string.Empty;

        var existing = await _db.ConversationReferences
            .FirstOrDefaultAsync(x => x.AadObjectId == entity.AadObjectId
                                   && x.ConversationType == entity.ConversationType);

        if (existing == null)
            _db.ConversationReferences.Add(entity);
        else
        {
            // Refresh values
            existing.UserId = entity.UserId;
            existing.UserName = entity.UserName;
            existing.ConversationId = entity.ConversationId;
            existing.ServiceUrl = entity.ServiceUrl;
            existing.ChannelId = entity.ChannelId;
            existing.BotId = entity.BotId;
            existing.BotName = entity.BotName;
        }

        await _db.SaveChangesAsync();

        // Return the conversation reference mapped back from the entity
        var savedEntity = existing ?? entity;
        return ConversationReferenceMapper.ToConversationReference(savedEntity);
    }

    public async Task<ConversationReference?> GetConversationReferenceAsync(string aadId, string conversationType = "personal")
    {
        var entity = await _db.ConversationReferences
            .FirstOrDefaultAsync(x => x.AadObjectId == aadId && x.ConversationType == conversationType);

        return entity != null ? ConversationReferenceMapper.ToConversationReference(entity) : null;
    }
}
