using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class TaiKhoanNganHang_DonVi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DonViId",
                table: "TaiKhoanNganHang",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoanNganHang_DonViId",
                table: "TaiKhoanNganHang",
                column: "DonViId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoanNganHang_DonVi_DonViId",
                table: "TaiKhoanNganHang",
                column: "DonViId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoanNganHang_DonVi_DonViId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoanNganHang_DonViId",
                table: "TaiKhoanNganHang");

            migrationBuilder.DropColumn(
                name: "DonViId",
                table: "TaiKhoanNganHang");
        }
    }
}
