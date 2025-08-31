using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemberRewardApproval.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixRewardRequestKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RewardRequests",
                table: "RewardRequests");

            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "RewardRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WynnId",
                table: "RewardRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RewardRequests",
                table: "RewardRequests",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RewardRequests",
                table: "RewardRequests");

            migrationBuilder.AlterColumn<string>(
                name: "WynnId",
                table: "RewardRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "RewardRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RewardRequests",
                table: "RewardRequests",
                column: "WynnId");
        }
    }
}
