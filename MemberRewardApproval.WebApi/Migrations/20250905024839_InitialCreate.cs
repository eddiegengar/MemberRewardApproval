using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemberRewardApproval.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversationReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AadObjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BotId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BotName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationReferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailySequences",
                columns: table => new
                {
                    EntityName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySequences", x => new { x.EntityName, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "MemberPerformanceSnapshots",
                columns: table => new
                {
                    SnapshotId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WynnId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvgBet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WinLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TheoWin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Playtime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ADT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPerformanceSnapshots", x => x.SnapshotId);
                });

            migrationBuilder.CreateTable(
                name: "RewardRequests",
                columns: table => new
                {
                    RequestId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WynnId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RewardType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedValue_Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedValue_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardRequests", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MemberPerformanceSnapshots",
                columns: new[] { "SnapshotId", "ADT", "AvgBet", "CreatedAt", "Playtime", "TheoWin", "WinLoss", "WynnId" },
                values: new object[] { "11111111-1111-1111-1111-111111111111", 3000m, 5000m, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 0, 0, 0), 18000m, -20000m, "12345678" });

            migrationBuilder.InsertData(
                table: "Supervisors",
                columns: new[] { "Id", "AadId", "Email", "Name" },
                values: new object[] { "1", "6f6a353c0843453e", "it@winson-group.com", "Supervisor1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationReferences");

            migrationBuilder.DropTable(
                name: "DailySequences");

            migrationBuilder.DropTable(
                name: "MemberPerformanceSnapshots");

            migrationBuilder.DropTable(
                name: "RewardRequests");

            migrationBuilder.DropTable(
                name: "Supervisors");
        }
    }
}
