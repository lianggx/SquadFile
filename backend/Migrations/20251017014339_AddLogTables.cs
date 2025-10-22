using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SquadFile.Migrations
{
    /// <inheritdoc />
    public partial class AddLogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanWrite",
                table: "folder_permission");

            // 移除了重复的 AddColumn 操作，因为 CreatedBy 列已经存在

            migrationBuilder.CreateTable(
                name: "download_log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DownloadTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FileId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_download_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "login_log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    LoginTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LoginResult = table.Column<bool>(type: "INTEGER", nullable: false),
                    FailureReason = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_log", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedTime", "UpdatedTime" },
                values: new object[] { new DateTime(2025, 10, 17, 9, 43, 13, 2, DateTimeKind.Local).AddTicks(5674), new DateTime(2025, 10, 17, 9, 43, 13, 2, DateTimeKind.Local).AddTicks(5861) });

            migrationBuilder.CreateIndex(
                name: "IX_download_log_DownloadTime",
                table: "download_log",
                column: "DownloadTime");

            migrationBuilder.CreateIndex(
                name: "IX_download_log_FileId",
                table: "download_log",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_download_log_UserId",
                table: "download_log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_login_log_LoginTime",
                table: "login_log",
                column: "LoginTime");

            migrationBuilder.CreateIndex(
                name: "IX_login_log_UserId",
                table: "login_log",
                column: "UserId");

            // 移除了重复的 CreateIndex 和 AddForeignKey 操作，因为 CreatedBy 的索引和外键已经存在
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 移除了重复的 DropForeignKey 和 DropIndex 操作

            migrationBuilder.DropTable(
                name: "download_log");

            migrationBuilder.DropTable(
                name: "login_log");

            // 移除了重复的 DropColumn 操作

            migrationBuilder.AddColumn<bool>(
                name: "CanWrite",
                table: "folder_permission",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedTime", "UpdatedTime" },
                values: new object[] { new DateTime(2025, 9, 29, 4, 0, 0, 0, DateTimeKind.Utc).AddTicks(1), new DateTime(2025, 9, 29, 4, 0, 0, 0, DateTimeKind.Utc).AddTicks(2) });
        }
    }
}