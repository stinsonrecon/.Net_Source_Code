using System;

namespace Epayment.ViewModels
{
    public class CreateQuaTrinhPheDuyet
    {
        public Guid BuocPheDuyetId { get; set; }
        public Guid HoSoId { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int TrangThaiXuLy { get; set; }
        public DateTime ThoiGianXuLy { get; set; }
        public Guid NguoiXuLyId { get; set; }
    }

    public class QuaTrinhPheDuyetViewModel
    {

    }
}
