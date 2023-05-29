using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BCXN.Models;

namespace Epayment.Models
{
    public class GiayTo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid GiayToId { get; set; }
        [StringLength(255)]
        public string TenGiayTo { get; set; }
        public string MaGiayTo { get; set; }
        public int KySo { get; set; }
        public string Nguon { get; set; }
        
        public int TrangThai { get; set; }
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime NgayTao { get; set; }
        public string NguoiTaoId { get; set; }
    }
}