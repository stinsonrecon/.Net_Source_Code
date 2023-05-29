using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class HoSoThanhToanaddDonViYeuCauId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoThanhToan_DonVi_DonViYeuCauId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AlterColumn<int>(
                name: "DonViYeuCauId",
                table: "HoSoThanhToan",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_DonVi_DonViYeuCauId",
                table: "HoSoThanhToan",
                column: "DonViYeuCauId",
                principalTable: "DonVi",
                principalColumn: "Id",
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
                name: "FK_HoSoThanhToan_DonVi_DonViYeuCauId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AlterColumn<int>(
                name: "DonViYeuCauId",
                table: "HoSoThanhToan",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_DonVi_DonViYeuCauId",
                table: "HoSoThanhToan",
                column: "DonViYeuCauId",
                principalTable: "DonVi",
                principalColumn: "Id",
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
