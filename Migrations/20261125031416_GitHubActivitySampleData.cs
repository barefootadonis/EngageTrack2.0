using System;
using Microsoft.EntityFrameworkCore.Migrations;


namespace EngageTrack.Migrations
{
    public partial class GitHubActivitySampleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GitHubActivities",
                columns: new[] { "GitHubActivityId", "ActivityType", "Title", "UserId", "OccurredAt", "Url" },
                values: new object[,]
                {
                    { 4, "Commit", "Updated project homepage", 1001, new DateTime(2026, 11, 25, 0, 0, 0), "https://github.com" },
                    { 5, "Pull Request", "Added student dashboard", 1001, new DateTime(2026, 11, 24, 0, 0, 0), "https://github.com" },
                    { 6, "Commit", "Fixed navigation layout", 1001, new DateTime(2026, 11, 23, 0, 0, 0), "https://github.com" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GitHubActivities",
                keyColumn: "GitHubActivityId",
                keyValues: new object[] { 4, 5, 6 });
        }
    }
}
