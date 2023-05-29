using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class ChungTuThanhToan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ChungTuId {get;set;}
        public int SoChungTu {get;set;}
        public HoSoThanhToan HoSoThanhToan { get;set;}
        public decimal SoTien { get; set; }
        public DateTime NgayLapCT{get;set;}
        public int TrangThai {get;set;}
        public int HinhThucChi {get;set;}
        public ApplicationUser NguoiLap {get;set;}
        public ApplicationUser NguoiKy {get;set;}
        public DateTime NgayKy {get;set;}
    }
}