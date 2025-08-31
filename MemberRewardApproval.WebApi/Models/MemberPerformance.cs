namespace MemberRewardApproval.WebApi.Models
{
    public class MemberPerformance
    {
        public string WynnId { get; set; }       // Primary key
        public decimal AvgBet { get; set; }
        public decimal WinLoss { get; set; }
        public decimal TheoWin { get; set; }
        public TimeSpan Playtime { get; set; }
        public decimal ADT { get; set; }
    }
}