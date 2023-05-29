using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class HoSoThamChieu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid HoSoThamChieuId { get; set; }
        public Guid HoSoLienQuanId { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
    }
}