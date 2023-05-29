using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class ChungTu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ChungTuId { get; set; }
        public string ChungTuERPId { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
        public DateTime NgayYeuCauLapCT { get; set; }
        public DateTime NgayLapChungTu { get; set; }
        public string SoChungTuERP { get; set; }
        public string SoTaiKhoanChuyen { get; set; }
        public string MaNganHangNhan { get; set; }
        public string TenNganHangChuyen { get; set; }
        public string TenTaiKhoanChuyen { get; set; }
        public string MaChiNhanhChuyen { get; set; }
        public string MaChiNhanhNhan { get; set; }
        public string MaNganHangChuyen { get; set; }
        //public NganHang NganHangChuyen { get; set; }
        //public NganHang NganHangNhan { get; set; }
        public string NoiDungTT { get; set; }
        public string LoaiPhi { get; set; }
        public string LoaiGiaoDich { get; set; }
        public decimal SoTien { get; set; }
        public string LoaiTienTe { get; set; }
        public decimal TyGia { get; set; }
        public string TenNguoiChuyen { get; set; }
        public string DiaChiChuyen { get; set; }
        public string DiaChiNhan { get; set; }
        public string MaTinhTPChuyen { get; set; }
        public string MaTinhTPNhan { get; set; }
        public string MaNuocChuyen { get; set; }
        public string MaNuocNhan { get; set; }
        public ApplicationUser NguoiYeuCauLap { get; set; }
        public string NguoiYeuCauLapId { get; set; }
        public ApplicationUser NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public DonVi DonViThanhToan { get; set; } // đơn vị thụ hưởng
        public int TrangThaiCT { get; set; }
        public Guid? KySoId { get; set; }
        public DateTime? NgayKy { get; set; }
        public int CapKy { get; set; }
        // public int TrangThaiKyDuyet { get; set; }
        public bool CTGS_GL { get; set; }
        public bool CTGS_AP { get; set; }
        public bool CTGS_CM { get; set; }
        public string GhiChu { get; set; }
        public bool DaXoa { get; set; }
        public string TenTaiKhoanNhan { get; set; }
        public string SoTaiKhoanNhan { get; set; }
        public string LoaiGiaoDichCha { get; set; }
        public string TenNganHangNhan { get; set; }
        public DateTime NgayGiaoDichThucTe { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public string TenLoaiGiaoDich { get; set; }
        public int LoaiChungTu { get; set; }
        public string TaiLieuGocCTGS { get; set; }
        public string TaiLieuKyCTGS { get; set; }
        public string DonViGhiNhanCongNo { get; set; }
        public string NguoiCapNhatCTGS { get; set; }
    }
}
