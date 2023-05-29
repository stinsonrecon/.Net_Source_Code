using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public  class HoSoThanhToan
    {
        public HoSoThanhToan(){
            
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid HoSoId { get; set; }
        [StringLength(50)]
        public string MaHoSo { get; set; }
        [StringLength(255)]
        public string TenHoSo { get; set; }
        public int NamHoSo { get; set; }
        public int ThoiGianLuTru { get; set; }
        public int MucDoUuTien { get; set; }
        public DateTime HanThanhToan { get; set; }
        public string GhiChu { get; set; }
        public LoaiHoSo LoaiHoSo { get; set; }
        public Guid LoaiHoSoId { get; set; }
        public DonVi DonViYeuCau { get; set; }
        public int DonViYeuCauId { get; set; }
        public DonVi BoPhanCauId { get; set; }
        public DateTime NgayGui { get; set; }
        public DateTime NgayTiepNhan { get; set; }
        public DateTime NgayDuyet { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayLapCT { get; set; }
        public int TrangThaiHoSo { get; set; }
        public int BuocThucHien { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiThuHuong { get; set; }
        public decimal SoTienThucTe { get; set; }
        public string SoTKThuHuong { get; set; }
        public NganHang NganHang { get; set; } 
        public int HinhThucTT { get; set; }
        public int HinhThucChi { get; set; }
        public string TenVietTat { get; set; }
        public int CoHieuLuc { get; set; }
        public int CoBanCung { get; set; }
        public int LanThanhToan { get; set; }
        public string TenYeuCauThanhToan { get; set; }
        public ApplicationUser NguoiTao { get; set; }
        public string LoaiTienTe { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public TaiKhoanNganHang TaiKhoanThuHuong { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public DateTime NgayKy { get; set; }
        public int CapKy { get; set; }
        public string NoiDungTT { get; set; }
        public Guid QuyTrinhPheDuyetId { get; set; }
        public Guid QuaTrinhPheDuyetId { get; set; }
        public ApplicationUser NguoiThamTra { get; set; }
        public ApplicationUser NguoiPheDuyetHoSo { get; set; }
        public string NguoiPheDuyetHoSoId { get; set; }
        public Guid ThaoTacVuaThucHienId { get; set; } // thao tác vừa thực hiện ở bước trước
    }
}
