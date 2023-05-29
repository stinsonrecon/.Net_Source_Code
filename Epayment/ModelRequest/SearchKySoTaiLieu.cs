using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Epayment.Models;

namespace Epayment.ModelRequest
{
    public class SearchKySoTaiLieu
    {
        public string TuKhoa { get; set; }
        public int DaKy { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime TuNgay { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime DenNgay { get; set; }
        [Required]
        public int CapKy { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int CapKyTrongHoSo { get; set; }
    }
}