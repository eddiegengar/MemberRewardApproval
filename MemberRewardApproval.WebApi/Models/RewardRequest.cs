namespace MemberRewardApproval.WebApi.Models
{
    public class RewardRequest
    {
        public string RequestId { get; set; }
        public string WynnId { get; set; } = default!;
        public string RewardType { get; set; } = default!;
        public RequestedValue RequestedValue { get; set; } = new RequestedValue();
        public string Status { get; set; } = RewardStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public static class RewardStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}
