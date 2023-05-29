using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class LinhVucBaoCao
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string TieuDe { get; set; }
        public int LinhVucChaId { get; set; }
        public int TrangThaiHoatDong { get; set; }
        public int DaXoa { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
        public int? Order { get; set; }
    }
}
