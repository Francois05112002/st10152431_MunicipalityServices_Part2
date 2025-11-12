using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace st10152431_MunicipalityService.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmployeeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Issues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReviewedDate",
                table: "Issues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Issues",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DueDate", "LastReviewedDate", "Priority", "ReviewedBy", "Status" },
                values: new object[] { null, null, null, null, "Pending" });

            migrationBuilder.UpdateData(
                table: "Issues",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DueDate", "LastReviewedDate", "Priority", "ReviewedBy", "Status" },
                values: new object[] { null, null, null, null, "Pending" });

            migrationBuilder.UpdateData(
                table: "Issues",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DueDate", "LastReviewedDate", "Priority", "ReviewedBy", "Status" },
                values: new object[] { null, null, null, null, "Pending" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "LastReviewedDate",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Issues");
        }
    }
}
