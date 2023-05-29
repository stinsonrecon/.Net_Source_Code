using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BCXN.ViewModels;
using Epayment.Models;

namespace Epayment.ViewModels
{
    public class HoSoThanhToanViewModel
    {
        public HoSoThanhToanViewModel()
        {
            HoSoId = Guid.NewGuid();
        }
        public Guid HoSoId{get;set;}
        public string MaHoSo{get;set;}
        public string TenHoSo{get;set;}
        public int NamHoSo { get; set; }
        public int ThoiGianLuTru { get; set; }
        public int MucDoUuTien { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime HanThanhToan { get; set; }
        public string GhiChu { get; set; }
        public Guid LoaiHoSoId{get;set;}
        public int DonViYeuCauId { get; set; }
        public int BoPhanCauId { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayGui { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayTiepNhan { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayDuyet { get; set; }
        public decimal SoTien { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayLapCT { get; set; }
        public int TrangThaiHoSo { get; set; }
        public int BuocThucHien { get; set; }
        // [DataType(DataType.Date)]
        // [JsonConverter(typeof(JsonDateConverter))]
        // public DateTime NgayTao { get; set; }
        public string NguoiThuHuong { get; set; }
        public decimal SoTienThucTe { get; set; }
        public string SoTKThuHuong { get; set; }
        public Guid NganHangId { get; set; }
        public int HinhThucTT { get; set; }
        public int HinhThucChi {get;set;}
        
    }
    // public class ParmHoSoThanhToanViewModel :HoSoThanhToanViewModel{
    //     public List<NguoiHuongThuViewModel> Data {get;set;}
    //     public ParmHoSoThanhToanViewModel(List<NguoiHuongThuViewModel> data) : base()
    //     {
    //         Data = data;
    //     }
    // }
    public class ParmHoSoThanhToanViewModel
    {
        public Guid HoSoId{get;set;}
        public string MaHoSo{get;set;}
        public string TenHoSo{get;set;}
        public int NamHoSo { get; set; }
        public int ThoiGianLuTru { get; set; }
        public int MucDoUuTien { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime HanThanhToan { get; set; }
        public string GhiChu { get; set; }
        public Guid LoaiHoSoId{get;set;}
        public int DonViYeuCauId { get; set; }
        public int BoPhanCauId { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayGui { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayTiepNhan { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayDuyet { get; set; }
        public decimal SoTien { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayLapCT { get; set; }
        public int TrangThaiHoSo { get; set; }
        public int BuocThucHien { get; set; }
        // [DataType(DataType.Date)]
        // [JsonConverter(typeof(JsonDateConverter))]
        // public DateTime NgayTao { get; set; }
        // public string NguoiThuHuong { get; set; }
        public decimal SoTienThucTe { get; set; }
        // public string SoTKThuHuong { get; set; }
        // public Guid NganHangId { get; set; }
        public int HinhThucTT { get; set; }
        public int HinhThucChi {get;set;}
        public string IdLogin {get;set;}
        public string TenVietTat { get; set; }
        public int CoHieuLuc { get; set; }
        public int CoBanCung { get; set; }
        public int LanThanhToan { get; set; }
        public string TenYeuCauThanhToan { get; set; }
        public string LoaiTienTe { get; set;}
        public DateTime NgayThanhToan { get; set;}  
        public Guid TaiKhoanNganHangId { get; set; }
        public string NoiDungThanhToan {get ;set;}
        public IEnumerable<string> HoSoLienQuan { get; set; }
        public Guid QuyTrinhPheDuyetId { get; set; }
        public Guid QuaTrinhPheDuyetId { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
    }

}