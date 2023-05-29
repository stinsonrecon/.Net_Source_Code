using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class TaiKhoanNganHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaiKhoanNganHang",
                columns: table => new
                {
                    TaiKhoanId = table.Column<Guid>(nullable: false),
                    TenTaiKhoan = table.Column<string>(nullable: true),
                    SoTaiKhoan = table.Column<string>(nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    GhiChu = table.Column<string>(nullable: true),
                    ChiNhanhNganHangId = table.Column<Guid>(nullable: true),
                    NganHangId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNganHang", x => x.TaiKhoanId);
                    table.ForeignKey(
                        name: "FK_TaiKhoanNganHang_ChiNhanhNganHang_ChiNhanhNganHangId",
                        column: x => x.ChiNhanhNganHangId,
                        principalTable: "ChiNhanhNganHang",
                        principalColumn: "ChiNhanhNganHangId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaiKhoanNganHang_NganHang_NganHangId",
                        column: x => x.NganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNganHang_ChiNhanhNganHangId",
                table: "TaiKhoanNganHang",
                column: "ChiNhanhNganHangId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNganHang_NganHangId",
                table: "TaiKhoanNganHang",
                column: "NganHangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoanNganHang");
        }
    }
}
