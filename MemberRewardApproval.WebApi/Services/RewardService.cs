using MemberRewardApproval.WebApi.Data;
using MemberRewardApproval.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MemberRewardApproval.WebApi.Services
{
    public class RewardService
    {
        private readonly RewardsDbContext _db;
        private readonly SequenceService _sequenceService;

        public RewardService(RewardsDbContext db, SequenceService sequenceService)
        {
            _db = db;
            _sequenceService = sequenceService;
        }

        public async Task<List<RewardRequest>> GetAllRequestsAsync()
        {
            return await _db.RewardRequests
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new reward request for the specified WynnID and reward details.
        /// Ensures there is no existing pending request for the same WynnID.
        /// Adds the request to the database with status "Pending" and returns it.
        /// </summary>
        public async Task<RewardRequest> CreateRequestAsync(string wynnId, string rewardType, RequestedValue requestedValue)
        {
            // Check pending requests
            var pending = await _db.RewardRequests
                .AnyAsync(r => r.WynnId == wynnId && r.Status == RewardStatus.Pending);
            if (pending) throw new InvalidOperationException($"WynnID {wynnId} already has a pending request.");

            // Create reward request
            string requestId = await _sequenceService.GenerateEntityIdAsync("Request", "REQ");
            var request = new RewardRequest
            {
                RequestId = requestId,
                WynnId = wynnId,
                RewardType = rewardType,
                RequestedValue = new RequestedValue
                {
                    Title = requestedValue.Title,
                    Amount = requestedValue.Amount
                },
                Status = RewardStatus.Pending
            };
            _db.RewardRequests.Add(request);
            await _db.SaveChangesAsync();

            return request;
        }

        /// <summary>
        /// Retrieves the performance metrics for a member identified by the given WynnID.
        /// </summary>
        public async Task<Dictionary<string, string>> GetMemberPerformanceAsync(string wynnId)
        {
            var latestSnapshot = await _db.MemberPerformanceSnapshots
                        .Where(s => s.WynnId == wynnId)
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefaultAsync();
            if (latestSnapshot == null)
                throw new InvalidOperationException($"Performance data for WynnID {wynnId} not found.");

            return new Dictionary<string, string>
            {
                { "WynnID", latestSnapshot.WynnId},
                { "Avg Bet", latestSnapshot.AvgBet.ToString("C", CultureInfo.CreateSpecificCulture("en-US")) },
                { "Win/Loss", latestSnapshot.WinLoss.ToString("C", CultureInfo.CreateSpecificCulture("en-US")) },
                { "Theo Win", latestSnapshot.TheoWin.ToString("C", CultureInfo.CreateSpecificCulture("en-US")) },
                { "Playtime", latestSnapshot.Playtime.TotalHours + "h" },
                { "ADT", latestSnapshot.ADT.ToString("C", CultureInfo.CreateSpecificCulture("en-US")) }
            };
        }

        /// <summary>
        /// Update request status after supervisor action
        /// </summary>
        public async Task<RewardRequest> UpdateRequestStatusAsync(string requestId, string status)
        {
            var request = await _db.RewardRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = status;
                await _db.SaveChangesAsync();
            }
            return request;
        }
    }
}

