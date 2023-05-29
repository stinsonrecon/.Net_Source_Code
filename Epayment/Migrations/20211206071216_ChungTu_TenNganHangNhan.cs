using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class ChungTu_TenNganHangNhan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenVietTat",
                table: "NganHang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNganHangNhan",
                table: "ChungTu",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenVietTat",
                table: "NganHang");

            migrationBuilder.DropColumn(
                name: "TenNganHangNhan",
                table: "ChungTu");
        }
    }
}
