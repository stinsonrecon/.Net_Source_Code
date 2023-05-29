using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;

namespace Epayment.Models
{
    public class LichSuHoSoTT
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid LichSuId { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
        public string MaHoSo { get; set; }
        public string TenHoSo { get; set; }
        public int NamHoSo { get; set; }
        public DateTime HanThanhToan { get; set; }
        public int MucDoUuTien { get; set; }
        public int ThoiGianLuTru { get; set; }
        public string GhiChu { get; set; }
        public LoaiHoSo LoaiHoSo { get; set; }
        public DonVi DonViYeuCau { get; set; }
        public DonVi BoPhanCau { get; set; }
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
        public Guid NganHangThuHuong { get; set; }
        public int HinhThucTT { get; set; }
        public DateTime ThoiGianCapNhat { get; set; }
        public ApplicationUser NguoiCapNhat { get; set; }
    }
}
