using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Models
{
    public class ChiNhanhNganHang
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ChiNhanhNganHangId { get; set; }
        public NganHang NganHang { get; set; }
        public string MaChiNhanhErp { get; set; }
        public string TenChiNhanh { get; set; }
        public string LoaiTaiKhoan { get; set; }
        public int TrangThai { get; set; }
        public bool DaXoa { get; set; }
    }
}
