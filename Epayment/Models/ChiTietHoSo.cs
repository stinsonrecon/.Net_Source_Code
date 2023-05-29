using System.ComponentModel.DataAnnotations;

namespace Epayment.Models
{
    public class ChiTietHoSo
    {
        [Key]
        public int GiayToId {get;set;}
        [StringLength(255)]
        public string  FileDinhKem {get;set;}
    }
}