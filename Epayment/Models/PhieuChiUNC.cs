using System;
using System.ComponentModel.DataAnnotations;

namespace Epayment.Models
{
    public class PhieuChiUNC
    {
        [Key]
        public int PhieuChiId {get;set;}
        public int ChungTuId {get;set;}
        public int SoPCUNC {get;set;}
        public int TKNo {get;set;}
        public int TKCo {get;set;}
        [StringLength(50)]
        public string MaNganHang {get;set;}
        [StringLength(500)]
        public string NoiDung {get;set;}
        public decimal SoTien {get;set;}
        public DateTime NgayLap{get;set;}
        public int NguoiLapId {get;set;}
        public int NguoiDuyetId{get;set;}
        public DateTime NgayDuyet {get;set;}
        public int TrangThai {get;set;}
        public string FileKy {get;set;}

    }
}