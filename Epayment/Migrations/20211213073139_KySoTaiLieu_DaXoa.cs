using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class KySoTaiLieu_DaXoa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.CreateTable(
                name: "KySoTaiLieu",
                columns: table => new
                {
                    KySoTaiLieuId = table.Column<Guid>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    NoiDungKy = table.Column<string>(nullable: true),
                    NgayKy = table.Column<DateTime>(nullable: true),
                    CapKy = table.Column<int>(nullable: false),
                    NguoiKyId = table.Column<string>(nullable: true),
                    TaiLieuGoc = table.Column<string>(nullable: true),
                    TaiLieuKy = table.Column<string>(nullable: true),
                    DaXoa = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KySoTaiLieu", x => x.KySoTaiLieuId);
                    table.ForeignKey(
                        name: "FK_KySoTaiLieu_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KySoTaiLieu_AspNetUsers_NguoiKyId",
                        column: x => x.NguoiKyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KySoTaiLieu_HoSoThanhToanHoSoId",
                table: "KySoTaiLieu",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_KySoTaiLieu_NguoiKyId",
                table: "KySoTaiLieu",
                column: "NguoiKyId");

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

            migrationBuilder.DropTable(
                name: "KySoTaiLieu");

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
