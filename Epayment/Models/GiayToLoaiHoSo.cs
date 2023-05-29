using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class GiayToLoaiHoSo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid GiayToLoaiHoSoId { get; set; }
        public GiayTo GiayTo {get;set;}
        public LoaiHoSo LoaiHoSo {get;set;}
        public DateTime NgayTao {get;set;}
        public int BatBuoc { get; set; }
        public string Nguon { get; set; }
        public int PheDuyetKySo { get; set; }
        public string MoTa { get; set; }
        public int ThuTu { get; set; }
    }
}