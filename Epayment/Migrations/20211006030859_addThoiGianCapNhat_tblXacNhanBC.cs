using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class addThoiGianCapNhat_tblXacNhanBC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianCapNhatDuLieu",
                table: "XacNhanBaoCao",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGianCapNhatDuLieu",
                table: "XacNhanBaoCao");
        }
    }
}
