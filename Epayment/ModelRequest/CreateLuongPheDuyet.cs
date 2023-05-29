namespace Epayment.ModelRequest
{
    public class CreateLuongPheDuyet
    {
        public string BuocThucHienId { get; set; }
        // public DateTime ThoiGianGiao { get; set; }
        public string VaiTroId { get; set; }
        public string TenVaiTro { get; set; }
        public string TrangThaiHoSoId { get; set; }
        // public DateTime ThoiGianTiepNhan { get; set; }
        // public string GhiChu { get; set; }
        public string LoaiHoSoId { get; set; }
        public string TenBuoc { get; set; }
        public int ChuyenSangERP { get; set; }
        public int CoThamTra { get; set; }
        public int ThuTu { get; set; }
    }
}