using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YeuCauId",
                table: "BaoCao");

            migrationBuilder.AddColumn<int>(
                name: "SapXep",
                table: "XacNhanBaoCao",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteName",
                table: "BaoCao",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "BaoCao",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SapXep",
                table: "XacNhanBaoCao");

            migrationBuilder.DropColumn(
                name: "SiteName",
                table: "BaoCao");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "BaoCao");

            migrationBuilder.AddColumn<int>(
                name: "YeuCauId",
                table: "BaoCao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
