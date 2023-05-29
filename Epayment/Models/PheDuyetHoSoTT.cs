using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class PheDuyetHoSoTT
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid PheDuyetHoSoTTId { get;set;}
        public HoSoThanhToan HoSoThanhToan { get;set;}
        [StringLength(50)]
        public ApplicationUser NguoiThucHien { get;set;}
        public DateTime NgayThucHien{get;set;}
        public string TrangThaiHoSo {get;set;}
        public string TrangThaiPheDuyet {get;set;}
        public string BuocThucHien {get;set;}
        public string NoiDung {get;set;}
    }
}