namespace MemberRewardApproval.WebApi.Models
{
    public class CardActionPayload
    {
        public ActionData Data { get; set; }
        public string SupervisorId { get; set; }
    }

    public class ActionData
    {
        public string Action { get; set; }
        public string RequestId { get; set; }
    }
}