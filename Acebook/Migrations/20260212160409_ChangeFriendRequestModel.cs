using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace acebook.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFriendRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending",
                table: "FriendRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pending",
                table: "FriendRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
