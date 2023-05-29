using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class ChiNhanhNganHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiNhanhNganHang",
                columns: table => new
                {
                    ChiNhanhNganHangId = table.Column<Guid>(nullable: false),
                    NganHangId = table.Column<Guid>(nullable: true),
                    MaChiNhanhErp = table.Column<string>(nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    DaXoa = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiNhanhNganHang", x => x.ChiNhanhNganHangId);
                    table.ForeignKey(
                        name: "FK_ChiNhanhNganHang_NganHang_NganHangId",
                        column: x => x.NganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiNhanhNganHang_NganHangId",
                table: "ChiNhanhNganHang",
                column: "NganHangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiNhanhNganHang");
        }
    }
}
