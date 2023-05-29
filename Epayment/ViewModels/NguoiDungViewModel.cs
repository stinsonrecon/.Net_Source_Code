using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class NguoiDungViewModel
    {
        public int id { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MoTa { get; set; }
        public int NguoiTaoId { get; set; }
        public int NhomQuyenId { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianDangNhap { get; set; }
    }
}
