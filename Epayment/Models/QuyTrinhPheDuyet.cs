using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class QuyTrinhPheDuyet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid QuyTrinhPheDuyetId { get; set; }
        [Required]
        public Guid LoaiHoSoId { get; set; }
        public LoaiHoSo LoaiHoSo { get; set; }
        [Required]
        public string TenQuyTrinh { get; set; }
        [Required]
        public DateTime NgayHieuLuc { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public bool DaXoa { get; set; }
    }
}
