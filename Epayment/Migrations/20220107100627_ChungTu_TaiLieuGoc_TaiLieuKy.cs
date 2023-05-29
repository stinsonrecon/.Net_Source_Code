using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class ChungTu_TaiLieuGoc_TaiLieuKy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "TaiLieuGoc",
                table: "KySoChungTu",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaiLieuKy",
                table: "KySoChungTu",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaiLieuGoc",
                table: "ChungTu",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaiLieuKy",
                table: "ChungTu",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "TaiLieuGoc",
                table: "KySoChungTu");

            migrationBuilder.DropColumn(
                name: "TaiLieuKy",
                table: "KySoChungTu");

            migrationBuilder.DropColumn(
                name: "TaiLieuGoc",
                table: "ChungTu");

            migrationBuilder.DropColumn(
                name: "TaiLieuKy",
                table: "ChungTu");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
