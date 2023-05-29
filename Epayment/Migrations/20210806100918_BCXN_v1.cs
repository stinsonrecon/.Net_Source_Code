using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoaiChuKy",
                table: "YeuCauBaoCao",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Nam",
                table: "YeuCauBaoCao",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiChuKy",
                table: "YeuCauBaoCao");

            migrationBuilder.DropColumn(
                name: "Nam",
                table: "YeuCauBaoCao");
        }
    }
}
