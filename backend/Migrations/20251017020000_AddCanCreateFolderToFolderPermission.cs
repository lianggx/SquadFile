using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SquadFile.Migrations
{
    /// <inheritdoc />
    public partial class AddCanCreateFolderToFolderPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanCreateFolder",
                table: "folder_permission",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCreateFolder",
                table: "folder_permission");
        }
    }
}