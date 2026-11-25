using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EngageTrack.Migrations
{
    /// <inheritdoc />
    public partial class SeedSupervisorGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "GroupId", "CurrentMembers", "DateCreated", "GroupChangeRequest", "GroupIsActive", "GroupName", "MaxMembers", "ProjectName", "ProjectSummary", "SupervisorId" },
                values: new object[,]
                {
                    { 4001, 3, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Green Energy Team", 5, "Campus Solar Tracker", "Designing a dashboard to monitor campus solar energy production.", 2001 },
                    { 4002, 3, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Access for All", 5, "Accessible Campus Map", "Creating an accessible map of campus facilities and routes.", 2002 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1001,
                column: "UserEmail",
                value: "amara.okafor.student@mail.shu.ac.uk");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1002,
                column: "UserEmail",
                value: "jamie.patel.student@mail.shu.ac.uk");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1003,
                column: "UserEmail",
                value: "leila.morgan.student@mail.shu.ac.uk");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1004,
                column: "UserEmail",
                value: "theo.williams.student@mail.shu.ac.uk");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserRole" },
                values: new object[,]
                {
                    { 1005, "aisha.khan.student@mail.shu.ac.uk", "Aisha Khan", "Student" },
                    { 1006, "oliver.james.student@mail.shu.ac.uk", "Oliver James", "Student" }
                });

            migrationBuilder.InsertData(
                table: "GroupMemberships",
                columns: new[] { "GroupId", "StudentId", "StudentJob" },
                values: new object[,]
                {
                    { 4001, 1001, null },
                    { 4001, 1002, null },
                    { 4001, 1003, null },
                    { 4002, 1004, null },
                    { 4002, 1005, null },
                    { 4002, 1006, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GroupMemberships",
                keyColumns: new[] { "GroupId", "StudentId" },
                keyValues: new object[] { 4001, 1001 });

            migrationBuilder.DeleteData(
                table: "GroupMemberships",
                keyColumns: new[] { "GroupId", "StudentId" },
                keyValues: new object[] { 4001, 1002 });

            migrationBuilder.DeleteData(
                table: "GroupMemberships",
                keyColumns: new[] { "GroupId", "StudentId" },
                keyValues: new object[] { 4001, 1003 });

            migrationBuilder.DeleteData(
                table: "GroupMemberships",
                keyColumns: new[] { "GroupId", "StudentId" },
                keyValues: new object[] { 4002, 1004 });

            migrationBuilder.DeleteData(
                table: "GroupMemberships",
                keyColumns: new[] { "GroupId", "StudentId" },
                keyValues: new object[] { 4002, 1005 });

            migrationBuilder.DeleteData(
                table: "GroupMemberships",
                keyColumns: new[] { "GroupId", "StudentId" },
                keyValues: new object[] { 4002, 1006 });

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "GroupId",
                keyValue: 4001);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "GroupId",
                keyValue: 4002);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1006);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1001,
                column: "UserEmail",
                value: "amara.okafor@gmail.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1002,
                column: "UserEmail",
                value: "jamie.patel@gmail.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1003,
                column: "UserEmail",
                value: "leila.morgan@gmail.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1004,
                column: "UserEmail",
                value: "theo.williams@gmail.com");
        }
    }
}
