using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;

namespace Epayment.Models
{
    public class TaiKhoanNganHang
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid TaiKhoanId { get; set; }
        public string TenTaiKhoan { get; set; }
        public string SoTaiKhoan { get; set; }
        public int TrangThai { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public string GhiChu { get; set; }
        public ChiNhanhNganHang ChiNhanhNganHang { get; set; }
        public NganHang NganHang { get; set; }
        public DonVi DonVi { get ; set; }
        public QuocGia QuocGia { get ; set; }
        public TinhThanhPho TinhTp { get ; set; }
        [StringLength(50)]
        public string Sdt { get ; set; }
        [StringLength(255)]
        public string DiaChi { get ; set; }
        [StringLength(50)]
        public string Email { get ; set; }
        public bool LaTaiKhoanNhan { get; set; }
    }
}
