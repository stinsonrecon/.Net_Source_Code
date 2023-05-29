using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class HoSoThanhToanaddNguoiPheDuyetHoSo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "NguoiPheDuyetHoSoId",
                table: "HoSoThanhToan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_NguoiPheDuyetHoSoId",
                table: "HoSoThanhToan",
                column: "NguoiPheDuyetHoSoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_AspNetUsers_NguoiPheDuyetHoSoId",
                table: "HoSoThanhToan",
                column: "NguoiPheDuyetHoSoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_HoSoThanhToan_AspNetUsers_NguoiPheDuyetHoSoId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_HoSoThanhToan_NguoiPheDuyetHoSoId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "NguoiPheDuyetHoSoId",
                table: "HoSoThanhToan");

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
