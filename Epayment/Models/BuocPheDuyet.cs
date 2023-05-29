using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epayment.Models
{
    public class BuocPheDuyet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid BuocPheDuyetId { get; set; }
        //public Guid BuocThucHienId { get; set; }
        [Required]
        public Guid QuyTrinhPheDuyetId { get; set; }
        public QuyTrinhPheDuyet QuyTrinhPheDuyet { get; set; }
        public Guid? BuocPheDuyetTruocId { get; set; }
        public Guid? BuocPheDuyetSauId { get; set; }
        [Required]
        public string TrangThaiHoSo { get; set; }
        [Required]
        public string TrangThaiChungTu { get; set; }
        [Required]
        public string TenBuoc { get; set; }
        public int ThuTu { get; set; }
        public bool DaXoa { get; set; }
        [Required]
        public string NguoiThucHien { get; set; }
        public int ThoiGianXuLy { get; set; }
        public string DinhDangKy { get; set; }
        public string ViTriKy { get; set; }
    }
}
