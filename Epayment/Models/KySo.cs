using System;
using System.ComponentModel.DataAnnotations;

namespace Epayment.Models
{
    public class KySo
    {
        [Key]
        public int KySoId{get;set;}
        public int ChungTuId {get;set;}
        public int SoPCUNC {get;set;}
        public int TKNo {get;set;}
        public int TKCo {get;set;}
        public DateTime NgayLap {get;set;}
        [StringLength(500)]
        public string NoiDung {get;set;}
        public DateTime NgayKy {get;set;}
        public string NguoiKy{get;set;}
        public string PhieuChiXML {get;set;}
        public string ChuKyXML {get;set;}
        public string CapKy {get;set;}
    }
}