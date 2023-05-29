using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class ChungTuaddTaiLieuGocCTGSTaiLieuKyCTGS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "TaiLieuGocCTGS",
                table: "ChungTu",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaiLieuKyCTGS",
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
                name: "TaiLieuGocCTGS",
                table: "ChungTu");

            migrationBuilder.DropColumn(
                name: "TaiLieuKyCTGS",
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
