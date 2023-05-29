using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class CauHinhNganHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.CreateTable(
                name: "CauHinhNganHang",
                columns: table => new
                {
                    CauHinhNganHangId = table.Column<Guid>(nullable: false),
                    NganHangId = table.Column<Guid>(nullable: true),
                    LinkTransferUrl = table.Column<string>(nullable: true),
                    FTPPath = table.Column<string>(nullable: true),
                    FTPUserName = table.Column<string>(nullable: true),
                    FTPPassword = table.Column<string>(nullable: true),
                    EVNDSAPath = table.Column<string>(nullable: true),
                    PassPhraseDSA = table.Column<string>(nullable: true),
                    BankDSAPath = table.Column<string>(nullable: true),
                    BankRSAPath = table.Column<string>(nullable: true),
                    BankApprover = table.Column<string>(nullable: true),
                    BankSenderAddr = table.Column<string>(nullable: true),
                    BankModel = table.Column<string>(maxLength: 50, nullable: true),
                    Version = table.Column<string>(maxLength: 50, nullable: true),
                    SoftwareProviderId = table.Column<string>(maxLength: 50, nullable: true),
                    BankLanguage = table.Column<string>(maxLength: 50, nullable: true),
                    BankAppointedApprover = table.Column<string>(maxLength: 50, nullable: true),
                    BankChannel = table.Column<string>(maxLength: 50, nullable: true),
                    BankMerchantId = table.Column<string>(maxLength: 50, nullable: true),
                    BankClientIP = table.Column<string>(maxLength: 50, nullable: true),
                    DaXoa = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhNganHang", x => x.CauHinhNganHangId);
                    table.ForeignKey(
                        name: "FK_CauHinhNganHang_NganHang_NganHangId",
                        column: x => x.NganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhNganHang_NganHangId",
                table: "CauHinhNganHang",
                column: "NganHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropTable(
                name: "CauHinhNganHang");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
