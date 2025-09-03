namespace MemberRewardApproval.WebApi.Models
{
    public class CardActionPayload
    {
        public ActionData Data { get; set; }
        public string SupervisorId { get; set; }
    }

    public class ActionData
    {
        public string Action { get; set; }        // "approve" or "reject"
        public string RequestId { get; set; }     // the request being approved/rejected
        public string WynnId { get; set; }        // the member id associated with the request
    }
}
