namespace MemberRewardApproval.WebApi.Models
{
    public class RewardRequestDto
    {
        public string WynnId { get; set; } = string.Empty;
        public string RewardType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
