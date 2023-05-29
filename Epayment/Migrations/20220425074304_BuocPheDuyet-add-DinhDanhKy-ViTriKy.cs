using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BuocPheDuyetaddDinhDanhKyViTriKy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "DinhDangKy",
                table: "BuocPheDuyet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViTriKy",
                table: "BuocPheDuyet",
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
                name: "DinhDangKy",
                table: "BuocPheDuyet");

            migrationBuilder.DropColumn(
                name: "ViTriKy",
                table: "BuocPheDuyet");

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
