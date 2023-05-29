using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThoiGian",
                table: "YeuCauBaoCao",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGian",
                table: "YeuCauBaoCao");
        }
    }
}
