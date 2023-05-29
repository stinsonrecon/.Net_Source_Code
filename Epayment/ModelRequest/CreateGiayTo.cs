using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Epayment.Models;

namespace Epayment.ModelRequest
{
    public class CreateGiayTo
    {
        public Guid GiayToId {get;set;}
        public string TenGiayTo {get;set;}
        public string MaGiayTo{get;set;}
        public  int KySo {get;set;}
        public string Nguon{get;set;}
        public int TrangThai{get;set;}
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayTao { get; set; }
        public string NguoiTaoId { get; set; }
    }
}