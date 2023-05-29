using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class EPayment_v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaXoa",
                table: "KySoChungTu",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaXoa",
                table: "KySoChungTu");
        }
    }
}
