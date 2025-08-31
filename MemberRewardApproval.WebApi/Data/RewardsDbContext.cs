using MemberRewardApproval.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberRewardApproval.WebApi.Data
{
    public class RewardsDbContext : DbContext
    {
        public RewardsDbContext(DbContextOptions<RewardsDbContext> options) : base(options) { }

        public DbSet<RewardRequest> RewardRequests { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<MemberPerformance> MemberPerformances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RewardRequest>()
                .HasKey(m => m.RequestId); 

            modelBuilder.Entity<MemberPerformance>()
                .HasKey(m => m.WynnId); 

            modelBuilder.Entity<Supervisor>()
                .HasKey(m => m.Id); //

            modelBuilder.Entity<Supervisor>().HasData(
                new Supervisor { Id = "1", Name = "Supervisor1", Email = "eddiegengar@gmail.com", AadId = "6f6a353c0843453e" },
                new Supervisor { Id = "2", Name = "Supervisor2", Email = "supervisor2@example.com", AadId = "AAD-ID-2" }
            );

            modelBuilder.Entity<MemberPerformance>().HasData(
                new MemberPerformance { WynnId = "W001", AvgBet = 100, WinLoss = 50, TheoWin = 120, Playtime = TimeSpan.FromHours(5), ADT = 30 },
                new MemberPerformance { WynnId = "W002", AvgBet = 200, WinLoss = 75, TheoWin = 180, Playtime = TimeSpan.FromHours(8), ADT = 50 }
            );
        }
    }

}