using System;
using System.ComponentModel.DataAnnotations;

namespace Epayment.ModelRequest{
    public class ApproveHoSoTT{
        [Required]
        public string HoSoId { get; set; }
        [Required]
        public string NguoiThucHienId { get; set; }
        public string BuocThucHien { get; set; }
        public int TrangThaiHoSo { get; set; }
        public int TrangThaiPheDuyet { get; set; }
        public string NoiDung { get; set; }
        public string NguoiThamTraId { get; set; }
        public Guid ThaoTacBuocPheDuyetId { get; set; }
        public Guid BuocPheDuyetId { get; set; }
        public string NguoiPheDuyetHoSoId { get; set; }
    }
}