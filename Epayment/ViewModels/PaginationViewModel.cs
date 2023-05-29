using Epayment.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BCXN.ViewModels
{
    public class PaginationViewModel
    {
        [Required]
        public int PageSize {get; set; }
        [Required]
        public int PageIndex { get; set; }
 
    }
    public class ChucNangSearchViewModel : PaginationViewModel
    {
        public string TenCN { get; set; }
        public int? TrangThaiCN { get; set; }
        public int? Type { get; set; }
    }


    public class DonViTinhSearchViewModel : PaginationViewModel
    {
        public string TenDVT { get; set; }
        public int? TrangThai { get; set; }
    }

    public class VatTuSearchViewModel : PaginationViewModel
    {
        public string TenVT { get; set; }
        public int? NhomVatTuId { get; set; }
    }

    public class NhomVatTuSearchViewModel
    {
        public string TenNVT { get; set; }
        public int? TrangThaiNVT { get; set; }
    }
    public class HoSoSearchViewModel : PaginationViewModel
    {
        public int TinhTrangPheDuyet { get; set; }
        public int BoPhanYeuCau { get; set; }
        public string LoaiHoSo { get; set; }
        public string TuKhoa { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime TuNgay { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime DenNgay { get; set; }
        public int DaPheDuyet { get; set; }
        public string UserId { get; set; }
        public int? ThamTraHoSo { get; set; }
    }
    public class LoaiHoSoSearchViewModel : PaginationViewModel
    {
        public int trangThai { get; set; }
        public string TuKhoa { get; set; }
    }
    public class ChungtuSearchViewModel : PaginationViewModel
    {
        public string LoaiHoSoId { get; set; }
        public int tinhTrang { get; set; }
        public string TuKhoa { get; set; }
    }
    public class ChiTietHSTTSearchViewModel: PaginationViewModel{
        public Guid idHSTT {get;set;}
    }
    public class HoSoThamChieuSearchViewModel : PaginationViewModel
    {
        //public int TinhTrangPheDuyet { get; set; }
        //public int BoPhanYeuCau { get; set; }
        //public string LoaiHoSo { get; set; }
        public string TuKhoa { get; set; }
        // [DataType(DataType.Date)]
        // [JsonConverter(typeof(JsonDateConverter))]
        // public DateTime TuNgay { get; set; }
        // [DataType(DataType.Date)]
        // [JsonConverter(typeof(JsonDateConverter))]
        // public DateTime DenNgay { get; set; }
        //public int DaPheDuyet { get; set; }
        public string LoaiHoSo { get; set; }
        public string TrangThaiHoSo { get; set; }
    }
    
}


