using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCXN.Migrations
{
    public partial class BCXN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true),
                    ChucNangDefault = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 10, nullable: true),
                    LastName = table.Column<string>(maxLength: 10, nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    DonViId = table.Column<int>(nullable: true),
                    NhomQuyenId = table.Column<string>(nullable: true),
                    ChangePassWordDate = table.Column<DateTime>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: true),
                    IsActive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaoCao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiBaoCao = table.Column<int>(nullable: false),
                    YeuCauId = table.Column<int>(nullable: false),
                    TenBaoCao = table.Column<string>(maxLength: 200, nullable: true),
                    UrlLink = table.Column<string>(maxLength: 255, nullable: true),
                    MoTa = table.Column<string>(maxLength: 500, nullable: true),
                    TrangThai = table.Column<int>(nullable: true),
                    DonViId = table.Column<int>(nullable: true),
                    LinhVucId = table.Column<int>(nullable: true),
                    ChuKy = table.Column<string>(maxLength: 10, nullable: true),
                    ViewName = table.Column<string>(maxLength: 255, nullable: true),
                    ViewId = table.Column<int>(nullable: true),
                    SiteId = table.Column<int>(nullable: true),
                    TrangThaiHoatDong = table.Column<int>(nullable: false),
                    ThoiGianTao = table.Column<DateTime>(nullable: true),
                    NguoiTaoId = table.Column<string>(nullable: true),
                    ThoiGianCapNhat = table.Column<DateTime>(nullable: true),
                    NguoiCapNhatId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChucNang",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(maxLength: 50, nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    ChucNangChaId = table.Column<int>(nullable: false),
                    MoTa = table.Column<string>(maxLength: 500, nullable: true),
                    TrangThai = table.Column<int>(nullable: false),
                    LinkUrl = table.Column<string>(maxLength: 255, nullable: true),
                    Icon = table.Column<string>(maxLength: 50, nullable: true),
                    Order = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucNang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChucNangNhom",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChucNangId = table.Column<int>(nullable: false),
                    NhomQuyenId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucNangNhom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DonVi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDonVi = table.Column<string>(maxLength: 255, nullable: true),
                    MaDonVi = table.Column<string>(maxLength: 50, nullable: true),
                    DiaChi = table.Column<string>(maxLength: 255, nullable: true),
                    SoDienThoai = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(maxLength: 500, nullable: true),
                    DonViChaId = table.Column<int>(nullable: true),
                    NguoiTaoId = table.Column<string>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: true),
                    LaPhongBan = table.Column<int>(nullable: true),
                    TrangThai = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonVi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichSuXacNhanBaoCao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaoCaoId = table.Column<int>(nullable: false),
                    YeuCauBaoCaoId = table.Column<int>(nullable: false),
                    GhiChu = table.Column<string>(maxLength: 300, nullable: true),
                    TrangThaiDuLieu = table.Column<int>(nullable: true),
                    TrangThaiXacNhan = table.Column<int>(nullable: true),
                    ThoiGianXacNhan = table.Column<DateTime>(nullable: true),
                    NguoiTaoId = table.Column<string>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuXacNhanBaoCao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinhVucBaoCao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(maxLength: 50, nullable: true),
                    LinhVucChaId = table.Column<int>(nullable: false),
                    TrangThaiHoatDong = table.Column<int>(nullable: false),
                    DaXoa = table.Column<int>(nullable: false),
                    ThoiGianTao = table.Column<DateTime>(nullable: true),
                    NguoiTaoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhVucBaoCao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(nullable: true),
                    MatKhau = table.Column<string>(nullable: true),
                    HoTen = table.Column<string>(nullable: true),
                    DiaChi = table.Column<string>(nullable: true),
                    SoDienThoai = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    MoTa = table.Column<string>(nullable: true),
                    NguoiTaoId = table.Column<int>(nullable: false),
                    NhomQuyenId = table.Column<int>(nullable: false),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    ThoiGianDangNhap = table.Column<DateTime>(nullable: false),
                    DonViId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomQuyen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhomQuyen = table.Column<string>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    NguoiTaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomQuyen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quyen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuyen = table.Column<string>(nullable: true),
                    MoTa = table.Column<string>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    NguoiTaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuyenCuaNhom",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhomQuyenId = table.Column<int>(nullable: false),
                    QuyenId = table.Column<int>(nullable: false),
                    ThoiGianTao = table.Column<DateTime>(nullable: false),
                    NguoiTaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyenCuaNhom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XacNhanBaoCao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaoCaoId = table.Column<int>(nullable: false),
                    YeuCauBaoCaoId = table.Column<int>(nullable: false),
                    GhiChu = table.Column<string>(nullable: true),
                    TrangThaiDuLieu = table.Column<int>(nullable: true),
                    TrangThaiXacNhan = table.Column<int>(nullable: true),
                    ThoiGianXacNhan = table.Column<DateTime>(nullable: true),
                    NguoiTaoId = table.Column<string>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XacNhanBaoCao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauBaoCao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChuKy = table.Column<string>(maxLength: 10, nullable: true),
                    HanHoanThanh = table.Column<DateTime>(nullable: true),
                    MoTa = table.Column<string>(maxLength: 500, nullable: true),
                    TrangThai = table.Column<int>(nullable: true),
                    ThoiGianTao = table.Column<DateTime>(nullable: true),
                    NguoiTaoId = table.Column<string>(nullable: true),
                    DaXoa = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauBaoCao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BaoCao");

            migrationBuilder.DropTable(
                name: "ChucNang");

            migrationBuilder.DropTable(
                name: "ChucNangNhom");

            migrationBuilder.DropTable(
                name: "DonVi");

            migrationBuilder.DropTable(
                name: "LichSuXacNhanBaoCao");

            migrationBuilder.DropTable(
                name: "LinhVucBaoCao");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "NhomQuyen");

            migrationBuilder.DropTable(
                name: "Quyen");

            migrationBuilder.DropTable(
                name: "QuyenCuaNhom");

            migrationBuilder.DropTable(
                name: "XacNhanBaoCao");

            migrationBuilder.DropTable(
                name: "YeuCauBaoCao");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
