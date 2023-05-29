namespace Epayment.ModelRequest {
    public class CreateGiayToLoaiHS {
        public string LoaiHoSoId { get; set; }
        public string GiayToId { get; set; }
        public int KySo { get; set; }
        public string GiayToLoaiHoSoId { get; set; }
        public int BatBuoc { get; set; }
        public string Nguon { get; set; }
        public int PheDuyetKySo { get; set; }
    }
}