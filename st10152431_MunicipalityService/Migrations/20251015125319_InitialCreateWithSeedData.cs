using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace st10152431_MunicipalityService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Issues",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Announcements",
                columns: new[] { "Id", "Category", "CreatedBy", "EndDate", "Location", "Name", "StartDate" },
                values: new object[,]
                {
                    { 1, "Maintenance", "0817246624", new DateTime(2025, 10, 20, 23, 59, 59, 0, DateTimeKind.Unspecified), "Northern Suburbs", "Water Supply Maintenance", new DateTime(2025, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "General", "0987654321", new DateTime(2025, 11, 15, 23, 59, 59, 0, DateTimeKind.Unspecified), "All areas", "New Recycling Schedule", new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Category", "CreatedBy", "EndDate", "Location", "Name", "StartDate" },
                values: new object[,]
                {
                    { 1, "Community", "0817246624", new DateTime(2025, 10, 22, 17, 0, 0, 0, DateTimeKind.Unspecified), "Central Park, Cape Town", "Community Clean-Up Day", new DateTime(2025, 10, 22, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Sports", "0123456789", new DateTime(2025, 10, 31, 18, 0, 0, 0, DateTimeKind.Unspecified), "Sports Stadium, Cape Town", "Local Football Tournament", new DateTime(2025, 10, 29, 8, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Issues",
                columns: new[] { "Id", "Category", "Description", "ImagePath", "Location", "Timestamp", "UserId" },
                values: new object[] { 3, "Electricity", "Streetlight not working for 2 weeks", null, "78 Park Avenue, Gardens", new DateTime(2025, 10, 14, 9, 15, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.InsertData(
                table: "PulseResponses",
                columns: new[] { "Id", "Answer", "CreatedAt", "Date", "UserId" },
                values: new object[] { 1, "Satisfied", new DateTime(2025, 10, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), "2025-10-15", "0123456789" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "CellphoneNumber", "Name", "PulseDates" },
                values: new object[,]
                {
                    { "0123456789", "John Doe", "" },
                    { "0817246624", "Test User", "" },
                    { "0987654321", "Jane Smith", "" }
                });

            migrationBuilder.InsertData(
                table: "Issues",
                columns: new[] { "Id", "Category", "Description", "ImagePath", "Location", "Timestamp", "UserId" },
                values: new object[,]
                {
                    { 1, "Road", "Large pothole causing traffic issues", null, "123 Main Street, Cape Town", new DateTime(2025, 10, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), "0817246624" },
                    { 2, "Water", "Water pipe burst on sidewalk", null, "45 Beach Road, Sea Point", new DateTime(2025, 10, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), "0123456789" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Announcements",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Issues",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Issues",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Issues",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PulseResponses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "CellphoneNumber",
                keyValue: "0987654321");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "CellphoneNumber",
                keyValue: "0123456789");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "CellphoneNumber",
                keyValue: "0817246624");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Issues",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
