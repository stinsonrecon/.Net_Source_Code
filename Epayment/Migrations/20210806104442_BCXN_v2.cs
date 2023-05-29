using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiChuKy",
                table: "YeuCauBaoCao");

            migrationBuilder.AddColumn<int>(
                name: "LoaiBaoCao",
                table: "YeuCauBaoCao",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiBaoCao",
                table: "YeuCauBaoCao");

            migrationBuilder.AddColumn<string>(
                name: "LoaiChuKy",
                table: "YeuCauBaoCao",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
