using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BCXN.Models;

namespace Epayment.Models
{
    public class LoaiHoSo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid LoaiHoSoId{get;set;}
        [StringLength(255)]
        public string TenLoaiHoSo{get;set;}
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayTao {get;set;}
        public string MoTa { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
        public string MaLoaiHoSo { get; set; }
    }
}