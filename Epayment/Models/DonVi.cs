using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class DonVi
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string TenDonVi { get; set; }
        [StringLength(50)]
        public string MaDonVi { get; set; }
        [StringLength(255)]
        public string DiaChi { get; set; }
        [StringLength(50)]
        public string SoDienThoai { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(500)]
        public string MoTa { get; set; }
        public int? DonViChaId { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public int? LaPhongBan { get; set; }
        public int? TrangThai { get; set; }
        public string ERPMaCongTy { get; set; }
        public string ERPMaChiNhanh { get; set; }
        public string ERPIDDonVi { get; set; }
        public int  DOfficeDonVi { get; set; }
    }
}
