using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class Epayment_QuyTrinhPheDuyet_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.AddColumn<Guid>(
                name: "QuaTrinhPheDuyetId",
                table: "HoSoThanhToan",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "QuyTrinhPheDuyetId",
                table: "HoSoThanhToan",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "QuyTrinhPheDuyet",
                columns: table => new
                {
                    QuyTrinhPheDuyetId = table.Column<Guid>(nullable: false),
                    LoaiHoSoId = table.Column<Guid>(nullable: false),
                    TenQuyTrinh = table.Column<string>(nullable: false),
                    NgayHieuLuc = table.Column<DateTime>(nullable: false),
                    MoTa = table.Column<string>(nullable: true),
                    TrangThai = table.Column<bool>(nullable: false),
                    DaXoa = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyTrinhPheDuyet", x => x.QuyTrinhPheDuyetId);
                    table.ForeignKey(
                        name: "FK_QuyTrinhPheDuyet_LoaiHoSo_LoaiHoSoId",
                        column: x => x.LoaiHoSoId,
                        principalTable: "LoaiHoSo",
                        principalColumn: "LoaiHoSoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuocPheDuyet",
                columns: table => new
                {
                    BuocPheDuyetId = table.Column<Guid>(nullable: false),
                    QuyTrinhPheDuyetId = table.Column<Guid>(nullable: false),
                    BuocPheDuyetTruocId = table.Column<Guid>(nullable: true),
                    BuocPheDuyetSauId = table.Column<Guid>(nullable: true),
                    TrangThaiHoSo = table.Column<string>(nullable: false),
                    TrangThaiChungTu = table.Column<string>(nullable: false),
                    TenBuoc = table.Column<string>(nullable: false),
                    ThuTu = table.Column<int>(nullable: false),
                    DaXoa = table.Column<bool>(nullable: false),
                    NguoiThucHien = table.Column<string>(nullable: false),
                    ThoiGianXuLy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuocPheDuyet", x => x.BuocPheDuyetId);
                    table.ForeignKey(
                        name: "FK_BuocPheDuyet_QuyTrinhPheDuyet_QuyTrinhPheDuyetId",
                        column: x => x.QuyTrinhPheDuyetId,
                        principalTable: "QuyTrinhPheDuyet",
                        principalColumn: "QuyTrinhPheDuyetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuaTrinhPheDuyet",
                columns: table => new
                {
                    QuaTrinhPheDuyetId = table.Column<Guid>(nullable: false),
                    BuocPheDuyetId = table.Column<Guid>(nullable: false),
                    HoSoId = table.Column<Guid>(nullable: false),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    TrangThaiXuLy = table.Column<int>(nullable: false),
                    ThoiGianXuLy = table.Column<DateTime>(nullable: false),
                    NguoiXuLyId = table.Column<Guid>(nullable: false),
                    NguoiXuLyId1 = table.Column<string>(nullable: true),
                    DaXoa = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuaTrinhPheDuyet", x => x.QuaTrinhPheDuyetId);
                    table.ForeignKey(
                        name: "FK_QuaTrinhPheDuyet_BuocPheDuyet_BuocPheDuyetId",
                        column: x => x.BuocPheDuyetId,
                        principalTable: "BuocPheDuyet",
                        principalColumn: "BuocPheDuyetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuaTrinhPheDuyet_AspNetUsers_NguoiXuLyId1",
                        column: x => x.NguoiXuLyId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThaoTacBuocPheDuyet",
                columns: table => new
                {
                    ThaoTacBuocPheDuyetId = table.Column<Guid>(nullable: false),
                    BuocPheDuyetId = table.Column<Guid>(nullable: false),
                    HanhDong = table.Column<string>(nullable: false),
                    KySo = table.Column<bool>(nullable: false),
                    LoaiKy = table.Column<int>(nullable: false),
                    GiayToId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThaoTacBuocPheDuyet", x => x.ThaoTacBuocPheDuyetId);
                    table.ForeignKey(
                        name: "FK_ThaoTacBuocPheDuyet_BuocPheDuyet_BuocPheDuyetId",
                        column: x => x.BuocPheDuyetId,
                        principalTable: "BuocPheDuyet",
                        principalColumn: "BuocPheDuyetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThaoTacBuocPheDuyet_GiayTo_GiayToId",
                        column: x => x.GiayToId,
                        principalTable: "GiayTo",
                        principalColumn: "GiayToId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_QuaTrinhPheDuyetId",
                table: "HoSoThanhToan",
                column: "QuaTrinhPheDuyetId",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_BuocPheDuyet_QuyTrinhPheDuyetId",
                table: "BuocPheDuyet",
                column: "QuyTrinhPheDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhPheDuyet_BuocPheDuyetId",
                table: "QuaTrinhPheDuyet",
                column: "BuocPheDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuaTrinhPheDuyet_NguoiXuLyId1",
                table: "QuaTrinhPheDuyet",
                column: "NguoiXuLyId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuyTrinhPheDuyet_LoaiHoSoId",
                table: "QuyTrinhPheDuyet",
                column: "LoaiHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_ThaoTacBuocPheDuyet_BuocPheDuyetId",
                table: "ThaoTacBuocPheDuyet",
                column: "BuocPheDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_ThaoTacBuocPheDuyet_GiayToId",
                table: "ThaoTacBuocPheDuyet",
                column: "GiayToId");

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
                name: "FK_HoSoThanhToan_QuaTrinhPheDuyet_QuaTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                table: "RefreshToken");

            migrationBuilder.DropTable(
                name: "QuaTrinhPheDuyet");

            migrationBuilder.DropTable(
                name: "ThaoTacBuocPheDuyet");

            migrationBuilder.DropTable(
                name: "BuocPheDuyet");

            migrationBuilder.DropTable(
                name: "QuyTrinhPheDuyet");

            migrationBuilder.DropIndex(
                name: "IX_HoSoThanhToan_QuaTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "QuaTrinhPheDuyetId",
                table: "HoSoThanhToan");

            migrationBuilder.DropColumn(
                name: "QuyTrinhPheDuyetId",
                table: "HoSoThanhToan");

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
