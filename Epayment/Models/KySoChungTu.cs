using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCXN.Models;

namespace Epayment.Models
{
    public class KySoChungTu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid KySoId{get;set;}
        public ChungTu ChungTu { get; set; }
        public string NoiDungKy { get; set; }
        public DateTime? NgayKy { get; set; }
        public ApplicationUser NguoiKy { get; set; }
        public string DuLieuKy { get; set; }
        public int CapKy { get; set; }
        public bool DaKy { get; set; }
        public bool DaXoa { get; set; }
        public DateTime NgayTao { get; set; }
        public string TaiLieuGoc { get; set; }
        public string TaiLieuKy { get; set; }
        public string NguoiKyId { get; set; }
    }
}