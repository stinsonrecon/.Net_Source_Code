using System;
using System.Collections.Generic;
using System.Text;
using Epayment.Models;
using BCXN.Repositories;
using BCXN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BCXN.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<XacNhanBaoCao> XacNhanBaoCao { get; set; }
        public DbSet<BaoCao> BaoCao { get; set; }
        public DbSet<LichSuXacNhanBaoCao> LichSuXacNhanBaoCao { get; set; }
        public DbSet<DonVi> DonVi { get; set; }
        // public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<NhomQuyen> NhomQuyen { get; set; }
        public DbSet<Quyen> Quyen { get; set; }
        // public DbSet<QuyenCuaNhom> QuyenCuaNhom { get; set; }
        public DbSet<YeuCauBaoCao> YeuCauBaoCao { get; set; }
        public DbSet<LinhVucBaoCao> LinhVucBaoCao { get; set; }
        public DbSet<ChucNang> ChucNang { get; set; }
        public DbSet<ChucNangNhom> ChucNangNhom { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }


        public DbSet<GiayTo> GiayTo { get; set; }
        public DbSet<GiayToLoaiHoSo> GiayToLoaiHoSo{get;set;}
        public DbSet<LoaiHoSo> LoaiHoSo{get;set;}
        public DbSet<HoSoThanhToan> HoSoThanhToan{get;set;}
        public DbSet<ChiTietGiayToHSTT> ChiTietGiayToHSTT { get;set;}
        public DbSet<PhanCongXuLy> PhanCongXuLy{get;set;}
        public DbSet<LichSuChiTietGiayTo> LichSuChiTietGiayTo { get;set;}
        public DbSet<KySoChungTu> KySoChungTu { get;set;}
        public DbSet<PheDuyetHoSoTT> PheDuyetHoSoTT{get;set;}
        public DbSet<ChungTuThanhToan> ChungTuThanhToan{get;set;}
        public DbSet<LichSuHoSoTT> LichSuHoSoTT { get; set; }
        public DbSet<ChungTu> ChungTu { get; set; }
        public DbSet<NguoiThuHuong> NguoiThuHuong { get; set; }
        public DbSet<YeuCauChiTien> YeuCauChiTien { get; set; }
        public DbSet<NganHang> NganHang { get; set; }
        public DbSet<LuongPheDuyet> LuongPheDuyet { get; set; }
        // public DbSet<NguoiDungVaiTro> NguoiDungVaiTro { get; set; }
        // public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<VaiTro> VaiTro { get; set; }
        public DbSet<TrangThaiHoSo> TrangThaiHoSo { get; set; }
        public DbSet<ChiTietHachToan> ChiTietHachToan { get; set; }
        public DbSet<ChiNhanhNganHang> ChiNhanhNganHang { get; set; }
        public DbSet<TaiKhoanNganHang> TaiKhoanNganHang { get; set; }
        public DbSet<KySoTaiLieu> KySoTaiLieu { get; set; }
        public DbSet<HoSoThamChieu> HoSoThamChieu { get; set; }
        public DbSet<QuocGia> QuocGia { get; set; }
        public DbSet<TinhThanhPho> TinhThanhPho { get; set; }

        public DbSet<QuyTrinhPheDuyet> QuyTrinhPheDuyet { get; set; }
        public DbSet<BuocPheDuyet> BuocPheDuyet { get; set; }
        public DbSet<QuaTrinhPheDuyet> QuaTrinhPheDuyet { get; set; }
        public DbSet<ThaoTacBuocPheDuyet> ThaoTacBuocPheDuyet { get; set; }

        public DbSet<CauHinhNganHang> CauHinhNganHang { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HoSoThanhToan>().HasIndex(p => p.MaHoSo).IsUnique();

            modelBuilder.Entity<ChungTu>().HasOne<ApplicationUser>(x => x.NguoiYeuCauLap)
                .WithMany()
                .HasForeignKey(x => x.NguoiYeuCauLapId);

            modelBuilder.Entity<HoSoThanhToan>().HasOne<ApplicationUser>(x => x.NguoiPheDuyetHoSo)
                .WithMany()
                .HasForeignKey(x => x.NguoiPheDuyetHoSoId);

            modelBuilder.Entity<KySoChungTu>().HasOne<ApplicationUser>(x => x.NguoiKy)
                .WithMany()
                .HasForeignKey(x => x.NguoiKyId);
            
            modelBuilder.Entity<HoSoThanhToan>().HasOne<LoaiHoSo>(x => x.LoaiHoSo)
                .WithMany()
                .HasForeignKey(x => x.LoaiHoSoId);
            
            modelBuilder.Entity<HoSoThanhToan>().HasOne<DonVi>(x => x.DonViYeuCau)
                .WithMany()
                .HasForeignKey(x => x.DonViYeuCauId);
        }
    }
}
