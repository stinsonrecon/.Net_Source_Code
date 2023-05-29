using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class VanPhongPham_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.CreateTable(
                name: "HoSoThamChieu",
                columns: table => new
                {
                    HoSoThamChieuId = table.Column<Guid>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoThamChieu", x => x.HoSoThamChieuId);
                    table.ForeignKey(
                        name: "FK_HoSoThamChieu_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThamChieu_HoSoThanhToanHoSoId",
                table: "HoSoThamChieu",
                column: "HoSoThanhToanHoSoId");

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
                name: "HoSoThamChieu");

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
