using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class Epayment_VanPhongPhamV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "TaiKhoanNganHang",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TaiKhoanNganHang",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuocGiaId",
                table: "TaiKhoanNganHang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sdt",
                table: "TaiKhoanNganHang",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TinhTpId",
                table: "TaiKhoanNganHang",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuocGia",
                columns: table => new
                {
                    QuocGiaId = table.Column<Guid>(nullable: false),
                    MaQuocGia = table.Column<string>(nullable: true),
                    TenQuocGia = table.Column<string>(nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    DaXoa = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuocGia", x => x.QuocGiaId);
                });

            migrationBuilder.CreateTable(
                name: "TinhThanhPho",
                columns: table => new
                {
                    TinhTpId = table.Column<Guid>(nullable: false),
                    QuocGiaId = table.Column<Guid>(nullable: true),
                    MaTinhTp = table.Column<string>(nullable: true),
                    TenTinhTp = table.Column<string>(nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    DaXoa = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhThanhPho", x => x.TinhTpId);
                    table.ForeignKey(
                        name: "FK_TinhThanhPho_QuocGia_QuocGiaId",
                        column: x => x.QuocGiaId,
                        principalTable: "QuocGia",
                        principalColumn: "QuocGiaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNganHang_QuocGiaId",
                table: "TaiKhoanNganHang",
                column: "QuocGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNganHang_TinhTpId",
                table: "TaiKhoanNganHang",
                column: "TinhTpId");

            migrationBuilder.CreateIndex(
                name: "IX_TinhThanhPho_QuocGiaId",
                table: "TinhThanhPho",
                column: "QuocGiaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNganHang_QuocGia_QuocGiaId",
                table: "TaiKhoanNganHang",
                column: "QuocGiaId",
                principalTable: "QuocGia",
                principalColumn: "QuocGiaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNganHang_TinhThanhPho_TinhTpId",
                table: "TaiKhoanNganHang",
                column: "TinhTpId",
                principalTable: "TinhThanhPho",
                principalColumn: "TinhTpId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNganHang_QuocGia_QuocGiaId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNganHang_TinhThanhPho_TinhTpId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropTable(
                name: "TinhThanhPho");

            migrationBuilder.DropTable(
                name: "QuocGia");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoanNganHang_QuocGiaId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoanNganHang_TinhTpId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "QuocGiaId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "Sdt",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "TinhTpId",
                table: "TaiKhoanNganHang");

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
