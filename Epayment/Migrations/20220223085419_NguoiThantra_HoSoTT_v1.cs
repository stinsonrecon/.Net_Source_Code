using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class NguoiThantra_HoSoTT_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "NguoiThamTraId",
                table: "HoSoThanhToan",
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
                name: "FK_HoSoThanhToan_AspNetUsers_NguoiThamTraId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoThanhToan_QuaTrinhPheDuyet_QuaTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropTable(
                name: "CauHinhNganHang");

            migrationBuilder.DropTable(
                name: "QuaTrinhPheDuyet");

            migrationBuilder.DropTable(
                name: "ThaoTacBuocPheDuyet");

            migrationBuilder.DropTable(
                name: "BuocPheDuyet");

            migrationBuilder.DropTable(
                name: "QuyTrinhPheDuyet");

            migrationBuilder.DropIndex(
                name: "IX_HoSoThanhToan_NguoiThamTraId",
                table: "HoSoThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_HoSoThanhToan_QuaTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "LaTaiKhoanNhan",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "NguoiThamTraId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "QuaTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "QuyTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "LoaiTaiKhoan",
                table: "ChiNhanhNganHang");

            migrationBuilder.AlterColumn<Guid>(
                name: "HoSoLienQuanId",
                table: "HoSoThamChieu",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

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
