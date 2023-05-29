using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class EPayment_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaXoa",
                table: "NganHang",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TenNganHang",
                table: "NganHang",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaLoaiHoSo",
                table: "LoaiHoSo",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DaKy",
                table: "KySoChungTu",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CoBanCung",
                table: "HoSoThanhToan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CoHieuLuc",
                table: "HoSoThanhToan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LanThanhToan",
                table: "HoSoThanhToan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TenVietTat",
                table: "HoSoThanhToan",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenYeuCauThanhToan",
                table: "HoSoThanhToan",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "GiayToLoaiHoSo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaXoa",
                table: "NganHang");

            migrationBuilder.DropColumn(
                name: "TenNganHang",
                table: "NganHang");

            //migrationBuilder.DropColumn(
            //    name: "MaLoaiHoSo",
            //    table: "LoaiHoSo");

            migrationBuilder.DropColumn(
                name: "DaKy",
                table: "KySoChungTu");

            migrationBuilder.DropColumn(
                name: "CoBanCung",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "CoHieuLuc",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "LanThanhToan",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "TenVietTat",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "TenYeuCauThanhToan",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "GiayToLoaiHoSo");
        }
    }
}
