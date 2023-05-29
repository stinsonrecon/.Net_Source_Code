using System;
using System.ComponentModel.DataAnnotations;

namespace Epayment.Models
{
    public class TrangThaiHoSo
    {
        [Key]
        public Guid TrangThaiHoSoId { get; set; }
        public string TenTrangThaiHoSo { get; set; }
        public int GiaTri { get; set; }
        public int TrangThai { get; set; }
    }
}
