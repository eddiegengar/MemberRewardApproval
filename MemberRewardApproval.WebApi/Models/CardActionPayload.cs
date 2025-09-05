namespace MemberRewardApproval.WebApi.Models
{
    public class CardActionPayload
    {
        public string Action { get; set; }
        public string RequestId { get; set; }

    }

    public class ActionData
    {
        public string Action { get; set; }        // "approve" or "reject"
        public string RequestId { get; set; }     // the request being approved/rejected
    }
}
