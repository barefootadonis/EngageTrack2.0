using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngageTrack.Migrations
{
    /// <inheritdoc />
    public partial class DummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserRole" },
                values: new object[,]
                {
                    { 1001, "amara.okafor@gmail.com", "Amara Okafor", "Student" },
                    { 1002, "jamie.patel@gmail.com", "Jamie Patel", "Student" },
                    { 1003, "leila.morgan@gmail.com", "Leila Morgan", "Student" },
                    { 1004, "theo.williams@gmail.com", "Theo Williams", "Student" },
                    { 2001, "hannah.reed@gmail.com", "Hannah Reed", "Supervisor" },
                    { 2002, "samir.ahmed@gmail.com", "Samir Ahmed", "Supervisor" },
                    { 3001, "maya.chen@gmail.com", "Maya Chen", "ModuleLeader" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3001);
        }
    }
}
