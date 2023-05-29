using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class HoSoThanhToan_TaiKhoanNhan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaiKhoanThuHuongTaiKhoanId",
                table: "HoSoThanhToan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_TaiKhoanThuHuongTaiKhoanId",
                table: "HoSoThanhToan",
                column: "TaiKhoanThuHuongTaiKhoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_TaiKhoanNganHang_TaiKhoanThuHuongTaiKhoanId",
                table: "HoSoThanhToan",
                column: "TaiKhoanThuHuongTaiKhoanId",
                principalTable: "TaiKhoanNganHang",
                principalColumn: "TaiKhoanId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoThanhToan_TaiKhoanNganHang_TaiKhoanThuHuongTaiKhoanId",
                table: "HoSoThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_HoSoThanhToan_TaiKhoanThuHuongTaiKhoanId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "TaiKhoanThuHuongTaiKhoanId",
                table: "HoSoThanhToan");
        }
    }
}
