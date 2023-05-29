using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Models
{
    public class ChucNangNhom
    {
        [Key]
        public int Id { get; set; }
        public int ChucNangId { get; set; }
        public string NhomQuyenId { get; set; }
    }
}
