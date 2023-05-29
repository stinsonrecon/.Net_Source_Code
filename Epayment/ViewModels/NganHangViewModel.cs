
using BCXN.ViewModels;
using Epayment.Models;
using System;
using System.Collections.Generic;

namespace Epayment.ViewModels
{
    public class NganHangViewModel
    {
        public Guid NganHangId { get; set; }
        public string MaNganHang { get; set; }
        public string MaNganHangERP { get; set; }
        public string LookupType { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int TrangThaiTiepNhan { get; set; }
        public DateTime ThoiGianTiepNhan { get; set; }
        public string GhiChu { get; set; }
        public string TenNganHang { get; set; }
        public string TenVietTat { get; set; }
        public bool DaXoa { get; set; }
        public string PhuongThucKetNoi { get; set; }
    }

    public class ResponseGetNganHang
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetNganHang(string message, string errorcode, bool success, int totalRecord, object items)
        {
            Data = new Dictionary<string, object>()
            {
                { "TotalRecord", totalRecord},
                { "Items", items }
            };
            Message = message;
            ErrorCode = errorcode;
            Success = success;
        }
    }

    public class ChiNhanhViewModel
    {
        public Guid ChiNhanhNganHangId { get; set; }
        public Guid NganHangId { get; set; }
        public string MaChiNhanhErp { get; set; }
        public int TrangThai { get; set; }
        public bool DaXoa { get; set; }
        public string TenChiNhanh { get; set; }
        public string TenNganHang { get; set; }
        public string LoaiTaiKhoan { get; set; }
    }

    public class ResponseGetChiNhanh
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetChiNhanh(string message, string errorcode, bool success, int totalRecord, object items)
        {
            Data = new Dictionary<string, object>()
            {
                { "TotalRecord", totalRecord},
                { "Items", items }
            };
            Message = message;
            ErrorCode = errorcode;
            Success = success;
        }
    }

    public class TaiKhoanNganHangViewModel
    {
        public string NguoiTaoId { get; set; }
        public Guid TaiKhoanId { get; set; }
        public Guid ChiNhanhNganHangId { get; set; }
        public Guid NganHangId { get; set; }
        public string TenTaiKhoan { get; set; }
        public string SoTaiKhoan { get; set; }
        public int TrangThai { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public string GhiChu { get; set; }
        public string TenChiNhanh { get; set; }
        public string TenNganHang { get; set; }
        public int DonViId { get; set; }
        public string TenDonVi { get ; set;} 
        public string MaChiNhanh { get; set; }
        public string MaNganHang { get; set; }
        public string TenNganHangVietTat { get; set; }
        public Guid QuocGiaId { get; set; }
        public string MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public Guid TinhTPId { get; set; }
        public string MaTinhTP { get; set; }
        public string TenTinhTP { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public bool LaTaiKhoanNhan { get; set; }
        public string LoaiTaiKhoan { get; set; }
    }

    public class ResponseGetTaiKhoanNganHang
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetTaiKhoanNganHang(string message, string errorcode, bool success, int totalRecord, object items)
        {
            Data = new Dictionary<string, object>()
            {
                { "TotalRecord", totalRecord},
                { "Items", items }
            };
            Message = message;
            ErrorCode = errorcode;
            Success = success;
        }
    }

    public class NganHangParams : PaginationViewModel
    {
        public string MaNganHang { get; set; }
        public string MaNganHangERP { get; set; }
        public string LookupType { get; set; }
        public int TrangThaiTiepNhan { get; set; }
        public DateTime ThoiGianTiepNhan { get; set; }
        public string GhiChu { get; set; }
        public string TenNganHang { get; set; }
        public string TenVietTat { get; set; }
        public string PhuongThucKetNoi { get; set; }
    }

    public class ChiNhanhNganHangParams : PaginationViewModel
    {
        public Guid NganHangId { get; set; }
        public string MaChiNhanhErp { get; set; }
        public string LoaiTaiKhoan { get; set; }
        public int TrangThai { get; set; }
        public bool DaXoa { get; set; }
        public string TenChiNhanh { get; set; }
    }

    public class TaiKhoanNganHangParams : PaginationViewModel
    {
        public string NguoiTaoId { get; set; }
        public Guid NganHangId { get; set; }
        public Guid ChiNhanhNganHangId { get; set; }
        public string TenTaiKhoan { get; set; }
        public string SoTaiKhoan { get; set; }
        public int TrangThai { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public string GhiChu { get; set; }
        public int DonViId { get; set; }
        public Guid QuocGiaId { get; set; }
        public Guid TinhTPId { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public bool LaTaiKhoanNhan { get; set; }
        public string MaDonViERP { get; set; }
        public string MaChiNhanhNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string TenChiNhanhNganHang { get; set; }
        public string SwiftCode { get; set; }
        public string ThanhPho { get; set; }
        public string QuocGia { get; set; }
        public bool Active { get; set; }
        public bool OldActive { get; set; }
        public string TenNganHangVietTat { get; set; }
        public string LoaiTaiKhoan { get; set; }
    }

    public class NganHangPagination : PaginationViewModel
    {
        public Guid? NganHangId { get; set; }
        public string MaNganHang { get; set; }
        public string MaNganHangERP { get; set; }
        public string LookupType { get; set; }
        public int? TrangThaiTiepNhan { get; set; }
        public DateTime? ThoiGianTiepNhan { get; set; }
        public string GhiChu { get; set; }
        public string TenNganHang { get; set; }
        public string TenVietTat { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public bool? DaXoa { get; set; }
        public string SearchString { get; set; }
    }

    public class ChiNhanhNganHangPagination : PaginationViewModel
    {
        public Guid? ChiNhanhNganHangId { get; set; }
        public Guid? NganHangId { get; set; }
        public string MaChiNhanhErp { get; set; }
        public int? TrangThai { get; set; }
        public bool? DaXoa { get; set; }
        public string TenChiNhanh { get; set; }
        public string SearchString { get; set; }
        public string LoaiTaiKhoan { get; set; }
    }

    public class TaiKhoanNganHangPagination : PaginationViewModel
    {
        public Guid? TaiKhoanId { get; set; }
        public Guid? NganHangId { get; set; }
        public Guid? ChiNhanhNganHangId { get; set; }
        public string TenTaiKhoan { get; set; }
        public string SoTaiKhoan { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? ThoiGianTao { get; set; }
        public string GhiChu { get; set; }
        public string SearchString { get; set; }
        public int? DonViId { get; set;}
    }
}