using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngageTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupMemberCountAndChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentMembers",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "GroupChangeRequest",
                table: "Groups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxMembers",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentMembers",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupChangeRequest",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MaxMembers",
                table: "Groups");
        }
    }
}
