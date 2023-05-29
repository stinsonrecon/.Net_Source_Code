using BCXN.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Models
{
    public class KySoTaiLieu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid KySoTaiLieuId { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }
        public string NoiDungKy { get; set; }
        public DateTime? NgayKy { get; set; }
        public DateTime NgayTao { get; set; }
        public int CapKy { get; set; }
        public ApplicationUser NguoiKy { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public bool DaXoa { get; set; }
        public bool DaKy { get; set; }
    }
}
