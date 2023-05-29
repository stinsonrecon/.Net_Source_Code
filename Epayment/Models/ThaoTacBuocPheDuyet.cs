using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class ThaoTacBuocPheDuyet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        [Required]
        public Guid BuocPheDuyetId { get; set; }
        public BuocPheDuyet BuocPheDuyet { get; set; }
        [Required]
        public string HanhDong { get; set; }
        public bool KySo { get; set; }
        public int LoaiKy { get; set; }
        // [Required]
        public Guid? GiayToId { get; set; }
        public GiayTo GiayTo { get; set; }
        public string TrangThaiHoSo { get; set; }
        public string TrangThaiChungTu { get; set; }
        public bool IsSendMail { get; set; }
        // đi đến bước phê duyệt 
        public Guid? DiDenBuocPheDuyetId { get; set; }
        public BuocPheDuyet DiDenBuocPheDuyet { get; set; }
    }
}
