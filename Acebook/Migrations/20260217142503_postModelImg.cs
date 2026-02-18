using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace acebook.Migrations
{
    /// <inheritdoc />
    public partial class postModelImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgLink",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImgLink",
                table: "Posts");
        }
    }
}
