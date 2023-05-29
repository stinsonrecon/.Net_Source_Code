using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Models;

namespace Epayment.ViewModels
{
    public class ChungTuParams : PaginationViewModel
    {
        public Guid ChungTuId { get; set; }
        public Guid HoSoThanhToanId { get; set; }
        public string MaHoSo { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime NgayYeuCauLapCT { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime NgayLapChungTu { get; set; }
        public string SoChungTuERP { get; set; }
        public string SoTaiKhoanChuyen { get; set; }
        public string MaNganHangNhan { get; set; }
        public string TenNganHangChuyen { get; set; }
        public string TenTaiKhoanChuyen { get; set; }
        public string MaChiNhanhChuyen { get; set; }
        public string MaChiNhanhNhan { get; set; }
        //public NganHang NganHangChuyen { get; set; }
        //public NganHang NganHangNhan { get; set; }
        public string NoiDungTT { get; set; }
        public string LoaiPhi { get; set; }
        public string LoaiGiaoDich { get; set; }
        public decimal SoTien { get; set; }
        public string LoaiTienTe { get; set; }
        public decimal TyGia { get; set; }
        public string TenNguoiChuyen { get; set; }
        public string DiaChiChuyen { get; set; }
        public string DiaChiNhan { get; set; }
        public string MaTinhTPChuyen { get; set; }
        public string MaTinhTPNhan { get; set; }
        public string NguoiYeuCauLapId { get; set; }
        public string NguoiDuyetId { get; set; }
        public DateTime NgayDuyet { get; set; }
        public int DonViThanhToanId { get; set; }
        public string TenNguoiThanhToan { get; set; }
        public int? TrangThaiCT { get; set; }
        public Guid KySoId { get; set; }
        public DateTime NgayKy { get; set; }
        public int CapKy { get; set; }
        public string MaDonViERP { get; set; }
        public bool CTGS_GL { get; set; }
        public bool CTGS_AP { get; set; }
        public bool CTGS_CM { get; set; }
        public string GhiChu { get; set; }
        public bool DaXoa { get; set; }
        public string TenTaiKhoanNhan { get; set; }
        public string SoTaiKhoanNhan { get; set; }
        public string MaNganHangChuyen { get; set; }
        public string MaNuocChuyen { get; set; }
        public string MaNuocNhan { get; set; }
        public string ChungTuERPId { get; set; }
        public DateTime? NgayYeuCauLapCTStart { get; set; }
        public DateTime? NgayYeuCauLapCTEnd { get; set; }
        public string? TuKhoa { get; set; }
        public string UserId { get ;set;}
        public string LoaiGiaoDichCha { get; set; }
        public string TenNganHangNhan { get; set; }
        public List<ChiTietHachToanParams> ChiTietHachToan { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime NgayGiaoDichThucTe { get; set; }
        public string TenLoaiGiaoDich { get; set; }
        public int LoaiChungTu { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public string DonViGhiNhanCongNo { get; set; }
        public string NguoiCapNhatCTGS { get; set; }
    }

    public class ResponseGetChungTuMetaData
    {
        public DonVi BoPhanYeuCau { get; set; }
        public LoaiHoSo LoaiHoSo { get; set; }
    }

    public class ResponseGetChungTu
    {
        public Dictionary<string, object> Data { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public bool Success { get; set; }

        public ResponseGetChungTu(string message, string errorcode, bool success, int totalRecord, object items)
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

    public class KySoChungTuViewModel
    {
        //public Guid KySoId { get; set; }
        //public ChungTu ChungTu { get; set; }
        //public string NoiDungKy { get; set; }
        //public DateTime? NgayKy { get; set; }
        //public ApplicationUser NguoiKy { get; set; }
        //public string DuLieuKy { get; set; }
        //public int CapKy { get; set; }
        //public bool DaKy { get; set; }
        //public bool DaXoa { get; set; }
        //public string TenHoSoTT { get; set; }
        //public int HinhThucTT { get; set; }
        //public string MaHoSo { get; set; }
        public Guid KySoId { get; set; }
        public ChungTu ChungTu { get; set; }
        public string NoiDungKy { get; set; }
        public DateTime? NgayKy { get; set; }
        public ApplicationUser NguoiKy { get; set; }
        public string DuLieuKy { get; set; }
        public int CapKy { get; set; }
        public bool DaKy { get; set; }
        public bool DaXoa { get; set; }
        public DateTime NgayTao { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
    }

    public class KySoChungTuParams : PaginationViewModel
    {
        public Guid KySoId { get; set; }
        public Guid? ChungTuId { get; set; }
        public string NoiDungKy { get; set; }
        public DateTime? NgayKy { get; set; }
        public string NguoiKyId { get; set; }
        public string DuLieuKy { get; set; }
        public int? CapKy { get; set; }
        public bool? DaKy { get; set; }
        public DateTime? NgayYeuCauLapCTStart { get; set; }
        public DateTime? NgayYeuCauLapCTEnd { get; set; }
        public int? TrangThaiCT { get; set; }
        public string? TuKhoa { get; set; }
        public bool? DaXoa { get; set; }
        public Guid HoSoId { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public int LoaiChungTu { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid BuocPheDuyetId { get; set; }
    }

    public class ResponseGetKySoChungTu
    {
        public Dictionary<string, object> Data { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
        public ResponseGetKySoChungTu(string message, string errorcode, bool success, int totalRecord, object items)
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

    public class ResponseTrangThaiChiTien
    {
        public Dictionary<string, object> Data { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
        public ResponseTrangThaiChiTien(string message, string errorcode, bool success, Dictionary<string, object> data)
        {
            Data = data;
            Message = message;
            ErrorCode = errorcode;
            Success = success;
        }
    }
}
