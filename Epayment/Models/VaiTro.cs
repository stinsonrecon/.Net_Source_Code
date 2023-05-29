using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class VaiTro
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid VaiTroId { get; set; }
        public string TenVaiTro { get; set; }
    }
}