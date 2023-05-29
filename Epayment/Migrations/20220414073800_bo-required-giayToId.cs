using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class borequiredgiayToId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_ThaoTacBuocPheDuyet_GiayTo_GiayToId",
                table: "ThaoTacBuocPheDuyet");

            migrationBuilder.AlterColumn<Guid>(
                name: "GiayToId",
                table: "ThaoTacBuocPheDuyet",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThaoTacBuocPheDuyet_GiayTo_GiayToId",
                table: "ThaoTacBuocPheDuyet",
                column: "GiayToId",
                principalTable: "GiayTo",
                principalColumn: "GiayToId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropForeignKey(
                name: "FK_ThaoTacBuocPheDuyet_GiayTo_GiayToId",
                table: "ThaoTacBuocPheDuyet");

            migrationBuilder.AlterColumn<Guid>(
                name: "GiayToId",
                table: "ThaoTacBuocPheDuyet",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThaoTacBuocPheDuyet_GiayTo_GiayToId",
                table: "ThaoTacBuocPheDuyet",
                column: "GiayToId",
                principalTable: "GiayTo",
                principalColumn: "GiayToId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
