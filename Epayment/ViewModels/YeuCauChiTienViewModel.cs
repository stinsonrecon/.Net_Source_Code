using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Epayment.ViewModels
{
    public class YeuCauChiTienParams : PaginationViewModel
    {
        public Guid YeuCauChiTienId { get; set; }
        public Guid ChungTuId { get; set; }
        public decimal SoTienTT { get; set; }
        public Guid NganHangChiId { get; set; }
        public string TaiKhoanChi { get; set; }
        public string TaiKhoanThuHuong { get; set; }
        public string NguoiThuHuong { get; set; }
        public Guid NganHangThuHuongId { get; set; }
        public int TrangThaiChi { get; set; }
        public DateTime? NgayYeuCauChi { get; set; }
        public string NguoiYeuCauChiId { get; set; }
        public string ChuKy { get; set; }
        public string MaKetQuaChi { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime ThoiGianChi { get; set; }
        public string MatKhau { get; set; }
        public string MaHoSo { get; set; }
        public string HoSoId { get; set; }
        public Guid RequestID { get; set; }
        // public string NoiDungTT { get; set; }
        // public string LoaiGiaoDich { get; set; }
        // public string LoaiPhi { get; set; }
        // public string LoaiTienTe { get; set; }
        // public string TenNganHangChuyen { get; set; }
        // public string TenNganHangNhan { get; set; }
        // public string SoChungTuERP { get; set; }
        public ChungTu ChungTu { get; set; }
        public DateTime NgayGiaoDichThucTe { get; set; }
    }
    
    public class ResponseGetYeuCauChiTien
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetYeuCauChiTien(string message, string errorcode, bool success, int totalRecord, object items)
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
}
