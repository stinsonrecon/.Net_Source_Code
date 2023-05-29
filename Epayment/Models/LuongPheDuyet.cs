using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;

namespace Epayment.Models
{
    public class LuongPheDuyet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        // public Guid LuongPheDuyetId { get; set; }
        public Guid BuocThucHienId { get; set; }
        // public DateTime ThoiGianGiao { get; set; }
        public VaiTro VaiTro { get; set; }
        public TrangThaiHoSo TrangThaiHoSo { get; set; }
        // public DateTime ThoiGianTiepNhan { get; set; }
        // public string GhiChu { get; set; }
        public LoaiHoSo LoaiHoSo { get; set; }
        public string TenBuoc { get; set; }
        public int ChuyenSangERP { get; set; }
        public int CoThamTra { get; set; }
        public int ThuTu { get; set; }
        public int DaXoa { get; set; }
    }
}
