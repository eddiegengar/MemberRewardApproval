using MemberRewardApproval.WebApi.Data;
using MemberRewardApproval.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberRewardApproval.WebApi.Services
{
    public class SequenceService
    {
        private readonly RewardsDbContext _db;

        public SequenceService(RewardsDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateEntityIdAsync(string entityName, string prefix)
        {
            var today = DateTime.UtcNow.Date;

            using var transaction = await _db.Database.BeginTransactionAsync();

            var sequence = await _db.Set<DailySequence>()
                .FirstOrDefaultAsync(s => s.EntityName == entityName && s.Date == today);

            if (sequence == null)
            {
                sequence = new DailySequence
                {
                    EntityName = entityName,
                    Date = today,
                    LastSequence = 1
                };
                _db.Set<DailySequence>().Add(sequence);
            }
            else
            {
                sequence.LastSequence += 1;
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return $"{prefix}-{today:yyyyMMdd}-{sequence.LastSequence:D3}";
        }
    }
}
