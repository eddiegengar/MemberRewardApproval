namespace MemberRewardApproval.WebApi.Models;

public class DailySequence
{
    public string EntityName { get; set; } = default!;
    public DateTime Date { get; set; }
    public int LastSequence { get; set; }
}
