using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace st10152431_MunicipalityService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStatsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyPulsesCompleted",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IssuesReported",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalDataPoints",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "CellphoneNumber",
                keyValue: "0123456789",
                columns: new[] { "DailyPulsesCompleted", "IssuesReported", "TotalDataPoints" },
                values: new object[] { 0, 0, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "CellphoneNumber",
                keyValue: "0817246624",
                columns: new[] { "DailyPulsesCompleted", "IssuesReported", "TotalDataPoints" },
                values: new object[] { 0, 0, 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "CellphoneNumber",
                keyValue: "0987654321",
                columns: new[] { "DailyPulsesCompleted", "IssuesReported", "TotalDataPoints" },
                values: new object[] { 0, 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyPulsesCompleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IssuesReported",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalDataPoints",
                table: "Users");
        }
    }
}
