using Microsoft.Bot.Schema;
using System.Collections.Concurrent;

namespace MemberRewardApproval.WebApi.Services.Bots
{
    public static class ConversationReferenceStore
    {
        // Keyed by user ID (or email/AAD ID)
        private static readonly ConcurrentDictionary<string, ConversationReference> _store
            = new();

        public static void Save(string userId, ConversationReference reference)
        {
            _store[userId] = reference;
        }

        public static bool TryGet(string userId, out ConversationReference reference)
        {
            return _store.TryGetValue(userId, out reference);
        }
    }
}