using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;

namespace Epayment.Models
{
    public class NganHang
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid NganHangId { get; set; }
        public string MaNganHang { get; set; }
        public string MaNganHangERP { get; set; }
        public string LookupType { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int TrangThaiTiepNhan { get; set; }
        public DateTime ThoiGianTiepNhan { get; set; }
        public string GhiChu { get; set; }
        public string TenNganHang { get; set; }
        public bool DaXoa { get; set; }
        public string TenVietTat { get; set; }
        public string PhuongThucKetNoi { get; set; }
    }
}
