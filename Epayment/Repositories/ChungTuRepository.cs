using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Data;
using Microsoft.Extensions.Logging;
using Epayment.ViewModels;
using BCXN.ViewModels;
using Epayment.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using BCXN.Statics;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Epayment.Repositories
{
    public interface IChungTuRepository
    {
        ResponseGetChungTu GetChungTu(ChungTuParams chungtu);
        Response ERPUpdateChungTu(ChungTuParams chungtu);
        Response ERPUpdateChungTuGhiSo(ChungTuParams ctgs);
        Response CreateChungTu(ChungTuParams chungtu);
        Response DeleteChungTu(ChungTuParams chungtu);
        Response KyChungTu(KySoChungTuParams kysochungtu);
        ResponseGetKySoChungTu GetKySoChungTu(KySoChungTuParams kysochungtu);
        ChungTu GetChungTuByID(Guid id);
        ResponseTrangThaiChiTien GetKySoChungTuByERPChungTuId(string ChungTuERPId);
        Response CancelChungTu(string NguoiKyId, KySoChungTuParams kysochungtu);
        ResponseGetKySoChungTu GetKySoChungTuChoBanTheHien(KySoChungTuParams kysochungtu);
        ChungTu GetChungTuByHoSoThanhToanId(Guid hsttid);
        ChungTu GetChungTuByYeuCauChi(Guid yeucauchiID);
        Response UpdateTrangThaiDongCT(Guid ChungTuId, string UserId, Guid thaoTacBuocPheDuyetId);
        bool DaChiTien(ChungTu chungtuItem);
        ResponseGetChungTuMetaData GetMetaDataByChungTuId(Guid id);
        Response UpdateTaiLieuChungtu(Guid chungtuId, string tailieugoc = null, string tailieuky = null);
        Response GetPCUNC(ChungTuParams chungtu);
        Response HotFixKyChungTuCap1(KySoChungTuParams kysochungtu);
        Response GetCTGS(ChungTuParams chungtu);
    }

    public class ChungTuRepository : IChungTuRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChungTuRepository> _logger;
        private readonly IConfiguration _configuration;

        public ChungTuRepository(ApplicationDbContext context, ILogger<ChungTuRepository> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public ChungTu GetChungTuByID(Guid id)
        {
            var getHoSoTT = from ct in _context.ChungTu
                            join hstt in _context.HoSoThanhToan on ct.HoSoThanhToan.HoSoId equals hstt.HoSoId
                            join dv in _context.DonVi on ct.DonViThanhToan.Id equals dv.Id
                            where ct.ChungTuId == id
                            select hstt;
            var getDonVi = from ct in _context.ChungTu
                           join dv in _context.DonVi on ct.DonViThanhToan.Id equals dv.Id
                           where ct.ChungTuId == id
                           select dv;
            var response = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == id));
            response.HoSoThanhToan = getHoSoTT.ToList()[0];
            response.DonViThanhToan = getDonVi.ToList()[0];
            return response;
        }

        public ResponseGetChungTuMetaData GetMetaDataByChungTuId(Guid id)
        {
            var resp = (from ct in _context.ChungTu
                            join hstt in _context.HoSoThanhToan on ct.HoSoThanhToan equals hstt
                            join lhs in _context.LoaiHoSo on hstt.LoaiHoSo equals lhs
                            join dv in _context.DonVi on hstt.BoPhanCauId equals dv
                            where ct.ChungTuId == id
                            select new ResponseGetChungTuMetaData
                            { 
                                LoaiHoSo = lhs,
                                BoPhanYeuCau = dv
                            }).FirstOrDefault();
            return resp;
        }

        public Response CreateChungTu(ChungTuParams chungtu)
        {
            try
            {
                if (chungtu.SoTien <= 0) return new Response(message: "Lỗi số tiền âm", data: "002", errorcode: "", success: false);
                //var taiKhoan = (from tk in _context.TaiKhoanNganHang
                //               join hstt in _context.HoSoThanhToan on tk.TaiKhoanId equals hstt.TaiKhoanThuHuong.TaiKhoanId
                //               join nh in _context.NganHang on tk.NganHang.NganHangId equals nh.NganHangId
                //               join cn in _context.ChiNhanhNganHang on tk.ChiNhanhNganHang.ChiNhanhNganHangId equals cn.ChiNhanhNganHangId where hstt.HoSoId == chungtu.HoSoThanhToanId
                //               select new ThongTinTaiKhoanByHoSoIdViewModel
                //               {
                //                   TenTaiKhoan = tk.TenTaiKhoan,
                //                   SoTaiKhoan = tk.SoTaiKhoan,
                //                   MaNganHang = nh.MaNganHang,
                //                   TenNganHang = nh.TenNganHang,
                //                   MaChiNhanh = cn.MaChiNhanhErp
                //               }).FirstOrDefault();
                var hosottItem = _context.HoSoThanhToan.FirstOrDefault(x => (x.HoSoId == chungtu.HoSoThanhToanId));
                //thông tin tài khoản, ngân hàng, chi nhánh lấy từ hồ sơ ra
                // var chungtuItem = new ChungTu
                // {
                //     CTGS_GL = chungtu.CTGS_GL,
                //     CTGS_AP = chungtu.CTGS_AP,
                //     CTGS_CM = true,
                //     HoSoThanhToan = hosottItem,
                //     TenTaiKhoanNhan = taiKhoan.TenTaiKhoan,
                //     SoTaiKhoanNhan = taiKhoan.SoTaiKhoan,
                //     MaNganHangNhan = taiKhoan.MaNganHang,
                //     MaChiNhanhNhan = taiKhoan.MaChiNhanh,
                //     TenNganHangNhan = taiKhoan.TenNganHang,
                //     GhiChu = hosottItem.GhiChu,
                //     DonViThanhToan = _context.DonVi.FirstOrDefault(x => (x.Id == chungtu.DonViThanhToanId)),
                //     SoTien = hosottItem.SoTien,
                //     LoaiTienTe = hosottItem.LoaiTienTe,
                //     TenNguoiChuyen = chungtu.TenNguoiChuyen,
                //     TyGia = chungtu.TyGia,
                //     NguoiYeuCauLap = _context.ApplicationUser.FirstOrDefault(x => (x.Id == chungtu.NguoiYeuCauLapId)),
                //     NoiDungTT = chungtu.NoiDungTT,
                //     NgayYeuCauLapCT = DateTime.Now,
                //     NgayLapChungTu = DateTime.Now,
                //     TrangThaiCT = 0,
                //     CapKy = 1
                // };

                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hosottItem.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hosottItem.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hosottItem.QuaTrinhPheDuyetId).ToList();
                var thuTu = 0;
                BuocPheDuyet buocHienTai = new BuocPheDuyet();
                if (listBuocPD.Count != 0 && buocPD.Count != 0)
                {
                    thuTu = buocPD[0].ThuTu;
                    foreach (var item in listBuocPD)
                    {
                        if ((item.ThuTu == buocPD[0].ThuTu + 1))
                        {
                            buocHienTai = item;
                        }
                    }
                QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                {
                    BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                    HoSoId = hosottItem.HoSoId,
                    ThoiGianTao = DateTime.Now,
                    TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                    ThoiGianXuLy = DateTime.Now,
                    NguoiXuLyId = new Guid(_context.ApplicationUser.FirstOrDefault(x => (x.Id == chungtu.UserId)).Id),
                };
                hosottItem.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                hosottItem.ThaoTacVuaThucHienId = chungtu.ThaoTacBuocPheDuyetId;
                _context.QuaTrinhPheDuyet.Add(qt);
                }   

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == chungtu.ThaoTacBuocPheDuyetId);

                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {chungtu.ThaoTacBuocPheDuyetId}");
                }

                // thông tin tài khoản, ngân hàng, chi nhánh đẩy trực tiếp từ giao diện xuống

                string NoiDungTTKD = "";
                if (!String.IsNullOrEmpty(chungtu.NoiDungTT))
                    NoiDungTTKD = BCXN.StringUtils.RemoveSign4VietnameseString(chungtu.NoiDungTT).Replace(",", "");
                else
                    NoiDungTTKD = chungtu.NoiDungTT;
                // var trangThaiChungTu = 0;
                // if(chungtu.LoaiChungTu == 0) {
                //     trangThaiChungTu = TrangThaiChungTu.KhoiTao;
                // }
                // else if(chungtu.LoaiChungTu == 1) {
                //     trangThaiChungTu = TrangThaiChungTu.KhoiTaoCTGS;
                // }
                var chungtuItem = new ChungTu
                {
                    CTGS_GL = chungtu.CTGS_GL,
                    CTGS_AP = chungtu.CTGS_AP,
                    // CTGS_CM = true,
                    CTGS_CM = chungtu.CTGS_CM,
                    HoSoThanhToan = hosottItem,
                    TenTaiKhoanNhan = chungtu.TenTaiKhoanNhan,
                    SoTaiKhoanNhan = chungtu.SoTaiKhoanNhan,
                    MaNganHangNhan = chungtu.MaNganHangNhan,
                    MaChiNhanhNhan = chungtu.MaChiNhanhNhan,
                    TenNganHangNhan = chungtu.TenNganHangNhan,
                    GhiChu = hosottItem.GhiChu,
                    DonViThanhToan = _context.DonVi.FirstOrDefault(x => (x.Id == chungtu.DonViThanhToanId)),
                    SoTien = chungtu.SoTien,
                    LoaiTienTe = hosottItem.LoaiTienTe,
                    TenNguoiChuyen = chungtu.TenNguoiChuyen,
                    TyGia = chungtu.TyGia,
                    NguoiYeuCauLap = _context.ApplicationUser.FirstOrDefault(x => (x.Id == chungtu.NguoiYeuCauLapId)),
                    NoiDungTT = NoiDungTTKD,
                    NgayYeuCauLapCT = chungtu.NgayYeuCauLapCT,
                    // TrangThaiCT = TrangThaiChungTu.KhoiTao,
                    // TrangThaiCT = trangThaiChungTu,
                    TrangThaiCT = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiChungTu),
                    CapKy = chungtu.LoaiChungTu == 0 ? 1 : 4,
                    LoaiChungTu = chungtu.LoaiChungTu
                };

                _context.ChungTu.Add(chungtuItem);
                // if(chungtu.LoaiChungTu == 0) {
                //     hosottItem.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.DaTaoChungTu;
                // }
                // else if(chungtu.LoaiChungTu == 1) {
                //     hosottItem.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.DaTaoChungTuGhiSo;
                // }

                hosottItem.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                hosottItem.NoiDungTT = NoiDungTTKD;
                // update lại ngay lập chứng từ 
                hosottItem.NgayLapCT = DateTime.Now;

                // thêm thông tin vào bảng lịch sử(LichSuHoSoTT)
                var hoSoDetailJoin = from hstt in _context.HoSoThanhToan
                                     join lhs in _context.LoaiHoSo on hstt.LoaiHoSo.LoaiHoSoId equals lhs.LoaiHoSoId
                                     join dv in _context.DonVi on hstt.DonViYeuCau.Id equals dv.Id
                                     join nn in _context.NganHang on hstt.NganHang.NganHangId equals nn.NganHangId
                                     where hstt.HoSoId == hosottItem.HoSoId
                                     select new
                                     {
                                         hstt = hstt,
                                         loaiHSId = lhs.LoaiHoSoId,
                                         donViId = dv.Id,
                                         nnId = nn.NganHangId,
                                     };
                var hoSoDetailList = hoSoDetailJoin.ToList();
                LichSuHoSoTT tam = new LichSuHoSoTT() { };
                tam.LichSuId = Guid.NewGuid();
                tam.HoSoThanhToan = hoSoDetailList[0].hstt;
                tam.LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoDetailList[0].loaiHSId);
                tam.MaHoSo = hosottItem.MaHoSo;
                tam.TenHoSo = hosottItem.TenHoSo;
                tam.NamHoSo = hosottItem.NamHoSo;
                tam.ThoiGianLuTru = hosottItem.ThoiGianLuTru;
                tam.MucDoUuTien = hosottItem.MucDoUuTien;
                tam.HanThanhToan = hosottItem.HanThanhToan;
                tam.GhiChu = hosottItem.GhiChu;
                tam.DonViYeuCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.BoPhanCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.NgayGui = hosottItem.NgayGui;
                tam.NgayTiepNhan = hosottItem.NgayTiepNhan;
                tam.NgayDuyet = DateTime.Now;
                tam.SoTien = hosottItem.SoTien;
                tam.NgayLapCT = DateTime.Now;
                // if(chungtu.LoaiChungTu == 0) {
                //     tam.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.DaTaoChungTu;
                // }
                // else if(chungtu.LoaiChungTu == 1) {
                //     tam.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.DaTaoChungTuGhiSo;
                // }

                tam.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                tam.BuocThucHien = hosottItem.BuocThucHien;
                tam.NgayTao = DateTime.Now;
                tam.NguoiThuHuong = hosottItem.NguoiThuHuong;
                tam.SoTienThucTe = hosottItem.SoTienThucTe;
                tam.SoTKThuHuong = hosottItem.SoTKThuHuong;
                tam.NganHangThuHuong = _context.NganHang.FirstOrDefault(x => x.NganHangId == hoSoDetailList[0].nnId).NganHangId;
                tam.HinhThucTT = hosottItem.HinhThucTT;
                tam.ThoiGianCapNhat = DateTime.Now;
                // tam.NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => (x.Id == hosottItem.NguoiTao.Id));
                _context.LichSuHoSoTT.Add(tam);

                _context.SaveChanges();

                // tạo luôn 3 bản ghi chứng từ rỗng
                var newChungTuItem = _context.ChungTu.FirstOrDefault(x => (x == chungtuItem));
                if(chungtu.LoaiChungTu == 0) {
                    for (int i = 1; i <= 3; i++)
                    {
                        var kysochungtuItem = new KySoChungTu
                        {
                            ChungTu = newChungTuItem,
                            CapKy = i,
                            NgayTao = DateTime.Now,
                            DaKy = false
                        };
                        _context.KySoChungTu.Add(kysochungtuItem);
                    }
                }
                else if(chungtu.LoaiChungTu == 1) {
                    for (int i = 4; i <= 5; i++)
                    {
                        var kysochungtuItem = new KySoChungTu
                        {
                            ChungTu = newChungTuItem,
                            CapKy = i,
                            NgayTao = DateTime.Now,
                            DaKy = false
                        };
                        _context.KySoChungTu.Add(kysochungtuItem);
                    }
                }
                _context.SaveChanges();
                return new Response(message: "Tạo mới chứng từ thành công", data: newChungTuItem.ChungTuId.ToString(), errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public bool DaChiTien(ChungTu chungtuItem)
        {
            if (chungtuItem.TrangThaiCT >= TrangThaiChungTu.DaGuiLenhChuyenTien) return true;
            var listycct = (from ycct in _context.YeuCauChiTien where ycct.ChungTu == chungtuItem select new YeuCauChiTien { TrangThaiChi = ycct.TrangThaiChi }).ToList();
            foreach (var yctt in listycct)
            {
                if (yctt.TrangThaiChi >= TrangThaiChiTien.GuiLenhChiTienThanhCong) return true;
            }
            return false;
        }

        public Response GetPCUNC(ChungTuParams chungtu)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == chungtu.ChungTuId));
                if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "001", success: false);

                string tailieugoc = chungtuItem.TaiLieuGoc;
                string tailieuky = chungtuItem.TaiLieuKy;

                if(String.IsNullOrEmpty(tailieuky) && String.IsNullOrEmpty(tailieugoc)) return new Response(message: "Có lỗi trong quá trình lấy pc/unc", data: "", errorcode: "002", success: false);

                if(tailieuky == null) return new Response(message: "", data: tailieugoc, errorcode: "", success: true);
                else return new Response(message: "", data: tailieuky, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
            }
        }

        public Response ERPUpdateChungTuGhiSo(ChungTuParams ctgs)
        {
            try
            {
                _logger.LogWarning($"[ERPUpdateChungTuGhiSo] Tham số truyền vào: {JsonConvert.SerializeObject(ctgs)}");

                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.HoSoThanhToan.MaHoSo == ctgs.MaHoSo && x.LoaiChungTu == 1));
                if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "001", success: false);

                _context.ChiTietHachToan.RemoveRange(_context.ChiTietHachToan.Where(x => (x.ChungTu == chungtuItem)));

                chungtuItem.NgayLapChungTu = ctgs.NgayLapChungTu;
                chungtuItem.ChungTuERPId = ctgs.ChungTuERPId;
                chungtuItem.SoChungTuERP = ctgs.SoChungTuERP;
                chungtuItem.DonViGhiNhanCongNo = ctgs.DonViGhiNhanCongNo;
                chungtuItem.LoaiTienTe = ctgs.LoaiTienTe;
                chungtuItem.NoiDungTT = ctgs.NoiDungTT;
                chungtuItem.TyGia = ctgs.TyGia;
                chungtuItem.SoTien = ctgs.SoTien;
                chungtuItem.NguoiCapNhatCTGS = ctgs.NguoiCapNhatCTGS;
                chungtuItem.TrangThaiCT = TrangThaiChungTu.ERPXacNhanCTGS;

                foreach (ChiTietHachToanParams ctht in ctgs.ChiTietHachToan)
                {
                    var chitiethachtoanItem = new ChiTietHachToan
                    {
                        TKNo = ctht.TKNo,
                        TKCo = ctht.TKCo,
                        DienGiai = ctht.DienGiai,
                        SoTien = ctht.SoTien,
                        ChungTu = chungtuItem
                    };
                    _context.ChiTietHachToan.Add(chitiethachtoanItem);
                }

                _context.SaveChanges();
                return new Response(message: "Cập nhật chứng từ thành công", data: chungtuItem.ChungTuId, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
            }
        }

        public Response ERPUpdateChungTu(ChungTuParams chungtu)
        {
            _logger.LogWarning($"[ERPUpdateChungTu] Tham số truyền vào: {JsonConvert.SerializeObject(chungtu)}");
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.HoSoThanhToan.MaHoSo == chungtu.MaHoSo && x.LoaiChungTu == 0 && x.DaXoa == false));
                    if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "001", success: false);

                    if ((chungtuItem.TrangThaiCT != TrangThaiChungTu.KhoiTao) && chungtuItem.TrangThaiCT != TrangThaiChungTu.KhoiTaoCTGS) return new Response(message: "Chứng từ đã được cập nhật trước đó", data: "", errorcode: "003", success: false);

                    _context.ChiTietHachToan.RemoveRange(_context.ChiTietHachToan.Where(x => (x.ChungTu == chungtuItem)));

                    chungtuItem.ChungTuERPId = chungtu.ChungTuERPId;
                    chungtuItem.SoChungTuERP = chungtu.SoChungTuERP;
                    chungtuItem.LoaiPhi = chungtu.LoaiPhi;
                    chungtuItem.LoaiGiaoDich = chungtu.LoaiGiaoDich;
                    chungtuItem.TenNganHangChuyen = chungtu.TenNganHangChuyen;
                    chungtuItem.MaNganHangChuyen = chungtu.MaNganHangChuyen;
                    chungtuItem.MaChiNhanhChuyen = chungtu.MaChiNhanhChuyen;
                    chungtuItem.SoTaiKhoanChuyen = chungtu.SoTaiKhoanChuyen;
                    chungtuItem.TenTaiKhoanChuyen = chungtu.TenTaiKhoanChuyen;
                    chungtuItem.DiaChiChuyen = chungtu.DiaChiChuyen;
                    chungtuItem.MaTinhTPChuyen = chungtu.MaTinhTPChuyen;
                    chungtuItem.MaTinhTPNhan = chungtu.MaTinhTPNhan;
                    chungtuItem.MaNuocChuyen = chungtu.MaNuocChuyen;
                    chungtuItem.MaNuocNhan = chungtu.MaNuocNhan;
                    if (!String.IsNullOrEmpty(chungtu.NoiDungTT))
                        chungtuItem.NoiDungTT = BCXN.StringUtils.RemoveSign4VietnameseString(chungtu.NoiDungTT).Replace(",", "");
                    else
                        chungtuItem.NoiDungTT = chungtu.NoiDungTT;
                    chungtuItem.NgayLapChungTu = chungtu.NgayLapChungTu;
                    chungtuItem.SoTien = chungtu.SoTien;
                    chungtuItem.TyGia = chungtu.TyGia;
                    chungtuItem.GhiChu = chungtu.GhiChu;
                    chungtuItem.NgayGiaoDichThucTe = chungtu.NgayLapChungTu;
                    //chungtuItem.NgayGiaoDichThucTe = chungtu.NgayGiaoDichThucTe;
                    if(chungtu.LoaiChungTu == 0) {
                        chungtuItem.TrangThaiCT = TrangThaiChungTu.ERPXacNhan;
                    }
                    else if(chungtu.LoaiChungTu == 1) {
                        chungtuItem.TrangThaiCT = TrangThaiChungTu.ERPXacNhanCTGS;
                    }

                    if(chungtu.LoaiChungTu == 0) {
                        chungtuItem.CapKy = 1;
                    }
                    else if(chungtu.LoaiChungTu == 1) {
                        chungtuItem.CapKy = 4;
                    }
                    // chungtuItem.CapKy = 1;
                    chungtuItem.LoaiGiaoDichCha = chungtu.LoaiGiaoDichCha;
                    chungtuItem.NgayDuyet = DateTime.Now;
                    chungtuItem.DiaChiChuyen = chungtu.DiaChiChuyen;
                    chungtuItem.DiaChiNhan = chungtu.DiaChiNhan;
                    chungtuItem.TenLoaiGiaoDich = chungtu.TenLoaiGiaoDich;

                    if (chungtu.TenNganHangNhan != null && chungtu.TenNganHangNhan != "") chungtuItem.TenNganHangNhan = chungtu.TenNganHangNhan;
                    if (chungtu.MaChiNhanhNhan != null && chungtu.MaChiNhanhNhan != "") chungtuItem.MaChiNhanhNhan = chungtu.MaChiNhanhNhan;
                    if (chungtu.MaNganHangNhan != null && chungtu.MaNganHangNhan != "") chungtuItem.MaNganHangNhan = chungtu.MaNganHangNhan;
                    if (chungtu.TenTaiKhoanNhan != null && chungtu.TenTaiKhoanNhan != "") chungtuItem.TenTaiKhoanNhan = chungtu.TenTaiKhoanNhan;
                    if (chungtu.SoTaiKhoanNhan != null && chungtu.SoTaiKhoanNhan != "") chungtuItem.SoTaiKhoanNhan = chungtu.SoTaiKhoanNhan;                                       

                    foreach (ChiTietHachToanParams ctht in chungtu.ChiTietHachToan)
                    {
                        var chitiethachtoanItem = new ChiTietHachToan
                        {
                            TKNo = ctht.TKNo,
                            DienGiai = ctht.DienGiai,
                            TKCo = ctht.TKCo,
                            SoTien = ctht.SoTien,
                            ChungTu = chungtuItem
                        };
                        _context.ChiTietHachToan.Add(chitiethachtoanItem);
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return new Response(message: "Cập nhật chứng từ thành công", data: chungtuItem.ChungTuId, errorcode: "", success: true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", ex);
                    return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
                }
            }
        }

        public Response UpdateTaiLieuChungtu(Guid chungtuId, string tailieugoc=null, string tailieuky=null)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == chungtuId));
                if (chungtuItem == null) return new Response(message: "Lỗi không tìm thấy chứng từ", data: "", errorcode: "001", success: false);
                if(chungtuItem.LoaiChungTu == 1 && chungtuItem.CapKy == 6) {
                    chungtuItem.TaiLieuGoc = tailieugoc;
                    chungtuItem.TaiLieuKy = tailieuky;
                }
                else if(chungtuItem.LoaiChungTu == 1 && (chungtuItem.CapKy == 4 || chungtuItem.CapKy == 5)) {
                    chungtuItem.TaiLieuGocCTGS = tailieugoc;
                    chungtuItem.TaiLieuKyCTGS = tailieuky;
                }
                else if(chungtuItem.LoaiChungTu == 0) {
                    chungtuItem.TaiLieuGoc = tailieugoc;
                    chungtuItem.TaiLieuKy = tailieuky;
                }
                _context.SaveChanges();
                return new Response(message: "", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
            }
        }

        public Response DeleteChungTu(ChungTuParams chungtu)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.HoSoThanhToan.MaHoSo == chungtu.MaHoSo));

                if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "001", success: false);

                chungtuItem.DaXoa = true;
                _context.SaveChanges();

                return new Response(message: "Xóa chứng từ thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
            }

        }

        public ChungTu GetChungTuByHoSoThanhToanId(Guid hsttid)
        {
            return _context.ChungTu.FirstOrDefault(x => (x.HoSoThanhToan.HoSoId == hsttid));
        }

        public ResponseGetChungTu GetChungTu(ChungTuParams chungtu)
        {
            try
            {
                var listChungTu = from ct in _context.ChungTu
                                  select new ChungTu
                                  {
                                      ChungTuId = ct.ChungTuId,
                                      ChungTuERPId = ct.ChungTuERPId,
                                      DonViThanhToan = ct.DonViThanhToan,
                                      HoSoThanhToan = ct.HoSoThanhToan,
                                      NgayYeuCauLapCT = ct.NgayYeuCauLapCT,
                                      SoChungTuERP = ct.SoChungTuERP,
                                      SoTaiKhoanChuyen = ct.SoTaiKhoanChuyen,
                                      MaNganHangNhan = ct.MaNganHangNhan,
                                      TenNganHangChuyen = ct.TenNganHangChuyen,
                                      TenTaiKhoanChuyen = ct.TenTaiKhoanChuyen,
                                      MaChiNhanhChuyen = ct.MaChiNhanhChuyen,
                                      MaChiNhanhNhan = ct.MaChiNhanhNhan,
                                      MaNganHangChuyen = ct.MaNganHangChuyen,
                                      NoiDungTT = ct.NoiDungTT,
                                      LoaiPhi = ct.LoaiPhi,
                                      LoaiGiaoDich = ct.LoaiGiaoDich,
                                      SoTien = ct.SoTien,
                                      LoaiTienTe = ct.LoaiTienTe,
                                      TyGia = ct.TyGia,
                                      TenNguoiChuyen = ct.TenNguoiChuyen,
                                      DiaChiChuyen = ct.DiaChiChuyen,
                                      MaTinhTPChuyen = ct.MaTinhTPChuyen,
                                      MaNuocChuyen = ct.MaNuocChuyen,
                                      NguoiYeuCauLap = ct.NguoiYeuCauLap,
                                      NguoiDuyet = ct.NguoiDuyet,
                                      NgayDuyet = ct.NgayDuyet,
                                      TrangThaiCT = ct.TrangThaiCT,
                                      KySoId = ct.KySoId,
                                      NgayKy = ct.NgayKy,
                                      CapKy = ct.CapKy,
                                      CTGS_GL = ct.CTGS_GL,
                                      CTGS_AP = ct.CTGS_AP,
                                      CTGS_CM = ct.CTGS_CM,
                                      GhiChu = ct.GhiChu,
                                      DaXoa = ct.DaXoa,
                                      TenTaiKhoanNhan = ct.TenTaiKhoanNhan,
                                      SoTaiKhoanNhan = ct.SoTaiKhoanNhan
                                  };

                if (chungtu.MaHoSo != null) listChungTu = listChungTu.Where(x => (x.HoSoThanhToan.MaHoSo == chungtu.MaHoSo));
                if (chungtu.ChungTuERPId != null) listChungTu = listChungTu.Where(x => (x.ChungTuERPId == chungtu.ChungTuERPId));
                if (chungtu.CapKy != 0) listChungTu = listChungTu.Where(x => (x.CapKy == chungtu.CapKy));
                if (chungtu.TrangThaiCT != null) listChungTu = listChungTu.Where(x => (x.TrangThaiCT == chungtu.TrangThaiCT));
                //if (chungtu.NgayYeuCauLapCT != null) listChungTu = listChungTu.Where(x => (x.NgayYeuCauLapCT == chungtu.NgayYeuCauLapCT));
                if (chungtu.NgayYeuCauLapCTStart != null) listChungTu = listChungTu.Where(x => (x.NgayYeuCauLapCT >= chungtu.NgayYeuCauLapCTStart));
                if (chungtu.NgayYeuCauLapCTEnd != null) listChungTu = listChungTu.Where(x => (x.NgayYeuCauLapCT <= chungtu.NgayYeuCauLapCTEnd));
                if (chungtu.TuKhoa != null) listChungTu = listChungTu.Where(x => (x.HoSoThanhToan.MaHoSo.Contains(chungtu.TuKhoa) || x.GhiChu.Contains(chungtu.TuKhoa) || x.NoiDungTT.Contains(chungtu.TuKhoa)));

                listChungTu = listChungTu.OrderByDescending(x => x.NgayYeuCauLapCT);

                var totalRecord = listChungTu.ToList().Count();
                if (chungtu.PageIndex > 0)
                {
                    listChungTu = listChungTu.Skip(chungtu.PageSize * (chungtu.PageIndex - 1)).Take(chungtu.PageSize);
                }
                var response = listChungTu.ToList();

                return new ResponseGetChungTu(message: "", errorcode: "", success: true, items: response, totalRecord: totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetChungTu(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }

        public Response HotFixKyChungTuCap1(KySoChungTuParams kysochungtu)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == kysochungtu.ChungTuId));
                if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "002", success: false);
                if (chungtuItem.TrangThaiCT == TrangThaiChungTu.KhoiTao) return new Response(message: "Chứng từ chưa được lập trên erp", data: "", errorcode: "003", success: false);
                if (chungtuItem.TrangThaiCT == TrangThaiChungTu.DaHuy) return new Response(message: "Chứng từ đã bị hủy", data: "", errorcode: "004", success: false);

                //cập nhật lại vào bản ghi ký số chứng từ hiện tại (cấp 1)
                var ksctItem = _context.KySoChungTu.FirstOrDefault(x => (x.ChungTu == chungtuItem && x.CapKy == kysochungtu.CapKy && x.DaXoa == false));
                if (ksctItem != null && kysochungtu.CapKy == 1)
                {
                    ksctItem.TaiLieuKy = kysochungtu.TaiLieuKy;
                    //cập nhật bản ghi ký số chứng từ cấp tiếp theo (cấp 2)
                    var nextksctItem = _context.KySoChungTu.FirstOrDefault(x => (x.ChungTu == chungtuItem && x.CapKy == kysochungtu.CapKy + 1 && x.DaXoa == false));
                    if (nextksctItem != null)
                    {
                        nextksctItem.TaiLieuGoc = kysochungtu.TaiLieuKy;
                        chungtuItem.TaiLieuKy = kysochungtu.TaiLieuKy;
                        _context.SaveChanges();
                    }
                }

                return new Response(message: "Ký thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response KyChungTu(KySoChungTuParams kysochungtu)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == kysochungtu.ChungTuId));
                if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "002", success: false);
                if (chungtuItem.TrangThaiCT == TrangThaiChungTu.KhoiTao) return new Response(message: "Chứng từ chưa được lập trên erp", data: "", errorcode: "003", success: false);
                if (chungtuItem.TrangThaiCT == TrangThaiChungTu.DaHuy) return new Response(message: "Chứng từ đã bị hủy", data: "", errorcode: "004", success: false);

                var hoSoItem = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == chungtuItem.HoSoThanhToan.HoSoId);
                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoItem.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoItem.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hoSoItem.QuaTrinhPheDuyetId).ToList();
                BuocPheDuyet buocHienTai = new BuocPheDuyet();
                var thuTu = 0;

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == kysochungtu.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {kysochungtu.ThaoTacBuocPheDuyetId}");
                }
                // khi thực hiện xong một bước, sẽ luôn lưu lại thao tác vừa thực hiện
                // nếu 
                var thaoTacTruoc = _context.ThaoTacBuocPheDuyet
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == hoSoItem.ThaoTacVuaThucHienId);
                if (thaoTacTruoc.DiDenBuocPheDuyetId != null)
                {
                    buocHienTai = _context.BuocPheDuyet.FirstOrDefault(x => x.BuocPheDuyetId == thaoTacTruoc.DiDenBuocPheDuyetId);
                    QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                        {
                            BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                            HoSoId = hoSoItem.HoSoId,
                            ThoiGianTao = DateTime.Now,
                            TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                            ThoiGianXuLy = DateTime.Now,
                            NguoiXuLyId = new Guid(_context.ApplicationUser.FirstOrDefault(x => (x.Id == kysochungtu.NguoiKyId)).Id),
                        };
                        hoSoItem.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                        hoSoItem.ThaoTacVuaThucHienId = kysochungtu.ThaoTacBuocPheDuyetId;
                }
                else
                {
                    if (listBuocPD.Count != 0 && buocPD.Count != 0)
                    {
                        thuTu = buocPD[0].ThuTu;
                        foreach (var item in listBuocPD)
                        {
                            if ((item.ThuTu == buocPD[0].ThuTu + 1))
                            {
                                buocHienTai = item;
                            }
                        }
                        QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                        {
                            BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                            HoSoId = hoSoItem.HoSoId,
                            ThoiGianTao = DateTime.Now,
                            TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                            ThoiGianXuLy = DateTime.Now,
                            NguoiXuLyId = new Guid(_context.ApplicationUser.FirstOrDefault(x => (x.Id == kysochungtu.NguoiKyId)).Id),
                        };
                        hoSoItem.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                        hoSoItem.ThaoTacVuaThucHienId = kysochungtu.ThaoTacBuocPheDuyetId;

                        if (chungtuItem.LoaiChungTu == 1)
                        {
                            hoSoItem.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                        }
                        _context.QuaTrinhPheDuyet.Add(qt);
                    }
                }

                

                //cập nhật lại vào bản ghi ký số chứng từ hiện tại
                var ksctItem = _context.KySoChungTu.FirstOrDefault(x => (x.ChungTu == chungtuItem && x.CapKy == kysochungtu.CapKy && x.DaXoa == false && x.DaKy == false));
                ksctItem.NoiDungKy = kysochungtu.NoiDungKy;
                ksctItem.NgayKy = DateTime.Now;
                ksctItem.NguoiKy = _context.ApplicationUser.FirstOrDefault(x => (x.Id == kysochungtu.NguoiKyId));
                ksctItem.DuLieuKy = kysochungtu.DuLieuKy;
                ksctItem.TaiLieuKy = kysochungtu.TaiLieuKy;
                if (ksctItem.TaiLieuGoc == null) ksctItem.TaiLieuGoc = kysochungtu.TaiLieuGoc; // lệnh này sẽ chỉ chạy khi kí cấp 1
                ksctItem.DaKy = true;

                //cập nhật bản ghi ký số chứng từ cấp tiếp theo
                var nextksctItem = _context.KySoChungTu.FirstOrDefault(x => (x.ChungTu == chungtuItem && x.CapKy == kysochungtu.CapKy + 1 && x.DaXoa == false && x.DaKy == false));
                if (nextksctItem != null)
                {
                    nextksctItem.TaiLieuGoc = kysochungtu.TaiLieuKy;
                    // _context.SaveChanges();
                }


                chungtuItem.KySoId = ksctItem.KySoId;
                chungtuItem.NguoiDuyet = _context.ApplicationUser.FirstOrDefault(x => (x.Id == kysochungtu.NguoiKyId));
                chungtuItem.CapKy = ksctItem.CapKy + 1;
                // chungtuItem.TrangThaiCT += 1;

                chungtuItem.TrangThaiCT = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiChungTu); //trạng thái chứng từ
                chungtuItem.NgayKy = DateTime.Now;
                if (chungtuItem.LoaiChungTu == 0)
                {
                    chungtuItem.TaiLieuKy = kysochungtu.TaiLieuKy;
                }
                else if (chungtuItem.LoaiChungTu == 1)
                {
                    chungtuItem.TaiLieuKyCTGS = kysochungtu.TaiLieuKy;
                }

                // nếu chứng từ thuộc loại chứng từ ghi sổ và đã ký được 2 chứ ký thì sẽ tạo ra 3 bản ghi ký số chứng từ 
                string chungTuId = null; 
                if (kysochungtu.LoaiChungTu == 1 && (ksctItem.CapKy + 1) == 6)
                {
                    // var kySoCT = _context.KySoChungTu.Where(x => x.ChungTu.ChungTuId == chungtuItem.ChungTuId).OrderBy(x => x.CapKy).ToList();
                    // var a = kySoCT[0].NguoiKyId;
                    // Dictionary<string, object> payload = new Dictionary<string, object> {
                    //     { "Trans_Id", chungtuItem.ChungTuERPId },
                    //     { "MaDonViERP", chungtuItem.DonViThanhToan.ERPMaCongTy },
                    //     { "MaHstt", chungtuItem.HoSoThanhToan.MaHoSo },
                    //     { "Sign1", kySoCT[0].NguoiKyId },
                    //     { "Sign2", kySoCT[1].NguoiKyId },
                    //     { "Sign3", "" },
                    // };
                    // HttpClient client = new HttpClient();
                    // var resultResponse = client.PostAsync(
                    //     _configuration["ErpSettings:UpdateApStatusAddress"],
                    //     new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                    // );
                    // if (resultResponse.Result.Content != null)
                    // {
                    //     resultResponse.Wait();
                            
                    // }
                    var buocPheDuyetTiepTheo = _context.BuocPheDuyet
                        .FirstOrDefault(b => b.BuocPheDuyetTruocId == thaoTacBuocPheDuyet.BuocPheDuyetId && !b.DaXoa);

                    if (buocPheDuyetTiepTheo == null)
                    {
                        throw new Exception($"Không tìm thấy thao tác bước phê duyệt tiếp sau bước phê duyệt có Id = {kysochungtu.ThaoTacBuocPheDuyetId}");
                    }

                    var chungtuItem1 = new ChungTu
                    {
                        CTGS_GL = false,
                        CTGS_AP = true,
                        CTGS_CM = false,
                        HoSoThanhToan = hoSoItem,
                        TenTaiKhoanNhan = chungtuItem.TenTaiKhoanNhan,
                        SoTaiKhoanNhan = chungtuItem.SoTaiKhoanNhan,
                        MaNganHangNhan = chungtuItem.MaNganHangNhan,
                        MaChiNhanhNhan = chungtuItem.MaChiNhanhNhan,
                        TenNganHangNhan = chungtuItem.TenNganHangNhan,
                        GhiChu = chungtuItem.GhiChu,
                        DonViThanhToan = _context.DonVi.FirstOrDefault(x => (x.Id == chungtuItem.DonViThanhToan.Id)),
                        SoTien = chungtuItem.SoTien,
                        LoaiTienTe = hoSoItem.LoaiTienTe,
                        TenNguoiChuyen = chungtuItem.TenNguoiChuyen,
                        TyGia = chungtuItem.TyGia,
                        NguoiYeuCauLap = _context.ApplicationUser.FirstOrDefault(x => (x.Id == chungtuItem.NguoiYeuCauLapId)),
                        // NoiDungTT = NoiDungTTKD,
                        NgayYeuCauLapCT = DateTime.Now,
                        TrangThaiCT = Convert.ToInt32(buocPheDuyetTiepTheo.TrangThaiChungTu),
                        CapKy = 1,
                        LoaiChungTu = 0
                    };
                    _context.ChungTu.Add(chungtuItem1); //tạo chứng từ thường
                    chungTuId = chungtuItem1.ChungTuId.ToString();
                    for (int i = 1; i <= 3; i++)
                    {
                        var kysochungtuItem = new KySoChungTu
                        {
                            ChungTu = chungtuItem1,
                            CapKy = i,
                            NgayTao = DateTime.Now,
                            DaKy = false
                        };
                        _context.KySoChungTu.Add(kysochungtuItem);
                    }

                    hoSoItem = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == chungtuItem.HoSoThanhToan.HoSoId);
                    listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoItem.QuyTrinhPheDuyetId).ToList();
                    buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoItem.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hoSoItem.QuaTrinhPheDuyetId).ToList();

                    thuTu = 0;
                    buocHienTai = new BuocPheDuyet();
                    if (listBuocPD.Count != 0 && buocPD.Count != 0)
                    {
                        thuTu = buocPD[0].ThuTu;
                        foreach (var item in listBuocPD)
                        {
                            if ((item.ThuTu == buocPD[0].ThuTu + 1))
                            {
                                buocHienTai = item;
                            }
                        }
                        QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                        {
                            BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                            HoSoId = hoSoItem.HoSoId,
                            ThoiGianTao = DateTime.Now,
                            TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                            ThoiGianXuLy = DateTime.Now,
                            NguoiXuLyId = new Guid(_context.ApplicationUser.FirstOrDefault(x => (x.Id == kysochungtu.NguoiKyId)).Id),
                        };
                        hoSoItem.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;

                        if (chungtuItem.LoaiChungTu == 1)
                        {
                            hoSoItem.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                        }
                        _context.QuaTrinhPheDuyet.Add(qt);
                    }
                }

                _context.SaveChanges();

                return new Response(message: "Ký thành công", data: chungTuId, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public ResponseTrangThaiChiTien GetKySoChungTuByERPChungTuId(string ChungTuERPId)
        {
            try
            {
                var chungTuItem = (from ct in _context.ChungTu
                                   join dv in _context.DonVi on ct.DonViThanhToan equals dv
                                   where ct.ChungTuERPId == ChungTuERPId
                                   select new
                                   {
                                       ChungTu = ct,
                                       DonViTT = dv,
                                       HoSo = ct.HoSoThanhToan
                                   }).FirstOrDefault();

                if (chungTuItem == null) return new ResponseTrangThaiChiTien(message: "Chưa có chứng từ trên erp", errorcode: "001", success: false, data: null);

                var listKySoChungTu = (from ksct in _context.KySoChungTu
                                       join u in _context.ApplicationUser on ksct.NguoiKy equals u
                                       where ksct.ChungTu == chungTuItem.ChungTu
                                       select new
                                       {
                                           ChungTuId = ksct.ChungTu.ChungTuId,
                                           NoiDungKy = ksct.NoiDungKy,
                                           NgayKy = ksct.NgayKy,
                                           DuLieuKy = ksct.DuLieuKy,
                                           CapKy = ksct.CapKy,
                                           TenNguoiKy = u.UserName
                                       }).OrderBy(x => x.CapKy).ToList();

                Dictionary<string, object> res = new Dictionary<string, object>()
                {
                    { "ChungTuERPId", chungTuItem.ChungTu.ChungTuERPId },
                    { "MaHoSo", chungTuItem.HoSo.MaHoSo },
                    { "MaDonViERP", chungTuItem.ChungTu.DonViThanhToan.ERPMaCongTy }
                };
                for (int i = 1; i < 4; i++) res.Add($"TenNguoiKy{i}", null);
                for (int i = 1; i <= listKySoChungTu.Count; i++) res[$"TenNguoiKy{i}"] = listKySoChungTu[i - 1].TenNguoiKy;

                var ycctItem = _context.YeuCauChiTien.FirstOrDefault(x => (x.ChungTu == chungTuItem.ChungTu));
                res.Add("NgayGuiLenhChi", ycctItem.NgayYeuCauChi.Value);
                res.Add("TenNguoiKy4", "Epayment");

                return new ResponseTrangThaiChiTien(message: "", errorcode: "", success: true, data: res);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseTrangThaiChiTien(message: e.Message, errorcode: "002", success: false, data: null);
            }
        }

        public ResponseGetKySoChungTu GetKySoChungTuChoBanTheHien(KySoChungTuParams kysochungtu)
        {
            try
            {
                var listKySoChungTu = from ksct in _context.KySoChungTu
                                      where ksct.ChungTu.ChungTuId == kysochungtu.ChungTuId
                                      select new
                                      { 
                                          CapKy = ksct.CapKy,
                                          DaKy = ksct.DaKy,
                                          DaXoa = ksct.DaXoa,
                                          KySoId = ksct.KySoId
                                      };

                if (kysochungtu.DaKy != null) listKySoChungTu = listKySoChungTu.Where(x => (x.DaKy == kysochungtu.DaKy));

                var totalRecord = listKySoChungTu.ToList().Count();
                if (kysochungtu.PageIndex > 0)
                {
                    listKySoChungTu = listKySoChungTu.Skip(kysochungtu.PageSize * (kysochungtu.PageIndex - 1)).Take(kysochungtu.PageSize);
                }
                var response = listKySoChungTu.ToList();

                return new ResponseGetKySoChungTu(message: "", errorcode: "", success: true, items: response, totalRecord: totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetKySoChungTu(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }

        public ChungTu GetChungTuByYeuCauChi(Guid yeucauchiID)
        {
            var chungtuItem = (from ycct in _context.YeuCauChiTien
                               join ct in _context.ChungTu on ycct.ChungTu equals ct
                               where ycct.YeuCauChiTienId == yeucauchiID
                               select ct);
            var chungTu = new ChungTu();
            if (chungtuItem != null)
            {
                chungTu = chungtuItem.FirstOrDefault();
            }
            return chungTu;
        }

        public ResponseGetKySoChungTu GetKySoChungTu(KySoChungTuParams kysochungtu)
        {
            try
            {
                var listKySoChungTu = from ksct in _context.KySoChungTu
                                      where ksct.ChungTu.CapKy >= kysochungtu.CapKy
                                    //   && ksct.ChungTu.CapKy < 4
                                       && ksct.ChungTu.LoaiChungTu == kysochungtu.LoaiChungTu
                                      select new
                                      {
                                          //ChungTu = ksct.ChungTu,
                                          ChungTu = new 
                                          {
                                              soChungTuERP = ksct.ChungTu.SoChungTuERP,
                                              ngayYeuCauLapCT = ksct.ChungTu.NgayYeuCauLapCT,
                                              noiDungTT = ksct.ChungTu.NoiDungTT,
                                              tenTaiKhoanChuyen = ksct.ChungTu.TenTaiKhoanChuyen,
                                              maNganHangChuyen = ksct.ChungTu.MaNganHangChuyen,
                                              soTaiKhoanChuyen = ksct.ChungTu.SoTaiKhoanChuyen,
                                              tenTaiKhoanNhan = ksct.ChungTu.TenTaiKhoanNhan,
                                              maNganHangNhan = ksct.ChungTu.MaNganHangNhan,
                                              soTaiKhoanNhan = ksct.ChungTu.SoTaiKhoanNhan,
                                              soTien = ksct.ChungTu.SoTien,
                                              loaiTienTe = ksct.ChungTu.LoaiTienTe,
                                              tyGia = ksct.ChungTu.TyGia,
                                              trangThaiCT = ksct.ChungTu.TrangThaiCT,
                                              chungTuId = ksct.ChungTu.ChungTuId,
                                              GhiChu = ksct.ChungTu.GhiChu,
                                              tenNganHangChuyen = ksct.ChungTu.TenNganHangChuyen,
                                              tenNganHangNhan = ksct.ChungTu.TenNganHangNhan,
                                              loaiChungTu = ksct.ChungTu.LoaiChungTu
                                          },
                                          NoiDungKy = ksct.NoiDungKy,
                                          NgayKy = ksct.NgayKy,
                                          //NguoiKy = ksct.NguoiKy,
                                          NguoiKy = ksct.NguoiKy != null ? new 
                                          {
                                              FirstName = ksct.NguoiKy.FirstName,
                                              LastName = ksct.NguoiKy.LastName
                                          } : null,
                                          CapKy = ksct.CapKy,
                                          DaKy = ksct.DaKy,
                                          DaXoa = ksct.DaXoa,
                                          KySoId = ksct.KySoId,
                                          NgayTao = ksct.NgayTao,
                                          HoSoThanhToan = new
                                          {
                                              hoSoId = ksct.ChungTu.HoSoThanhToan.HoSoId,
                                              tenHoSo = ksct.ChungTu.HoSoThanhToan.TenHoSo,
                                              ngayThanhToan = ksct.ChungTu.HoSoThanhToan.NgayThanhToan,
                                              hinhThucChi = ksct.ChungTu.HoSoThanhToan.HinhThucChi,
                                              maHoSo = ksct.ChungTu.HoSoThanhToan.MaHoSo
                                          }
                                      };

                if (kysochungtu.NgayKy != null) listKySoChungTu = listKySoChungTu.Where(x => (x.NgayKy == kysochungtu.NgayKy));
                if (kysochungtu.DaKy != null) listKySoChungTu = listKySoChungTu.Where(x => (x.DaKy == kysochungtu.DaKy));
                if (kysochungtu.NoiDungKy != null) listKySoChungTu = listKySoChungTu.Where(x => x.NoiDungKy.Contains(kysochungtu.NoiDungKy));
                if (kysochungtu.ChungTuId != null) listKySoChungTu = listKySoChungTu.Where(x => (x.ChungTu.chungTuId == kysochungtu.ChungTuId));
                if (kysochungtu.CapKy != null) listKySoChungTu = listKySoChungTu.Where(x => (x.CapKy == kysochungtu.CapKy));
                if (kysochungtu.DaXoa != null) listKySoChungTu = listKySoChungTu.Where(x => (x.DaXoa == kysochungtu.DaXoa));
                if (kysochungtu.NgayYeuCauLapCTStart != null) listKySoChungTu = listKySoChungTu.Where(x => (x.ChungTu.ngayYeuCauLapCT.Date >= kysochungtu.NgayYeuCauLapCTStart.Value.Date));
                if (kysochungtu.NgayYeuCauLapCTEnd != null) listKySoChungTu = listKySoChungTu.Where(x => (x.ChungTu.ngayYeuCauLapCT.Date <= kysochungtu.NgayYeuCauLapCTEnd.Value.Date));
                if (kysochungtu.TrangThaiCT != null) listKySoChungTu = listKySoChungTu.Where(x => (x.ChungTu.trangThaiCT == kysochungtu.TrangThaiCT));
                if (kysochungtu.TuKhoa != null) listKySoChungTu = listKySoChungTu.Where(x => (x.HoSoThanhToan.maHoSo.Contains(kysochungtu.TuKhoa) || x.ChungTu.GhiChu.Contains(kysochungtu.TuKhoa) || x.ChungTu.noiDungTT.Contains(kysochungtu.TuKhoa) || x.HoSoThanhToan.tenHoSo.Contains(kysochungtu.TuKhoa) || x.ChungTu.soChungTuERP.Contains(kysochungtu.TuKhoa)));

                listKySoChungTu = listKySoChungTu.OrderByDescending(x => x.NgayTao);
                var totalRecord = listKySoChungTu.ToList().Count();
                if (kysochungtu.PageIndex > 0)
                {
                    listKySoChungTu = listKySoChungTu.Skip(kysochungtu.PageSize * (kysochungtu.PageIndex - 1)).Take(kysochungtu.PageSize);
                }
                var response = listKySoChungTu.ToList();

                return new ResponseGetKySoChungTu(message: "", errorcode: "", success: true, items: response, totalRecord: totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetKySoChungTu(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }

        public Response CancelChungTu(string NguoiKyId, KySoChungTuParams kysochungtu)
        {
            try
            {
                var chungTuDetail = _context.ChungTu.FirstOrDefault(x => x.ChungTuId == kysochungtu.ChungTuId);
                var hoSoThanhToanDetail = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == kysochungtu.HoSoId);
                var kySoByChungTuId = from ksct in _context.KySoChungTu
                                      where ksct.ChungTu.ChungTuId == kysochungtu.ChungTuId
                                      select ksct;

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == kysochungtu.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {kysochungtu.ThaoTacBuocPheDuyetId}");
                }

                var buocPheDuyet = _context.BuocPheDuyet.FirstOrDefault(b => b.BuocPheDuyetId == thaoTacBuocPheDuyet.BuocPheDuyetId);
                if (buocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy bước phê duyệt Id = {thaoTacBuocPheDuyet.BuocPheDuyetId}");
                }
                
                //quay về bước 1
                var buocPheDuyet1 = _context.BuocPheDuyet.FirstOrDefault(b => b.QuyTrinhPheDuyetId == buocPheDuyet.QuyTrinhPheDuyetId && b.ThuTu == 1 && !b.DaXoa);
                if (buocPheDuyet1 == null)
                {
                    throw new Exception($"Không tìm thấy bước phê duyệt đầu tiên của quy trình phê duyệt Id = {buocPheDuyet.QuyTrinhPheDuyetId}");
                }
                hoSoThanhToanDetail.QuaTrinhPheDuyetId = buocPheDuyet1.BuocPheDuyetId;

                //danh sách các bước
                var buocPheDuyetIds = _context.BuocPheDuyet
                    .Where(b => b.QuyTrinhPheDuyetId == buocPheDuyet.QuyTrinhPheDuyetId && !b.DaXoa)
                    .Select(b => b.BuocPheDuyetId)
                    .ToList();

                // quay về trạng thái hồ sơ của bước phê duyệt 1
                hoSoThanhToanDetail.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);

                //Xóa quá trình phê duyệt
                var quaTrinhPheDuyetRemoves = _context.QuaTrinhPheDuyet.Where(q => buocPheDuyetIds.Contains(q.BuocPheDuyetId)).ToList();
                foreach (var quaTrinhPheDuyetRemove in quaTrinhPheDuyetRemoves)
                {
                    quaTrinhPheDuyetRemove.DaXoa = true;
                }

                var kySoTaiLieuRemove = from tt in _context.KySoTaiLieu
                                        where tt.HoSoThanhToan.HoSoId == kysochungtu.HoSoId
                                        select tt;
                foreach (var item in kySoTaiLieuRemove.ToList())
                {
                    item.DaXoa = true;
                }

                // cập nhật thông tin ký số về trạng thái hủy
                chungTuDetail.TrangThaiCT = TrangThaiChungTu.DaHuy;
                chungTuDetail.DaXoa = true;
                chungTuDetail.CapKy = 1;

                // cập nhật thông tin ký số chứng từ trong bảng KySoChungTu
                foreach (var i in kySoByChungTuId.ToList())
                {
                    i.DaXoa = true;
                }

                //xóa chứng từ, ký số chứng từ cho vpp
                var chungTuRemove = _context.ChungTu.Where(c => c.HoSoThanhToan.HoSoId == kysochungtu.HoSoId).ToList();
                foreach (var item in chungTuRemove)
                {
                    item.DaXoa = true;
                    item.TrangThaiCT = TrangThaiChungTu.DaHuy;
                }
                var chungTuRemoveIds = chungTuRemove.Select(c => c.ChungTuId).ToList();
                var kySoChungTuRemove = _context.KySoChungTu.Where(k => chungTuRemoveIds.Contains(k.ChungTu.ChungTuId)).ToList();
                foreach (var item in kySoChungTuRemove)
                {
                    item.DaXoa = true;
                }

                // cập nhật lý do không ký vào bảng KySoChungTu
                var ksctItem = _context.KySoChungTu.FirstOrDefault(x => (x.KySoId == kysochungtu.KySoId));
                ksctItem.NoiDungKy = kysochungtu.NoiDungKy;
                ksctItem.NguoiKy = _context.ApplicationUser.FirstOrDefault(x => (x.Id == NguoiKyId));
                ksctItem.DaKy = false;

                _context.SaveChanges();
                return new Response(message: "Hủy ký thành công", data: "", errorcode: "", success: true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response UpdateTrangThaiDongCT(Guid ChungTuId, string UserId, Guid thaoTacBuocPheDuyetId)
        {
            try
            {
                var nguoiPheDuyet = _context.Users.FirstOrDefault(x => x.Id == UserId);
                var chungTuDetail = _context.ChungTu.FirstOrDefault(x => x.ChungTuId == ChungTuId);
                if (chungTuDetail == null)
                {
                    return new Response(message: "Lỗi không tìm thấy chứng từ", data: null, errorcode: "002", success: false);
                }
                var ItemHoSoTT = from ksct in _context.ChungTu
                                 where ksct.ChungTuId == ChungTuId
                                 select ksct.HoSoThanhToan.HoSoId;
                var hoSoThanhToanDetail = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == ItemHoSoTT.FirstOrDefault());
                if (hoSoThanhToanDetail == null)
                {
                    return new Response(message: "Lỗi không tìm thấy hồ sơ", data: null, errorcode: "003", success: false);
                }

                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoThanhToanDetail.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoThanhToanDetail.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hoSoThanhToanDetail.QuaTrinhPheDuyetId).ToList();
                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == thaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {thaoTacBuocPheDuyetId}");
                }

                var thuTu = 0;
                BuocPheDuyet buocHienTai = new BuocPheDuyet();
                if (listBuocPD.Count != 0 && buocPD.Count != 0)
                {
                    thuTu = buocPD[0].ThuTu;
                    foreach (var item in listBuocPD)
                    {
                        if ((item.ThuTu == buocPD[0].ThuTu + 1))
                        {
                            buocHienTai = item;
                        }
                    }
                    QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                    {
                        BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                        HoSoId = hoSoThanhToanDetail.HoSoId,
                        ThoiGianTao = DateTime.Now,
                        TrangThaiXuLy = 1,
                        ThoiGianXuLy = DateTime.Now,
                        NguoiXuLyId = new Guid(_context.ApplicationUser.FirstOrDefault(x => (x.Id == UserId)).Id),
                    };
                    hoSoThanhToanDetail.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;

                    if (chungTuDetail.LoaiChungTu == 1)
                    {
                        hoSoThanhToanDetail.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                    }
                    _context.QuaTrinhPheDuyet.Add(qt);
                }
                if (chungTuDetail.TrangThaiCT == TrangThaiChungTu.DaChuyenTien && hoSoThanhToanDetail.TrangThaiHoSo == BCXN.Statics.TrangThaiHoSo.DaThanhToan)
                {
                    // cập nhật trạng thái hồ sơ trong bảng HoSoThanhToan về trạng thái đóng hồ sơ
                    // hoSoThanhToanDetail.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.DongHoSo;
                    hoSoThanhToanDetail.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                    // cập nhật thông tin ký số về trạng thái  đóng hồ sơ
                    // chungTuDetail.TrangThaiCT = TrangThaiChungTu.DongHoSo;
                    // (DEVEPAY 70) đề nghị sửa lại, chỉ chuyển trạng thái hồ sơ, không chuyển trạng thái chứng từ, trạng thái chứng từ cuối cùng chỉ là "Đã thanh toán"
                    // chungTuDetail.TrangThaiCT = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiChungTu);
                    PheDuyetHoSoTT pd = new PheDuyetHoSoTT() { };
                    pd.PheDuyetHoSoTTId = new Guid();
                    pd.HoSoThanhToan = hoSoThanhToanDetail;
                    pd.NguoiThucHien = nguoiPheDuyet;
                    pd.NgayThucHien = DateTime.Now;
                    pd.BuocThucHien = "";
                    pd.TrangThaiHoSo = thaoTacBuocPheDuyet.TrangThaiHoSo;
                    pd.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.DongHoSo.ToString();
                    pd.NoiDung = "đóng hồ sơ chứng từ";
                    _context.PheDuyetHoSoTT.Add(pd);
                    _context.SaveChanges();
                    return new Response(message: "Đóng hố sơ chứng từ thành công", data: "", errorcode: "", success: true);
                }
                else
                {
                    return new Response(message: "Lỗi chưa được thanh toán", data: null, errorcode: "004", success: false);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response GetCTGS(ChungTuParams chungtu)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == chungtu.ChungTuId));
                if (chungtuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "001", success: false);

                string tailieugoc = chungtuItem.TaiLieuGocCTGS;
                string tailieuky = chungtuItem.TaiLieuKyCTGS;

                if(tailieuky == null && tailieugoc == null) return new Response(message: "Có lỗi trong quá trình lấy pc/unc", data: "", errorcode: "002", success: false);

                if(tailieuky == null) return new Response(message: "", data: tailieugoc, errorcode: "", success: true);
                else return new Response(message: "", data: tailieuky, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "002", success: false);
            }
        }
    }
}
