using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.ViewModels
{
    public class NguoiHuongThuViewModel
    {
        public Guid NguoiThuHuongId { get; set; }
        public string SoTKThuHuong { get; set; }
        public string TenNguoiThuHuong { get; set; }
        public decimal SoTienThanhToan { get; set; }
        public int HinhThucTT { get; set; }
        public Guid NganHangHuongThu { get; set; }
    }
}
