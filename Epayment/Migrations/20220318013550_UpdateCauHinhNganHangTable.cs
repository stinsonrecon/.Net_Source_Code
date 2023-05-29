using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class UpdateCauHinhNganHangTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "EVNRSAPath",
                table: "CauHinhNganHang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkQueryBaseUrl",
                table: "CauHinhNganHang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassPhraseRSA",
                table: "CauHinhNganHang",
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
                name: "EVNRSAPath",
                table: "CauHinhNganHang");

            migrationBuilder.DropColumn(
                name: "LinkQueryBaseUrl",
                table: "CauHinhNganHang");

            migrationBuilder.DropColumn(
                name: "PassPhraseRSA",
                table: "CauHinhNganHang");

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
