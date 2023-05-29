using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class EPayment_v8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NguoiTaoId",
                table: "HoSoThanhToan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_NguoiTaoId",
                table: "HoSoThanhToan",
                column: "NguoiTaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoThanhToan_AspNetUsers_NguoiTaoId",
                table: "HoSoThanhToan",
                column: "NguoiTaoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoSoThanhToan_AspNetUsers_NguoiTaoId",
                table: "HoSoThanhToan");

            migrationBuilder.DropIndex(
                name: "IX_HoSoThanhToan_NguoiTaoId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "NguoiTaoId",
                table: "HoSoThanhToan");
        }
    }
}
