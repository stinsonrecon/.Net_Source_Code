using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class LichSuXacNhanBaoCao
    {
        [Key]
        public int Id { get; set; }
        public int BaoCaoId { get; set; }
        public int YeuCauBaoCaoId { get; set; }
        [StringLength(300)]
        public string GhiChu { get; set; }
        public int? TrangThaiDuLieu { get; set; }
        public int? TrangThaiXacNhan { get; set; }
        public DateTime? ThoiGianXacNhan { get; set; }
        public string NguoiTaoId { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string FileBaoCao { get; set; }
    }
}
