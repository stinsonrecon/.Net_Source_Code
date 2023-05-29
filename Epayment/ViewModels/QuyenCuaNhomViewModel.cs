using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class QuyenCuaNhomViewModel
    {
        public int id { get; set; }
        public int NhomQuyenId { get; set; }
        public int QuyenId { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
}
