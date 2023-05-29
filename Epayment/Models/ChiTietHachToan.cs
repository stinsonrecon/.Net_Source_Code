using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Models
{
    public class ChiTietHachToan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ChiTietHachToanId { get; set; }
        public string TKNo { get; set; }
        public string TKCo { get; set; }
        public decimal SoTien { get; set; }
        public string DienGiai { get; set; }
        public ChungTu ChungTu { get; set; }
    }
}
