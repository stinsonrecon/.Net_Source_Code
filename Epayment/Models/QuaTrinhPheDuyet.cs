using BCXN.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class QuaTrinhPheDuyet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid QuaTrinhPheDuyetId { get; set; }
        
        [Required]
        public Guid BuocPheDuyetId { get; set; }
        public BuocPheDuyet BuocPheDuyet { get; set; }
        
        [Required]
        public Guid HoSoId { get; set; }
        public HoSoThanhToan HoSoThanhToan { get; set; }

        public DateTime ThoiGianTao { get; set; }
        public int TrangThaiXuLy { get; set; }
        [Required]
        public DateTime ThoiGianXuLy { get; set; }

        [Required]
        public Guid NguoiXuLyId { get; set; }
        public ApplicationUser NguoiXuLy { get; set; }

        public bool DaXoa { get; set; }
    }
}
