using System;

namespace Epayment.ViewModels
{
    public class GiayToLoaiHoSoViewModel
    {
        public Guid giayToLoaiHoSoId { get; set; }
        public string giayTo {get;set;}
        public Guid giayToId {get;set;}
        public string loaiHoSo { get; set; }
        public Guid loaiHoSoId { get; set; }
        public DateTime ngayTao {get;set;}
    }
}