using MemberRewardApproval.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberRewardApproval.WebApi.Data
{
    public class RewardsDbContext : DbContext
    {
        public RewardsDbContext(DbContextOptions<RewardsDbContext> options) : base(options) { }

        public DbSet<RewardRequest> RewardRequests { get; set; }
        public DbSet<DailySequence> DailySequences { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        // public DbSet<MemberPerformance> MemberPerformances { get; set; }
        public DbSet<MemberPerformanceSnapshot> MemberPerformanceSnapshots { get; set; }
        public DbSet<ConversationReferenceEntity> ConversationReferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RewardRequest>()
                .HasKey(m => m.RequestId);

            modelBuilder.Entity<Supervisor>()
               .HasKey(m => m.Id);

            modelBuilder.Entity<DailySequence>()
              .HasKey(e => new { e.EntityName, e.Date });

            modelBuilder.Entity<MemberPerformanceSnapshot>()
                .HasKey(m => m.SnapshotId);

            modelBuilder.Entity<Supervisor>().HasData(
                new Supervisor { Id = "1", Name = "Supervisor1", Email = "it@winson-group.com", AadId = "6f6a353c0843453e" }
            );

            modelBuilder.Entity<MemberPerformanceSnapshot>().HasData(
                new MemberPerformanceSnapshot
                {
                    SnapshotId = new Guid("11111111-1111-1111-1111-111111111111").ToString(),
                    WynnId = "12345678",
                    AvgBet = 5000,
                    WinLoss = -20000,
                    TheoWin = 18000,
                    Playtime = TimeSpan.FromHours(12),
                    ADT = 3000,
                    CreatedAt = new DateTime(2025, 9, 1)
                }
            );
        }
    }

}