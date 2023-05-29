using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class Epayment_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ERPIDDonVi",
                table: "DonVi",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ERPMaChiNhanh",
                table: "DonVi",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ERPMaCongTy",
                table: "DonVi",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GiayTo",
                columns: table => new
                {
                    GiayToId = table.Column<Guid>(nullable: false),
                    TenGiayTo = table.Column<string>(maxLength: 255, nullable: true),
                    MaGiayTo = table.Column<string>(nullable: true),
                    KySo = table.Column<int>(nullable: false),
                    Nguon = table.Column<string>(nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    NgayTao = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiayTo", x => x.GiayToId);
                });

            migrationBuilder.CreateTable(
                name: "LoaiHoSo",
                columns: table => new
                {
                    LoaiHoSoId = table.Column<Guid>(nullable: false),
                    TenLoaiHoSo = table.Column<string>(maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(nullable: false),
                    MoTa = table.Column<string>(nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    DaXoa = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiHoSo", x => x.LoaiHoSoId);
                });

            migrationBuilder.CreateTable(
                name: "NganHang",
                columns: table => new
                {
                    NganHangId = table.Column<Guid>(nullable: false),
                    MaNganHang = table.Column<string>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    TrangThaiTiepNhan = table.Column<int>(nullable: false),
                    ThoiGianTiepNhan = table.Column<DateTime>(nullable: false),
                    GhiChu = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NganHang", x => x.NganHangId);
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiHoSo",
                columns: table => new
                {
                    TrangThaiHoSoId = table.Column<Guid>(nullable: false),
                    TenTrangThaiHoSo = table.Column<string>(nullable: true),
                    GiaTri = table.Column<int>(nullable: false),
                    TrangThai = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiHoSo", x => x.TrangThaiHoSoId);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    VaiTroId = table.Column<Guid>(nullable: false),
                    TenVaiTro = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.VaiTroId);
                });

            migrationBuilder.CreateTable(
                name: "GiayToLoaiHoSo",
                columns: table => new
                {
                    GiayToLoaiHoSoId = table.Column<Guid>(nullable: false),
                    GiayToId = table.Column<Guid>(nullable: true),
                    LoaiHoSoId = table.Column<Guid>(nullable: true),
                    NgayTao = table.Column<DateTime>(nullable: false),
                    BatBuoc = table.Column<int>(nullable: false),
                    Nguon = table.Column<string>(nullable: true),
                    PheDuyetKySo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiayToLoaiHoSo", x => x.GiayToLoaiHoSoId);
                    table.ForeignKey(
                        name: "FK_GiayToLoaiHoSo_GiayTo_GiayToId",
                        column: x => x.GiayToId,
                        principalTable: "GiayTo",
                        principalColumn: "GiayToId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GiayToLoaiHoSo_LoaiHoSo_LoaiHoSoId",
                        column: x => x.LoaiHoSoId,
                        principalTable: "LoaiHoSo",
                        principalColumn: "LoaiHoSoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoThanhToan",
                columns: table => new
                {
                    HoSoId = table.Column<Guid>(nullable: false),
                    MaHoSo = table.Column<string>(maxLength: 50, nullable: true),
                    TenHoSo = table.Column<string>(maxLength: 255, nullable: true),
                    NamHoSo = table.Column<int>(nullable: false),
                    ThoiGianLuTru = table.Column<int>(nullable: false),
                    MucDoUuTien = table.Column<int>(nullable: false),
                    HanThanhToan = table.Column<DateTime>(nullable: false),
                    GhiChu = table.Column<string>(nullable: true),
                    LoaiHoSoId = table.Column<Guid>(nullable: true),
                    DonViYeuCauId = table.Column<int>(nullable: true),
                    BoPhanCauIdId = table.Column<int>(nullable: true),
                    NgayGui = table.Column<DateTime>(nullable: false),
                    NgayTiepNhan = table.Column<DateTime>(nullable: false),
                    NgayDuyet = table.Column<DateTime>(nullable: false),
                    SoTien = table.Column<decimal>(nullable: false),
                    NgayLapCT = table.Column<DateTime>(nullable: false),
                    TrangThaiHoSo = table.Column<int>(nullable: false),
                    BuocThucHien = table.Column<int>(nullable: false),
                    NgayTao = table.Column<DateTime>(nullable: false),
                    NguoiThuHuong = table.Column<string>(nullable: true),
                    SoTienThucTe = table.Column<decimal>(nullable: false),
                    SoTKThuHuong = table.Column<string>(nullable: true),
                    NganHangId = table.Column<Guid>(nullable: true),
                    HinhThucTT = table.Column<int>(nullable: false),
                    HinhThucChi = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoThanhToan", x => x.HoSoId);
                    table.ForeignKey(
                        name: "FK_HoSoThanhToan_DonVi_BoPhanCauIdId",
                        column: x => x.BoPhanCauIdId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoThanhToan_DonVi_DonViYeuCauId",
                        column: x => x.DonViYeuCauId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoThanhToan_LoaiHoSo_LoaiHoSoId",
                        column: x => x.LoaiHoSoId,
                        principalTable: "LoaiHoSo",
                        principalColumn: "LoaiHoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoThanhToan_NganHang_NganHangId",
                        column: x => x.NganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LuongPheDuyet",
                columns: table => new
                {
                    BuocThucHienId = table.Column<Guid>(nullable: false),
                    VaiTroId = table.Column<Guid>(nullable: true),
                    TrangThaiHoSoId = table.Column<Guid>(nullable: true),
                    LoaiHoSoId = table.Column<Guid>(nullable: true),
                    TenBuoc = table.Column<string>(nullable: true),
                    ChuyenSangERP = table.Column<int>(nullable: false),
                    CoThamTra = table.Column<int>(nullable: false),
                    ThuTu = table.Column<int>(nullable: false),
                    DaXoa = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuongPheDuyet", x => x.BuocThucHienId);
                    table.ForeignKey(
                        name: "FK_LuongPheDuyet_LoaiHoSo_LoaiHoSoId",
                        column: x => x.LoaiHoSoId,
                        principalTable: "LoaiHoSo",
                        principalColumn: "LoaiHoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LuongPheDuyet_TrangThaiHoSo_TrangThaiHoSoId",
                        column: x => x.TrangThaiHoSoId,
                        principalTable: "TrangThaiHoSo",
                        principalColumn: "TrangThaiHoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LuongPheDuyet_VaiTro_VaiTroId",
                        column: x => x.VaiTroId,
                        principalTable: "VaiTro",
                        principalColumn: "VaiTroId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietGiayToHSTT",
                columns: table => new
                {
                    ChiTietHoSoId = table.Column<Guid>(nullable: false),
                    GiayToId = table.Column<Guid>(nullable: true),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    FileDinhKem = table.Column<string>(nullable: true),
                    TrangThaiGiayTo = table.Column<int>(nullable: false),
                    NgayCapNhat = table.Column<DateTime>(nullable: false),
                    NguoiCapNhatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietGiayToHSTT", x => x.ChiTietHoSoId);
                    table.ForeignKey(
                        name: "FK_ChiTietGiayToHSTT_GiayTo_GiayToId",
                        column: x => x.GiayToId,
                        principalTable: "GiayTo",
                        principalColumn: "GiayToId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietGiayToHSTT_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietGiayToHSTT_AspNetUsers_NguoiCapNhatId",
                        column: x => x.NguoiCapNhatId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChungTu",
                columns: table => new
                {
                    ChungTuId = table.Column<Guid>(nullable: false),
                    ChungTuERPId = table.Column<string>(nullable: true),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    NgayYeuCauLapCT = table.Column<DateTime>(nullable: false),
                    NgayLapChungTu = table.Column<DateTime>(nullable: false),
                    SoChungTuERP = table.Column<string>(nullable: true),
                    SoTaiKhoanChuyen = table.Column<string>(nullable: true),
                    MaNganHangNhan = table.Column<string>(nullable: true),
                    TenNganHangChuyen = table.Column<string>(nullable: true),
                    TenTaiKhoanChuyen = table.Column<string>(nullable: true),
                    MaChiNhanhChuyen = table.Column<string>(nullable: true),
                    MaChiNhanhNhan = table.Column<string>(nullable: true),
                    MaNganHangChuyen = table.Column<string>(nullable: true),
                    NoiDungTT = table.Column<string>(nullable: true),
                    LoaiPhi = table.Column<string>(nullable: true),
                    LoaiGiaoDich = table.Column<string>(nullable: true),
                    SoTien = table.Column<decimal>(nullable: false),
                    LoaiTienTe = table.Column<string>(nullable: true),
                    TyGia = table.Column<decimal>(nullable: false),
                    TenNguoiChuyen = table.Column<string>(nullable: true),
                    DiaChiChuyen = table.Column<string>(nullable: true),
                    MaTinhTPChuyen = table.Column<string>(nullable: true),
                    MaNuocChuyen = table.Column<string>(nullable: true),
                    NguoiYeuCauLapId = table.Column<string>(nullable: true),
                    NguoiDuyetId = table.Column<string>(nullable: true),
                    NgayDuyet = table.Column<DateTime>(nullable: false),
                    DonViThanhToanId = table.Column<int>(nullable: true),
                    TrangThaiCT = table.Column<int>(nullable: false),
                    KySoId = table.Column<Guid>(nullable: false),
                    NgayKy = table.Column<DateTime>(nullable: false),
                    CapKy = table.Column<int>(nullable: false),
                    CTGS_GL = table.Column<bool>(nullable: false),
                    CTGS_AP = table.Column<bool>(nullable: false),
                    CTGS_CM = table.Column<bool>(nullable: false),
                    GhiChu = table.Column<string>(nullable: true),
                    DaXoa = table.Column<bool>(nullable: false),
                    TenTaiKhoanNhan = table.Column<string>(nullable: true),
                    SoTaiKhoanNhan = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChungTu", x => x.ChungTuId);
                    table.ForeignKey(
                        name: "FK_ChungTu_DonVi_DonViThanhToanId",
                        column: x => x.DonViThanhToanId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChungTu_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChungTu_AspNetUsers_NguoiDuyetId",
                        column: x => x.NguoiDuyetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChungTu_AspNetUsers_NguoiYeuCauLapId",
                        column: x => x.NguoiYeuCauLapId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChungTuThanhToan",
                columns: table => new
                {
                    ChungTuId = table.Column<Guid>(nullable: false),
                    SoChungTu = table.Column<int>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    SoTien = table.Column<decimal>(nullable: false),
                    NgayLapCT = table.Column<DateTime>(nullable: false),
                    TrangThai = table.Column<int>(nullable: false),
                    HinhThucChi = table.Column<int>(nullable: false),
                    NguoiLapId = table.Column<string>(nullable: true),
                    NguoiKyId = table.Column<string>(nullable: true),
                    NgayKy = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChungTuThanhToan", x => x.ChungTuId);
                    table.ForeignKey(
                        name: "FK_ChungTuThanhToan_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChungTuThanhToan_AspNetUsers_NguoiKyId",
                        column: x => x.NguoiKyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChungTuThanhToan_AspNetUsers_NguoiLapId",
                        column: x => x.NguoiLapId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuChiTietGiayTo",
                columns: table => new
                {
                    ChiTietHoSoId = table.Column<Guid>(nullable: false),
                    GiayToId = table.Column<Guid>(nullable: true),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    FileDinhKem = table.Column<string>(nullable: true),
                    TrangThaiGiayTo = table.Column<int>(nullable: false),
                    NgayCapNhat = table.Column<DateTime>(nullable: false),
                    NguoiCapNhatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuChiTietGiayTo", x => x.ChiTietHoSoId);
                    table.ForeignKey(
                        name: "FK_LichSuChiTietGiayTo_GiayTo_GiayToId",
                        column: x => x.GiayToId,
                        principalTable: "GiayTo",
                        principalColumn: "GiayToId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuChiTietGiayTo_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuChiTietGiayTo_AspNetUsers_NguoiCapNhatId",
                        column: x => x.NguoiCapNhatId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuHoSoTT",
                columns: table => new
                {
                    LichSuId = table.Column<Guid>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    MaHoSo = table.Column<string>(nullable: true),
                    TenHoSo = table.Column<string>(nullable: true),
                    NamHoSo = table.Column<int>(nullable: false),
                    HanThanhToan = table.Column<DateTime>(nullable: false),
                    MucDoUuTien = table.Column<int>(nullable: false),
                    ThoiGianLuTru = table.Column<int>(nullable: false),
                    GhiChu = table.Column<string>(nullable: true),
                    LoaiHoSoId = table.Column<Guid>(nullable: true),
                    DonViYeuCauId = table.Column<int>(nullable: true),
                    BoPhanCauId = table.Column<int>(nullable: true),
                    NgayGui = table.Column<DateTime>(nullable: false),
                    NgayTiepNhan = table.Column<DateTime>(nullable: false),
                    NgayDuyet = table.Column<DateTime>(nullable: false),
                    SoTien = table.Column<decimal>(nullable: false),
                    NgayLapCT = table.Column<DateTime>(nullable: false),
                    TrangThaiHoSo = table.Column<int>(nullable: false),
                    BuocThucHien = table.Column<int>(nullable: false),
                    NgayTao = table.Column<DateTime>(nullable: false),
                    NguoiThuHuong = table.Column<string>(nullable: true),
                    SoTienThucTe = table.Column<decimal>(nullable: false),
                    SoTKThuHuong = table.Column<string>(nullable: true),
                    NganHangThuHuong = table.Column<Guid>(nullable: false),
                    HinhThucTT = table.Column<int>(nullable: false),
                    ThoiGianCapNhat = table.Column<DateTime>(nullable: false),
                    NguoiCapNhatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuHoSoTT", x => x.LichSuId);
                    table.ForeignKey(
                        name: "FK_LichSuHoSoTT_DonVi_BoPhanCauId",
                        column: x => x.BoPhanCauId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuHoSoTT_DonVi_DonViYeuCauId",
                        column: x => x.DonViYeuCauId,
                        principalTable: "DonVi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuHoSoTT_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuHoSoTT_LoaiHoSo_LoaiHoSoId",
                        column: x => x.LoaiHoSoId,
                        principalTable: "LoaiHoSo",
                        principalColumn: "LoaiHoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuHoSoTT_AspNetUsers_NguoiCapNhatId",
                        column: x => x.NguoiCapNhatId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiThuHuong",
                columns: table => new
                {
                    NguoiThuHuongId = table.Column<Guid>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    SoTKThuHuong = table.Column<string>(nullable: true),
                    TenNguoiThuHuong = table.Column<string>(nullable: true),
                    SoTienThanhToan = table.Column<decimal>(nullable: false),
                    HinhThucTT = table.Column<int>(nullable: false),
                    NganHangThuHuongNganHangId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiThuHuong", x => x.NguoiThuHuongId);
                    table.ForeignKey(
                        name: "FK_NguoiThuHuong_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NguoiThuHuong_NganHang_NganHangThuHuongNganHangId",
                        column: x => x.NganHangThuHuongNganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongXuLy",
                columns: table => new
                {
                    PhanCongXuLyId = table.Column<Guid>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    NguoiXuLyId = table.Column<string>(nullable: true),
                    ThoiGianGiao = table.Column<DateTime>(nullable: false),
                    TrangThaiTiepNhan = table.Column<int>(nullable: false),
                    GhiChu = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongXuLy", x => x.PhanCongXuLyId);
                    table.ForeignKey(
                        name: "FK_PhanCongXuLy_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanCongXuLy_AspNetUsers_NguoiXuLyId",
                        column: x => x.NguoiXuLyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PheDuyetHoSoTT",
                columns: table => new
                {
                    PheDuyetHoSoTTId = table.Column<Guid>(nullable: false),
                    HoSoThanhToanHoSoId = table.Column<Guid>(nullable: true),
                    NguoiThucHienId = table.Column<string>(nullable: true),
                    NgayThucHien = table.Column<DateTime>(nullable: false),
                    TrangThaiHoSo = table.Column<string>(nullable: true),
                    TrangThaiPheDuyet = table.Column<string>(nullable: true),
                    BuocThucHien = table.Column<string>(nullable: true),
                    NoiDung = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PheDuyetHoSoTT", x => x.PheDuyetHoSoTTId);
                    table.ForeignKey(
                        name: "FK_PheDuyetHoSoTT_HoSoThanhToan_HoSoThanhToanHoSoId",
                        column: x => x.HoSoThanhToanHoSoId,
                        principalTable: "HoSoThanhToan",
                        principalColumn: "HoSoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PheDuyetHoSoTT_AspNetUsers_NguoiThucHienId",
                        column: x => x.NguoiThucHienId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHachToan",
                columns: table => new
                {
                    ChiTietHachToanId = table.Column<Guid>(nullable: false),
                    TKNo = table.Column<string>(nullable: true),
                    TKCo = table.Column<string>(nullable: true),
                    SoTien = table.Column<decimal>(nullable: false),
                    ChungTuId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHachToan", x => x.ChiTietHachToanId);
                    table.ForeignKey(
                        name: "FK_ChiTietHachToan_ChungTu_ChungTuId",
                        column: x => x.ChungTuId,
                        principalTable: "ChungTu",
                        principalColumn: "ChungTuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KySoChungTu",
                columns: table => new
                {
                    KySoId = table.Column<Guid>(nullable: false),
                    ChungTuId = table.Column<Guid>(nullable: true),
                    NoiDungKy = table.Column<string>(nullable: true),
                    NgayKy = table.Column<DateTime>(nullable: false),
                    NguoiKyId = table.Column<string>(nullable: true),
                    DuLieuKy = table.Column<string>(nullable: true),
                    ChuKyXML = table.Column<string>(nullable: true),
                    CapKy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KySoChungTu", x => x.KySoId);
                    table.ForeignKey(
                        name: "FK_KySoChungTu_ChungTu_ChungTuId",
                        column: x => x.ChungTuId,
                        principalTable: "ChungTu",
                        principalColumn: "ChungTuId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KySoChungTu_AspNetUsers_NguoiKyId",
                        column: x => x.NguoiKyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauChiTien",
                columns: table => new
                {
                    YeuCauChiTienId = table.Column<Guid>(nullable: false),
                    ChungTuId = table.Column<Guid>(nullable: true),
                    SoTienTT = table.Column<decimal>(nullable: false),
                    NganHangThuHuongNganHangId = table.Column<Guid>(nullable: true),
                    TaiKhoanChi = table.Column<string>(nullable: true),
                    TaiKhoanThuHuong = table.Column<string>(nullable: true),
                    NguoiThuHuong = table.Column<string>(nullable: true),
                    NganHangChiNganHangId = table.Column<Guid>(nullable: true),
                    TrangThaiChi = table.Column<int>(nullable: false),
                    NgayYeuCauChi = table.Column<DateTime>(nullable: false),
                    NguoiYeuCauChiId = table.Column<string>(nullable: true),
                    ChuKy = table.Column<string>(nullable: true),
                    MaKetQuaChi = table.Column<string>(nullable: true),
                    ThoiGianChi = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauChiTien", x => x.YeuCauChiTienId);
                    table.ForeignKey(
                        name: "FK_YeuCauChiTien_ChungTu_ChungTuId",
                        column: x => x.ChungTuId,
                        principalTable: "ChungTu",
                        principalColumn: "ChungTuId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauChiTien_NganHang_NganHangChiNganHangId",
                        column: x => x.NganHangChiNganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauChiTien_NganHang_NganHangThuHuongNganHangId",
                        column: x => x.NganHangThuHuongNganHangId,
                        principalTable: "NganHang",
                        principalColumn: "NganHangId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauChiTien_AspNetUsers_NguoiYeuCauChiId",
                        column: x => x.NguoiYeuCauChiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiayToHSTT_GiayToId",
                table: "ChiTietGiayToHSTT",
                column: "GiayToId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiayToHSTT_HoSoThanhToanHoSoId",
                table: "ChiTietGiayToHSTT",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGiayToHSTT_NguoiCapNhatId",
                table: "ChiTietGiayToHSTT",
                column: "NguoiCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHachToan_ChungTuId",
                table: "ChiTietHachToan",
                column: "ChungTuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTu_DonViThanhToanId",
                table: "ChungTu",
                column: "DonViThanhToanId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTu_HoSoThanhToanHoSoId",
                table: "ChungTu",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTu_NguoiDuyetId",
                table: "ChungTu",
                column: "NguoiDuyetId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTu_NguoiYeuCauLapId",
                table: "ChungTu",
                column: "NguoiYeuCauLapId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTuThanhToan_HoSoThanhToanHoSoId",
                table: "ChungTuThanhToan",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTuThanhToan_NguoiKyId",
                table: "ChungTuThanhToan",
                column: "NguoiKyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChungTuThanhToan_NguoiLapId",
                table: "ChungTuThanhToan",
                column: "NguoiLapId");

            migrationBuilder.CreateIndex(
                name: "IX_GiayToLoaiHoSo_GiayToId",
                table: "GiayToLoaiHoSo",
                column: "GiayToId");

            migrationBuilder.CreateIndex(
                name: "IX_GiayToLoaiHoSo_LoaiHoSoId",
                table: "GiayToLoaiHoSo",
                column: "LoaiHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_BoPhanCauIdId",
                table: "HoSoThanhToan",
                column: "BoPhanCauIdId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_DonViYeuCauId",
                table: "HoSoThanhToan",
                column: "DonViYeuCauId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_LoaiHoSoId",
                table: "HoSoThanhToan",
                column: "LoaiHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoThanhToan_NganHangId",
                table: "HoSoThanhToan",
                column: "NganHangId");

            migrationBuilder.CreateIndex(
                name: "IX_KySoChungTu_ChungTuId",
                table: "KySoChungTu",
                column: "ChungTuId");

            migrationBuilder.CreateIndex(
                name: "IX_KySoChungTu_NguoiKyId",
                table: "KySoChungTu",
                column: "NguoiKyId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChiTietGiayTo_GiayToId",
                table: "LichSuChiTietGiayTo",
                column: "GiayToId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChiTietGiayTo_HoSoThanhToanHoSoId",
                table: "LichSuChiTietGiayTo",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChiTietGiayTo_NguoiCapNhatId",
                table: "LichSuChiTietGiayTo",
                column: "NguoiCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoTT_BoPhanCauId",
                table: "LichSuHoSoTT",
                column: "BoPhanCauId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoTT_DonViYeuCauId",
                table: "LichSuHoSoTT",
                column: "DonViYeuCauId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoTT_HoSoThanhToanHoSoId",
                table: "LichSuHoSoTT",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoTT_LoaiHoSoId",
                table: "LichSuHoSoTT",
                column: "LoaiHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoSoTT_NguoiCapNhatId",
                table: "LichSuHoSoTT",
                column: "NguoiCapNhatId");

            migrationBuilder.CreateIndex(
                name: "IX_LuongPheDuyet_LoaiHoSoId",
                table: "LuongPheDuyet",
                column: "LoaiHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_LuongPheDuyet_TrangThaiHoSoId",
                table: "LuongPheDuyet",
                column: "TrangThaiHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_LuongPheDuyet_VaiTroId",
                table: "LuongPheDuyet",
                column: "VaiTroId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiThuHuong_HoSoThanhToanHoSoId",
                table: "NguoiThuHuong",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiThuHuong_NganHangThuHuongNganHangId",
                table: "NguoiThuHuong",
                column: "NganHangThuHuongNganHangId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongXuLy_HoSoThanhToanHoSoId",
                table: "PhanCongXuLy",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongXuLy_NguoiXuLyId",
                table: "PhanCongXuLy",
                column: "NguoiXuLyId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetHoSoTT_HoSoThanhToanHoSoId",
                table: "PheDuyetHoSoTT",
                column: "HoSoThanhToanHoSoId");

            migrationBuilder.CreateIndex(
                name: "IX_PheDuyetHoSoTT_NguoiThucHienId",
                table: "PheDuyetHoSoTT",
                column: "NguoiThucHienId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauChiTien_ChungTuId",
                table: "YeuCauChiTien",
                column: "ChungTuId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauChiTien_NganHangChiNganHangId",
                table: "YeuCauChiTien",
                column: "NganHangChiNganHangId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauChiTien_NganHangThuHuongNganHangId",
                table: "YeuCauChiTien",
                column: "NganHangThuHuongNganHangId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauChiTien_NguoiYeuCauChiId",
                table: "YeuCauChiTien",
                column: "NguoiYeuCauChiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietGiayToHSTT");

            migrationBuilder.DropTable(
                name: "ChiTietHachToan");

            migrationBuilder.DropTable(
                name: "ChungTuThanhToan");

            migrationBuilder.DropTable(
                name: "GiayToLoaiHoSo");

            migrationBuilder.DropTable(
                name: "KySoChungTu");

            migrationBuilder.DropTable(
                name: "LichSuChiTietGiayTo");

            migrationBuilder.DropTable(
                name: "LichSuHoSoTT");

            migrationBuilder.DropTable(
                name: "LuongPheDuyet");

            migrationBuilder.DropTable(
                name: "NguoiThuHuong");

            migrationBuilder.DropTable(
                name: "PhanCongXuLy");

            migrationBuilder.DropTable(
                name: "PheDuyetHoSoTT");

            migrationBuilder.DropTable(
                name: "YeuCauChiTien");

            migrationBuilder.DropTable(
                name: "GiayTo");

            migrationBuilder.DropTable(
                name: "TrangThaiHoSo");

            migrationBuilder.DropTable(
                name: "VaiTro");

            migrationBuilder.DropTable(
                name: "ChungTu");

            migrationBuilder.DropTable(
                name: "HoSoThanhToan");

            migrationBuilder.DropTable(
                name: "LoaiHoSo");

            migrationBuilder.DropTable(
                name: "NganHang");

            migrationBuilder.DropColumn(
                name: "ERPIDDonVi",
                table: "DonVi");

            migrationBuilder.DropColumn(
                name: "ERPMaChiNhanh",
                table: "DonVi");

            migrationBuilder.DropColumn(
                name: "ERPMaCongTy",
                table: "DonVi");
        }
    }
}
