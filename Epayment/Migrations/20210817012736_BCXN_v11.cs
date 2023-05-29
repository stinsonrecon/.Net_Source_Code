using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileBaoCao",
                table: "XacNhanBaoCao",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileBaoCao",
                table: "LichSuXacNhanBaoCao",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileBaoCao",
                table: "XacNhanBaoCao");

            migrationBuilder.DropColumn(
                name: "FileBaoCao",
                table: "LichSuXacNhanBaoCao");
        }
    }
}
