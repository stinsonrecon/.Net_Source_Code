using System.Collections.Generic;

namespace Epayment.ModelRequest
{
    public class CreateLoaiHoSo
    {
        public string LoaiHoSoId{get;set;}
        
        public string TenLoaiHoSo{get;set;}
        public string Mota { get; set; }
        public int TrangThai { get; set; }
        public int DaXoa { get; set; }
        public string MaLoaiHoSo { get; set; }
        // thêm mới luồng phê duyệt của một loại hồ sơ ngay khi thêm mới loại hồ sơ
        public IEnumerable<LuongPheDuyetRequest> LuongPheDuyet { get; set; }
        public IEnumerable<TaiLieuYeuCauRequest> TaiLieuYeuCau { get; set; }
    }
    public class TaiLieuYeuCauRequest
    {
        public string GiayToLoaiHoSoId { get; set; } // dùng trong việc cập nhật
        public string GiayToId { get; set; }
        public string MoTa { get; set; }
        public int BatBuoc { get; set; }
        public string Nguon { get; set; }
        public int PheDuyetKySo { get; set; }
        public int ThuTu { get; set; }
    }
    public class LuongPheDuyetRequest
    {
        public string BuocThucHienId { get; set; } // dùng trong việc cập nhật
        public string VaiTroId { get; set; }
        public string LoaiHoSoId { get; set; }
        public string TrangThaiHoSoId { get; set; }
        public string TenBuoc { get; set; }
        public int ChuyenSangERP { get; set; }
        public int CoThamTra { get; set; }
        public int ThuTu { get; set; }
    }
}