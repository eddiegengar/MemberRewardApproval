using MemberRewardApproval.WebApi.Models;

namespace MemberRewardApproval.WebApi.Services
{
    public interface INotificationService
    {
        Task SendApprovalCardAsync(string supervisorId, RewardRequest request, Dictionary<string, string> performanceData);
    }
}
