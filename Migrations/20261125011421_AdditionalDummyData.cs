using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngageTrack.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserRole" },
                values: new object[,]
                {
                    { 1005, "zara.adeyemi@gmail.com", "Zara Adeyemi", "Student" },
                    { 1006, "noah.clarke@gmail.com", "Noah Clarke", "Student" },
                    { 1007, "priya.shah@gmail.com", "Priya Shah", "Student" },
                    { 1008, "ellis.thompson@gmail.com", "Ellis Thompson", "Student" },
                    { 1009, "mina.hassan@gmail.com", "Mina Hassan", "Student" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1009);
        }
    }
}
