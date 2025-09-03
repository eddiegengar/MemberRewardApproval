namespace MemberRewardApproval.WebApi.Models;

public class MemberPerformanceSnapshot
{
    public string SnapshotId { get; set; } = Guid.NewGuid().ToString();
    public string WynnId { get; set; } = default!;
    public decimal AvgBet { get; set; }
    public decimal WinLoss { get; set; }
    public decimal TheoWin { get; set; }
    public TimeSpan Playtime { get; set; }
    public decimal ADT { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
