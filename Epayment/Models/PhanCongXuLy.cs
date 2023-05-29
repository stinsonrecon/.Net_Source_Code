using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class PhanCongXuLy
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid PhanCongXuLyId {get;set;}
        public HoSoThanhToan HoSoThanhToan { get;set;}
        public ApplicationUser NguoiXuLy { get;set;}
        public DateTime ThoiGianGiao{get;set;}
        public int TrangThaiTiepNhan {get;set;}
        [StringLength(500)]
        public string GhiChu {get;set;}
    }
}