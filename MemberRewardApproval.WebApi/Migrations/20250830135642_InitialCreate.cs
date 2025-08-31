using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MemberRewardApproval.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberPerformances",
                columns: table => new
                {
                    WynnId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AvgBet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WinLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TheoWin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Playtime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ADT = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPerformances", x => x.WynnId);
                });

            migrationBuilder.CreateTable(
                name: "RewardRequests",
                columns: table => new
                {
                    WynnId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RewardType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardRequests", x => x.WynnId);
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
                table: "MemberPerformances",
                columns: new[] { "WynnId", "ADT", "AvgBet", "Playtime", "TheoWin", "WinLoss" },
                values: new object[,]
                {
                    { "W001", 30m, 100m, new TimeSpan(0, 5, 0, 0, 0), 120m, 50m },
                    { "W002", 50m, 200m, new TimeSpan(0, 8, 0, 0, 0), 180m, 75m }
                });

            migrationBuilder.InsertData(
                table: "Supervisors",
                columns: new[] { "Id", "AadId", "Email", "Name" },
                values: new object[,]
                {
                    { "1", "6f6a353c0843453e", "eddiegengar@gmail.com", "Supervisor1" },
                    { "2", "AAD-ID-2", "supervisor2@example.com", "Supervisor2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberPerformances");

            migrationBuilder.DropTable(
                name: "RewardRequests");

            migrationBuilder.DropTable(
                name: "Supervisors");
        }
    }
}
