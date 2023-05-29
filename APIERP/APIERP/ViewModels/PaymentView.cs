using System;
using System.ComponentModel.DataAnnotations;

namespace APIERP.ViewModels
{
    public class PaymentView
    {
        public int MaChungTuERP { get; set; }
        public string MaDonViERP { get; set; }
        public string MaHSTT { get; set; }

        //[DataType(DataType.Date)]
        public string NgayChuyenNganHang { get; set; }
        public string NgayNganHangThanhToan { get; set; }
        public string ChuKy1 { get; set; }
        public string ChuKy2 { get; set; }
        public string ChuKy3 { get; set; }
        public string ChuKy4 { get; set; }
        public string TrangThaiHSTT { get; set; }
        public string v_result { get; set; }
    }
}
