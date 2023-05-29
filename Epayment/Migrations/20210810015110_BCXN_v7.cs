using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TieuDeId",
                table: "BaoCao");

            migrationBuilder.AddColumn<int>(
                name: "SoLieuId",
                table: "BaoCao",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLieuId",
                table: "BaoCao");

            migrationBuilder.AddColumn<int>(
                name: "TieuDeId",
                table: "BaoCao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
