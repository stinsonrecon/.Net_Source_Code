using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Models;

namespace Epayment.ViewModels
{
    public class KySoTaiLieuParams : PaginationViewModel
    {
        public Guid KySoTaiLieuId { get; set; }
        public Guid HoSoThanhToanId { get; set; }
        public string NoiDungKy { get; set; }
        public DateTime NgayKy { get; set; }
        public string NguoiKyId { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public bool? DaXoa { get; set; }
        public int CapKy { get; set; }
        public bool DaKy { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid BuocPheDuyetId { get; set; }
    }

    public class DanhSachKySoTaiLieuViewModel
    {
        public Guid KySoTaiLieuId { get; set; }
        public Guid HoSoThanhToanId { get; set; }
        public string MaHoSoTT { get; set; }
        public string TenHoSoTT { get; set; }
        public string LoaiTien { get; set; }
        public string FileKyHoSoTT { get; set; }
        public string FileGocHoSoTT { get; set; }
        public int CapKyHoSoTT { get; set; }
        public Decimal SoTienNguyenTe { get; set; }
        public Decimal SoTienThanhToan { get; set; }
        public string NoiDungKy { get; set; }
        public DateTime? NgayKy { get; set; }
        public string NguoiKyId { get; set; }
        public string TenNguoiKy { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public bool? DaXoa { get; set; }
        public int CapKy { get; set; }
        public int DaKy { get; set; }
        public DateTime NgayTao { get; set; }
        public string TaiLieuGocConvertBase64 => ConvertToBase64(TaiLieuGoc);
        public string TaiLieuKyConvertBase64 => ConvertToBase64(TaiLieuKy);
        public string ConvertToBase64 (string filePDF){
            try
            {
                if (!String.IsNullOrEmpty(filePDF))
                {
                    byte[] bytes = File.ReadAllBytes(filePDF);
                    string file = Convert.ToBase64String(bytes);
                    return file;
                }
                return "";
            }catch(Exception ex)
            {
                return "";
            }
        }
    }
    public class ResponseGetKySoTaiLieu
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetKySoTaiLieu(string message, string errorcode, bool success, int totalRecord, object items)
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
    public class KySoPhieuThamTraPram{
        public Guid? GiayToId { get; set; }
        public Guid HoSoThanhToanId { get; set; }
        public int CapKy { get; set; }
        public string NoiDungKy { get; set; }
        public string NguoiKyId { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid BuocPheDuyetId { get; set; }
    }
    public class KySoPhieuThamTraHoSoViewModel
    {
        public Guid GiayToId { get; set; }
        public Guid HoSoThanhToanId { get; set; }
        public int CapKy { get; set; }
        public string NoiDungKy { get; set; }
        public string NguoiKyId { get; set; }
        public string TenNguoiKy { get; set; }
        public string TaiLieuKy { get; set; }
    }
}
