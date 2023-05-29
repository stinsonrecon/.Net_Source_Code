using System;

namespace Epayment.ModelRequest
{
    public class PhanCongXuLy{
        public Guid HoSoId { get; set; }
        public string NguoiPhanCongId { get; set; }
        public string NguoiDuocPhanCongId { get; set; }
        public string ThaoTacBuocPheDuyetId { get; set; }
    }
}