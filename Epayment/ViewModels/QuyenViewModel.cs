using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class QuyenViewModel
    {
        public int id { get; set; }
        public string TenQuyen { get; set; }
        public string MoTa { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
}
