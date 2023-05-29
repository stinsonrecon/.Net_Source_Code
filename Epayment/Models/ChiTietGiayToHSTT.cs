using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class ChiTietGiayToHSTT
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ChiTietHoSoId { get; set; }
        public GiayTo GiayTo { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
        public string  FileDinhKem {get;set;}
        public int TrangThaiGiayTo { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public ApplicationUser NguoiCapNhat { get; set; }
    }
}