using MemberRewardApproval.WebApi.Data;
using MemberRewardApproval.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberRewardApproval.WebApi.Services
{
    public class RewardService
    {
        private readonly RewardsDbContext _db;

        public RewardService(RewardsDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new reward request for the specified WynnID and reward details.
        /// Ensures there is no existing pending request for the same WynnID.
        /// Adds the request to the database with status "Pending" and returns it.
        /// </summary>
        public async Task<RewardRequest> CreateRequestAsync(string wynnId, string rewardType, decimal amount)
        {
            if (await _db.RewardRequests.AnyAsync(r => r.WynnId == wynnId && r.Status == "Pending"))
                throw new InvalidOperationException($"WynnID {wynnId} already has a pending request.");

            var request = new RewardRequest
            {
                WynnId = wynnId,
                RewardType = rewardType,
                Amount = amount,
                Status = "Pending"
            };

            _db.RewardRequests.Add(request);
            await _db.SaveChangesAsync();

            return request;
        }

        /// <summary>
        /// Returns the supervisor's Azure AD ID for the given email.
        /// </summary>
        public async Task<string> GetSupervisorAadIdAsync(string email)
        {
            var supervisor = await _db.Supervisors.FirstOrDefaultAsync(s => s.Email == email);
            return supervisor?.AadId ?? throw new InvalidOperationException($"Supervisor for email {email} not found.");
        }

        /// <summary>
        /// Retrieves the performance metrics for a member identified by the given WynnID.
        /// </summary>
        public async Task<Dictionary<string, string>> GetMemberPerformanceAsync(string wynnId)
        {
            var performance = await _db.MemberPerformances.FindAsync(wynnId);
            if (performance == null)
                throw new InvalidOperationException($"Performance data for WynnID {wynnId} not found.");

            return new Dictionary<string, string>
            {
                { "WynnID", performance.WynnId},
                { "Avg Bet", performance.AvgBet.ToString("C") },
                { "Win/Loss", performance.WinLoss.ToString("C") },
                { "Theo Win", performance.TheoWin.ToString("C") },
                { "Playtime", performance.Playtime.TotalHours + "h" },
                { "ADT", performance.ADT.ToString("C") }
            };
        }

        /// <summary>
        /// Update request status after supervisor action
        /// </summary>
        public async Task UpdateRequestStatusAsync(string requestId, string status)
        {
            var request = await _db.RewardRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = status;
                await _db.SaveChangesAsync();
            }
        }
    }
}
