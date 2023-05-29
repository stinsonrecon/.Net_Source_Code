using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class ThaoTacBuocPheDuyetaddDiDenBuocPheDuyetId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<Guid>(
                name: "DiDenBuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThaoTacBuocPheDuyet_DiDenBuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet",
                column: "DiDenBuocPheDuyetId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThaoTacBuocPheDuyet_BuocPheDuyet_DiDenBuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet",
                column: "DiDenBuocPheDuyetId",
                principalTable: "BuocPheDuyet",
                principalColumn: "BuocPheDuyetId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_ThaoTacBuocPheDuyet_BuocPheDuyet_DiDenBuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet");

            migrationBuilder.DropIndex(
                name: "IX_ThaoTacBuocPheDuyet_DiDenBuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet");

            migrationBuilder.DropColumn(
                name: "DiDenBuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet");

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
