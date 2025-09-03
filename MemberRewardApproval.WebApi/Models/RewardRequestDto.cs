using Microsoft.EntityFrameworkCore;

namespace MemberRewardApproval.WebApi.Models
{
    public class RewardRequestDto
    {
        public string WynnId { get; set; } = string.Empty;
        public string RewardType { get; set; } = string.Empty;
        public RequestedValue RequestedValue { get; set; } = new RequestedValue();
    }

    [Owned]
    public class RequestedValue
    {
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
