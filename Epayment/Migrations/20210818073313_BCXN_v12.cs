using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TieuDe",
                table: "YeuCauBaoCao",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "LinhVucBaoCao",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TieuDe",
                table: "YeuCauBaoCao");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "LinhVucBaoCao");
        }
    }
}
