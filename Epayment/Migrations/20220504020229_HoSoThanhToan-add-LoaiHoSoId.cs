using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class HoSoThanhToanaddLoaiHoSoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoThanhToan_LoaiHoSo_LoaiHoSoId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoaiHoSoId",
                table: "HoSoThanhToan",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_LoaiHoSo_LoaiHoSoId",
                table: "HoSoThanhToan",
                column: "LoaiHoSoId",
                principalTable: "LoaiHoSo",
                principalColumn: "LoaiHoSoId",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_HoSoThanhToan_LoaiHoSo_LoaiHoSoId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoaiHoSoId",
                table: "HoSoThanhToan",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_LoaiHoSo_LoaiHoSoId",
                table: "HoSoThanhToan",
                column: "LoaiHoSoId",
                principalTable: "LoaiHoSo",
                principalColumn: "LoaiHoSoId",
                onDelete: ReferentialAction.Restrict);

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
