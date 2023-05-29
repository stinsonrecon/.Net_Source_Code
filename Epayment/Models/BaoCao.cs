using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class BaoCao
    {
        [Key]
        public int Id { get; set; }
        public int LoaiBaoCao { get; set; }
        public int TypeId { get; set; }
        public int SoLieuId { get; set; }
        [StringLength(200)]
        public string TenBaoCao { get; set; }
        [StringLength(255)]
        public string UrlLink { get; set; }
        [StringLength(500)]
        public string MoTa { get; set; }
        public int? TrangThai { get; set; }
        public int? DonViId { get; set; }
        public int? LinhVucId { get; set; }
        [StringLength(10)]
        public string ChuKy { get; set; }
        [StringLength(255)]
        public string ViewName { get; set; }
        public string ViewId { get; set; }
        public string SiteName { get; set; }
        public string SiteId { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
        public string NguoiCapNhatId { get; set; }
        public int? Order { get; set; }
    }
}
