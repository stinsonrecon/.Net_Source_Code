using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BCXN.ViewModels;
using System.IO;

namespace Epayment.ViewModels
{
    public class DanhSachHoSoViewModel
    {
        public Guid Id { get; set; }
        public string maHoSo { get; set; }
        public string tenVietTat { get; set; }
        public int namHoSo { get; set; }
        public string ghiChu { get; set; }
        public DateTime ngayGui { get; set; }
        public DateTime ngayDuyet { get; set; }
        public DateTime ngayLapCT { get; set; }
        public int trangThaiHoSo { get; set; }
        public int buocThucHien { get; set; }
        public DateTime ngayTao { get; set; }
        public string nguoiThuHuong { get; set; }
        public int thoiGianLuutru { get; set; }

        public string soTKThuHuong { get; set; }
        public int hinhThucTT { get; set; }
        public int hinhThucChi {get;set;}
        public string tenNguoiTao { get; set; }
        public DateTime ngayTiepNhan { get; set; }
        public DateTime thoiGianThanhToan { get; set; }//hạn thanh toán
        public int mucDoUuTien { get; set; }
        public string boPhanYeuCau { get; set; }
        public int boPhanYeuCauId { get; set; }
        public string LoaiHoSo { get; set; }
        public Guid LoaiHoSoId { get; set; }
        public decimal tongTienDeNghiTT { get; set; }
        public string vaiTroPheDuyet { get; set; }
        public int donViId { get; set; }
        public string donVi { get; set; }
        public decimal soTien { get; set; }
        public decimal SoTienThucTe { get; set; }
        public Guid nganHangThuHuongId { get; set; }
        public string nganHangThuHuong { get; set; }
        public string NguoiDuyetId { get; set; }
        public string TenVietTatHoSo { get; set; }
        public int CoHieuLuc { get; set; }
        public int CoBanCung { get; set; }
        public int LanThanhToan { get; set; }
        public string TenYeuCauThanhToan { get; set; }
        public string TenNganHang { get; set; }
        public string LoaiTienTe { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string TenNguoiDuyet { get; set; }
        public string NoiDungDuyet { get ; set; } 
        public Guid TKTaiKhoanId { get; set; }
        public string TKTenTaiKhoan { get; set; }
        public string TKSoTk { get; set; }
        public int TKTrangThaiTaiKhoan { get; set ; }
        public Guid ChiNhanhId { get; set; }
        public string TenChiNhanh { get; set; }

       public string ThucTeTenNguoiNT { get; set; }
       public Decimal ThucTeSoTienNT { get; set;}
       public string ThucTeSoTaiKhoanNT { get; set; }
       public string ThuTeTenNganHangNT { get; set; }
       public string ThuTeTenChiNhanhNganHangNT { get; set;}
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public DateTime NgayKy { get; set; }
        public int CapKy { get; set; }
        public List<Guid> TaiLieuKyId { get; set; }
        public string NguoiTaoId { get; set; }
        public string NoiDungThanhToan { get; set; }
        public List<HoSoThamChieuViewModel> HoSoThamChieu { get; set; }
        public BuocPheDuyetChiTietViewModel BuocPheDuyet { get; set; }
        public string NguoiThamTraId { get; set; }
        public string NguoiThamTra { get; set; }
    }
    public class ResponseHoSoViewModel : ResponseWithPaginationViewModel
    {
        public List<DanhSachHoSoViewModel> Data { get; set; }
        public ResponseHoSoViewModel(List<DanhSachHoSoViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
    public class ResponseChiTietHSTTViewModel : ResponseWithPaginationViewModel
    {
        public List<ChiTietGiayToHSTTViewModel> Data { get; set; }
        public ResponseChiTietHSTTViewModel(List<ChiTietGiayToHSTTViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
}