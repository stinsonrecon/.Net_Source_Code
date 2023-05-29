using BCXN.Data;
using BCXN.Statics;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Repositories
{
    public interface IYeuCauChiTienRepository
    {
        Response CreateYeuCauChiTien(YeuCauChiTienParams ycct);
        ResponseGetYeuCauChiTien GetYeuCauChiTien(SearchYeuCauChiTien ycct);
        Response ERPUpdateYeuCauChiTien(YeuCauChiTienParams ycct);
        Response CreateYeuCauChiTienMat(YeuCauChiTienParams ycct);
        Response UpdateYeuCauChiTien(YeuCauChiTienParams ycct);
    }
    public class YeuCauChiTienRepository : IYeuCauChiTienRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<YeuCauChiTienRepository> _logger;

        public YeuCauChiTienRepository(ApplicationDbContext context, ILogger<YeuCauChiTienRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Response UpdateYeuCauChiTien(YeuCauChiTienParams ycct)
        {
            try
            {
                var ycctItem = _context.YeuCauChiTien.FirstOrDefault(x => (x.ChungTu.ChungTuId == ycct.ChungTuId && x.YeuCauChiTienId == ycct.YeuCauChiTienId));
                if (ycct.MaKetQuaChi != null && ycctItem.MaKetQuaChi != "") ycctItem.MaKetQuaChi = ycct.MaKetQuaChi;
                ycctItem.TrangThaiChi = ycct.TrangThaiChi;
                if (ycct.ChuKy != null && ycctItem.ChuKy != "") ycctItem.ChuKy = ycct.ChuKy;

                if (ycct.TrangThaiChi == TrangThaiChiTien.GuiLenhChiTienThanhCong)
                {
                    var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == ycct.ChungTuId));
                    chungtuItem.TrangThaiCT = TrangThaiChungTu.DaGuiLenhChuyenTien;
                }

                _context.SaveChanges();

                return new Response(message: "", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response CreateYeuCauChiTien(YeuCauChiTienParams ycct)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == ycct.ChungTuId));
                var nganhangchiItem = (from tk in _context.TaiKhoanNganHang join nh in _context.NganHang on tk.NganHang equals nh where tk.SoTaiKhoan == chungtuItem.SoTaiKhoanChuyen select nh).FirstOrDefault();
                var nganhangthuhuongItem = (from tk in _context.TaiKhoanNganHang join nh in _context.NganHang on tk.NganHang equals nh where tk.SoTaiKhoan == chungtuItem.SoTaiKhoanNhan select nh).FirstOrDefault();

                var yeucauchitienItem = new YeuCauChiTien
                {
                    ChungTu = chungtuItem,
                    SoTienTT = chungtuItem.SoTien,
                    NganHangChi = nganhangchiItem,
                    NganHangThuHuong = nganhangthuhuongItem,
                    TaiKhoanChi = chungtuItem.SoTaiKhoanChuyen,
                    TaiKhoanThuHuong = chungtuItem.SoTaiKhoanNhan,
                    NguoiThuHuong = chungtuItem.TenTaiKhoanNhan,
                    TrangThaiChi = TrangThaiChiTien.LoiChuaXacDinh,
                    NgayYeuCauChi = DateTime.Now,
                    NguoiYeuCauChi = _context.ApplicationUser.FirstOrDefault(x => (x.Id == ycct.NguoiYeuCauChiId)),
                    ChuKy = ycct.ChuKy
                };
                _context.YeuCauChiTien.Add(yeucauchitienItem);
                if (ycct.NgayGiaoDichThucTe.Date < DateTime.Now.Date){
                    _logger.LogWarning($"[CreateYeuCauChiTien] Ngày giao dịch thực tế phải lớn hơn hoặc bằng ngày hiện tại");
                    return new Response(message: "Ngày giao dịch thực tế phải lớn hơn hoặc bằng ngày hiện tại", data: "", errorcode: "05", success: false);
                }
                //update ngày giao dịch thực tế vào chứng từ
                chungtuItem.NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe;
                
                _context.SaveChanges();

                return new Response(message: "Thêm mới thành công", data: _context.YeuCauChiTien.FirstOrDefault(x => (x == yeucauchitienItem)).YeuCauChiTienId, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                 _logger.LogWarning($"[CreateYeuCauChiTien] lỗi: {ex}");
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response ERPUpdateYeuCauChiTien(YeuCauChiTienParams ycct)
        {
            try
            {
                var ycctItem = _context.YeuCauChiTien.FirstOrDefault(x => (x.ChungTu.ChungTuId == ycct.ChungTuId && x.YeuCauChiTienId == ycct.YeuCauChiTienId));
                ycctItem.ThoiGianChi = ycct.ThoiGianChi;
                var chungtuItem = ycctItem.ChungTu;
                if (ycct.MaKetQuaChi.ToLower() == "true")
                {
                    ycctItem.TrangThaiChi = TrangThaiChiTien.DaChuyenTien;
                    ycctItem.MaKetQuaChi = "Ngân hàng đã chuyển tiền";
                    chungtuItem.TrangThaiCT = TrangThaiChungTu.DaChuyenTien;
                }
                else
                {
                    ycctItem.MaKetQuaChi = "Ngân hàng chưa chuyển tiền";
                    ycctItem.TrangThaiChi = TrangThaiChiTien.NganHangXuLyLoi;
                    chungtuItem.TrangThaiCT = TrangThaiChungTu.ThuTruongCoQuanDaKy;
                }
                _context.SaveChanges();

                _logger.LogWarning($"[ERPUpdateYeuCauChiTien] ERP cập nhật trạng thái chi tiền cho chứng từ: {ycct.ChungTuId}");
                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public ResponseGetYeuCauChiTien GetYeuCauChiTien(SearchYeuCauChiTien ycct)
        {
            try
            {
                var listYeuCauChiTien = from item in _context.YeuCauChiTien
                                            //join ct in _context.ChungTu on item.ChungTu.ChungTuId equals ct.ChungTuId
                                        select new YeuCauChiTienParams
                                        {
                                            YeuCauChiTienId = item.YeuCauChiTienId,
                                            ChungTuId = item.ChungTu.ChungTuId,
                                            SoTienTT = item.SoTienTT,
                                            NganHangChiId = item.NganHangChi.NganHangId,
                                            NganHangThuHuongId = item.NganHangThuHuong.NganHangId,
                                            TaiKhoanChi = item.TaiKhoanChi,
                                            TaiKhoanThuHuong = item.TaiKhoanThuHuong,
                                            NguoiThuHuong = item.NguoiThuHuong,
                                            TrangThaiChi = item.TrangThaiChi,
                                            NgayYeuCauChi = item.NgayYeuCauChi,
                                            NguoiYeuCauChiId = item.NguoiYeuCauChi.Id,
                                            //ChuKy = item.ChuKy,
                                            MaKetQuaChi = item.MaKetQuaChi,
                                            ThoiGianChi = item.ThoiGianChi.Value,
                                            //   NoiDungTT = ct.NoiDungTT,
                                            //   LoaiGiaoDich = ct.LoaiGiaoDich,
                                            //   LoaiPhi = ct.LoaiPhi,
                                            //   LoaiTienTe = ct.LoaiTienTe,
                                            //   TenNganHangChuyen = ct.TenNganHangChuyen,
                                            //   TenNganHangNhan = ct.TenNganHangNhan,
                                            //   SoChungTuERP = ct.SoChungTuERP,
                                            ChungTu = item.ChungTu
                                        };
                if (listYeuCauChiTien.Any())
                {
                    if (ycct != null)
                    {
                        if(ycct.TrangThaiChi != -99)
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(s => s.TrangThaiChi == ycct.TrangThaiChi); 
                        }
                        if (!String.IsNullOrEmpty(ycct.NganHangChiId))
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(s => s.NganHangChiId.ToString() == ycct.NganHangChiId);
                        }
                        if (!String.IsNullOrEmpty(ycct.NganHangThuHuongId))
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(s => s.NganHangThuHuongId.ToString() == ycct.NganHangThuHuongId);
                        }
                        if (!String.IsNullOrEmpty(ycct.TuKhoa))
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(s => s.NguoiThuHuong.Contains(ycct.TuKhoa) || s.MaKetQuaChi.Contains(ycct.TuKhoa) || s.ChungTu.NoiDungTT.Contains(ycct.TuKhoa) || s.ChungTu.SoChungTuERP.Contains(ycct.TuKhoa) || s.YeuCauChiTienId.ToString().Contains(ycct.TuKhoa));
                        }
                        if (ycct.ThoiGianChiTu != DateTime.MinValue)
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(x => x.ThoiGianChi.Date >= ycct.ThoiGianChiTu);
                        }
                        if (ycct.ThoiGianChiDen != DateTime.MinValue)
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(x => x.ThoiGianChi.Date <= ycct.ThoiGianChiDen);
                        }
                        if (ycct.NgayYeuCauChiTu != DateTime.MinValue)
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(x => x.NgayYeuCauChi.Value.Date >= ycct.NgayYeuCauChiTu);
                        }
                        if (ycct.NgayYeuCauChiDen != DateTime.MinValue)
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Where(x => x.NgayYeuCauChi.Value.Date <= ycct.NgayYeuCauChiDen);
                        }

                        listYeuCauChiTien = listYeuCauChiTien.OrderByDescending(x => x.NgayYeuCauChi);

                        var totalRecord = listYeuCauChiTien.ToList().Count();
                        if (ycct.PageIndex > 0)
                        {
                            listYeuCauChiTien = listYeuCauChiTien.Skip(ycct.PageSize * (ycct.PageIndex - 1)).Take(ycct.PageSize);
                        }
                        var listYeuCauChiTienPaging = listYeuCauChiTien.ToList();
                        return new ResponseGetYeuCauChiTien(message: "", errorcode: "", success: true, items: listYeuCauChiTienPaging, totalRecord: totalRecord);
                    }
                    else
                    {
                        return new ResponseGetYeuCauChiTien(message: "", errorcode: "", success: false, items: null, totalRecord: 0);
                    }
                }
                else
                {
                    return new ResponseGetYeuCauChiTien(message: "", errorcode: "", success: false, items: null, totalRecord: 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new ResponseGetYeuCauChiTien(message: ex.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }

        public Response CreateYeuCauChiTienMat(YeuCauChiTienParams ycct)
        {
            try
            {
                var chungtuItem = _context.ChungTu.FirstOrDefault(x => (x.ChungTuId == ycct.ChungTuId));
                var hoSoTTItem = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString().ToUpper() == ycct.HoSoId.ToUpper());
                var yeucauchitienItem = new YeuCauChiTien
                {
                    ChungTu = chungtuItem,
                    SoTienTT = ycct.SoTienTT,
                    NguoiThuHuong = ycct.NguoiThuHuong,
                    TrangThaiChi = TrangThaiChiTien.DaChuyenTien,
                    NgayYeuCauChi = DateTime.Now,
                    NguoiYeuCauChi = _context.ApplicationUser.FirstOrDefault(x => (x.Id == ycct.NguoiYeuCauChiId)),
                    ChuKy = ycct.ChuKy,
                    ThoiGianChi = DateTime.Now
                };
                _context.YeuCauChiTien.Add(yeucauchitienItem);
                //_context.SaveChanges();
                chungtuItem.TrangThaiCT = TrangThaiChungTu.DaChuyenTien;
                if (ycct.NgayGiaoDichThucTe.Date < DateTime.Now.Date ){
                    return new Response(message: "Ngày giao dịch thực tế phải lớn hơn hoặc bằng ngày hiện tại", data: "", errorcode: "05", success: false);
                }
                chungtuItem.NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe;
                //_context.SaveChanges();

                hoSoTTItem.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.DaThanhToan;
                _context.SaveChanges();

                _logger.LogWarning($"[CreateYeuCauChiTienMat] Đã chi tiền mặt cho chứng từ: {ycct.ChungTuId}");
                return new Response(message: "Thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }
    }
}
