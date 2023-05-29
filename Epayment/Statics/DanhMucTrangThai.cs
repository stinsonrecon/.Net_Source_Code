using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Statics
{    
    public class TrangThaiChungTu
    {
        public const int DaHuy = -1;
        public const int KhoiTao = 0;
        public const int ERPXacNhan = 1;
        public const int KeToanNganHangDaKy = 2;
        public const int LanhDaoBanTaiChinhKeToanDaKy = 3;
        public const int ThuTruongCoQuanDaKy = 4;
        public const int DaGuiLenhChuyenTien = 5;
        public const int DaChuyenTien = 6;
        public const int DongHoSo = 7;
        public const int KhoiTaoCTGS = 8; // khởi tạo chứng từ ghi sổ, chưa đẩy sang epayment
        public const int ERPXacNhanCTGS = 9; // ERP đã xác nhận và cập nhật kết quả của chứng từ ghi sổ
        public const int DaHuyCTGS = 10; // chứng từ ghi sổ đã bị hủy
        public const int KeToanThanhToanDaKy = 11; // kế toán thanh toán đã ký chứng từ ghi sổ
        public const int LanhDaoTaiChinhKeToan = 12; // lãnh đạo phòng tài chính kế toán đã ký chứng từ ghi sổ
    }
    public class TrangThaiChiTien
    {
        public const int LoiDayFile = -5;
        public const int LoiTaoFile = -4;
        public const int NganHangXuLyLoi = -3;
        public const int KhongGuiDuocLenhChi = -2;
        public const int LenhChiTienLoiThamSo = -1;
        public const int LoiChuaXacDinh = 0;
        public const int GuiLenhChiTienThanhCong = 1;
        public const int DaChuyenTien = 2;
        public const int DaCapNhatKetQuaERP = 3;
    }
    public class TrangThaiHoSo {
        public const int XoaHoSo = -1;
        public const int ChuaPheDuyet = 1;
        public const int DaPheDuyet = 2;
        public const int YeuCauThaiDoi = 3;
        public const int DaThanhToan = 4;
        public const int KhoiTao = 5;
        public const int DaTaoChungTu = 6;
        public const int DongHoSo = 7;
        public const int TrinhKy = 8;
        public const int DaTaoChungTuGhiSo = 9;
        public const int TrinhKyCTGS = 10;
        public const int ThamTraHoSo = 11;
        public const int ChuaTiepNhan = 12;
        public const int KyPhieuThamTraHoSo = 13;
    }
    public class TrangThaiPheDuyetHoSo {
        public const int PheDuyet = 2;
        public const int TuChoi = 3;
        public const int DongHoSo = 4;
        public const int PheDuyetToTrinh = 5;
        public const int TuChoiPheDuyetToTrinh = 6;
        public const int ThamTraHoSo = 7;
        public const int TuChoiThamTraHoSo = 8;
        public const int TiepNhanHoSo = 9;
        public const int TuChoiPhanCongNguoiPheDuyet = 10;
        public const int TuChoiTiepNhanHoSo = 11;
        public const int PhanCongNguoiPheDuyet = 12;
    }
    public class TrangThaiChiTietGiayToThamTra {
        public const int TrinhKyCap1 = 2;
        public const int TrinhKyCap2 = 3;

    }
}
