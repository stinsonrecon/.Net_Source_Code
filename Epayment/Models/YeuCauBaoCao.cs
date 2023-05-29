using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class YeuCauBaoCao
    {
        [Key]
        public int Id { get; set; }
        public int? Nam { get; set; }
        public int? ChuKy { get; set; }
        [StringLength(10)]
        public int? LoaiChuKy { get; set; }
        public string TieuDe { get; set; }
        public DateTime? HanHoanThanh { get; set; }
        [StringLength(500)]
        public string MoTa { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string NguoiTaoId { get; set; }
        public int? DaXoa { get; set; }
        public int ChotSoLieu { get; set; }    //0: 15 thang    ; 1:// cuoi thang
        public string FileBaoCaoHieuChinh { get; set; }
    }
}