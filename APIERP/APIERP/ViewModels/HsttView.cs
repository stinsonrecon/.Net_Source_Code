using System;
using System.ComponentModel.DataAnnotations;

namespace APIERP.ViewModels
{
    public class HsttView
    {
        public string UserName { get; set; }
        public string MaDonViERP { get; set; }
        public string MaChiNhanh { get; set; }
        public string TrangThaiAp { get; set; }
        public string TrangThaiGl { get; set; }
        public string TrangThaiCm { get; set; }

        //[DataType(DataType.Date)]
        public string NgayChungTu { get; set; }
        public string NoiDungThanhToan { get; set; }
        public string MaHSTT { get; set; }
        public string TenHSTT { get; set; }
        public string TaiKhoanNguoiThuHuong { get; set; }
        public string TenNguoiThuHuong { get; set; }
        public string MaChiNhanhNhan { get; set; }
        public string LoaiTien { get; set; }
        public decimal TyGia { get; set; }
        public decimal SoTienNguyenTe { get; set; }
        public decimal SoTienQuyDoi { get; set; }
        public string TrangThaiHSTT { get; set; }
        public string v_result { get; set; }
    }
}
