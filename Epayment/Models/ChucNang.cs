using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class ChucNang
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string TieuDe { get; set; }
        public string ClaimValue { get; set; }
        public int ChucNangChaId { get; set; }
        [StringLength(500)]
        public string MoTa { get; set; }
        public int TrangThai { get; set; }
        [StringLength(255)]
        public string LinkUrl { get; set; }
        [StringLength(50)]
        public string Icon { get; set; }

        public int? Order { get; set; }
        public int? Type { get; set; }
    }
}
