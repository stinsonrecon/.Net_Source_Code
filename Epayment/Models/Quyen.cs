using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class Quyen
    {
        [Key]
        public int Id { get; set; }
        public string TenQuyen { get; set; }
        public string MoTa { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
}
