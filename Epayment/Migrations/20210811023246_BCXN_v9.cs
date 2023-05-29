using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN_v9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiBaoCao",
                table: "YeuCauBaoCao");

            migrationBuilder.DropColumn(
                name: "ThoiGian",
                table: "YeuCauBaoCao");

            migrationBuilder.AlterColumn<int>(
                name: "ChuKy",
                table: "YeuCauBaoCao",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiChuKy",
                table: "YeuCauBaoCao",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiChuKy",
                table: "YeuCauBaoCao");

            migrationBuilder.AlterColumn<string>(
                name: "ChuKy",
                table: "YeuCauBaoCao",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiBaoCao",
                table: "YeuCauBaoCao",
                type: "int",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThoiGian",
                table: "YeuCauBaoCao",
                type: "int",
                nullable: true);
        }
    }
}
