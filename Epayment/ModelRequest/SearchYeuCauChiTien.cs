using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Epayment.Models;

namespace Epayment.ModelRequest
{
    public class SearchYeuCauChiTien
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime ThoiGianChiTu { get; set; }

        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime ThoiGianChiDen { get; set; }

        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayYeuCauChiTu { get; set; }

        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayYeuCauChiDen { get; set; }
        public string NganHangThuHuongId { get; set; }
        public string NganHangChiId { get; set; }
        public string TuKhoa { get; set; }
        public int TrangThaiChi { get; set; }
        [Required]
        public int PageIndex { get; set; }
        [Required]
        public int PageSize { get; set; }

    }
}