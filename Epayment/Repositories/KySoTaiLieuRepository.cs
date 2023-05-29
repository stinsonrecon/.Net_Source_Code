using BCXN.Data;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Statics;

namespace Epayment.Repositories
{
    public interface IKySoTaiLieuRepository
    {
        Response CreateKySoTaiLieu(KySoTaiLieuParams kstl);
        Response KyToTrinh(KySoTaiLieuParams kstl);
        ResponseGetKySoTaiLieu GetKySoTaiLieuPaging(SearchKySoTaiLieu request);
        ResponseGetKySoTaiLieu GetKySoTaiLieuById(Guid KySoTaiLieuId);
        //Response UpdateSoTaiLieu(KySoTaiLieuParams kstl);
        ResponseGetKySoTaiLieu GetKySoTaiLieuByHoSoId(string HoSoId);
        Response CancelToTrinh(KySoTaiLieuParams request, string NguoiHuy);
        Response KySoPhieuThamTraHoSo (KySoPhieuThamTraPram kstl);
        Response GetPhieuThamTraByIdHSTT (Guid HoSoTTId, Guid GiayToId);
    }

    public class KySoTaiLieuRepository : IKySoTaiLieuRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KySoTaiLieuRepository> _logger;

        public KySoTaiLieuRepository(ApplicationDbContext context, ILogger<KySoTaiLieuRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Response CreateKySoTaiLieu(KySoTaiLieuParams kstl)
        {
            try
            {
                var hsttItem = _context.HoSoThanhToan.FirstOrDefault(x => (x.HoSoId == kstl.HoSoThanhToanId));
                for (int i = 1; i <= 3; i++)
                {
                    var kysotailieuItem = new KySoTaiLieu
                    {
                        HoSoThanhToan = hsttItem,
                        CapKy = i,
                        TaiLieuGoc = kstl.TaiLieuGoc,
                        DaKy = false,
                        NgayTao = DateTime.Now
                    };                   

                    //if (i == 1)
                    //{
                    //    hsttItem.TaiLieuKy = hsttItem.TaiLieuGoc;
                    //    kysotailieuItem.NoiDungKy = "Đồng ý";
                    //    kysotailieuItem.NguoiKy = _context.ApplicationUser.FirstOrDefault(x => (x.Id == kstl.NguoiKyId));
                    //    kysotailieuItem.NgayKy = DateTime.Now;
                    //    kysotailieuItem.TaiLieuKy = kstl.TaiLieuGoc;
                    //}
                    _context.KySoTaiLieu.Add(kysotailieuItem);
                }
                _context.SaveChanges();
                return new Response(message: "Tạo mới ký số tài liệu thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response KyToTrinh(KySoTaiLieuParams kstl)
        {
            try
            {
                var hsttItem = _context.HoSoThanhToan.FirstOrDefault(x => (x.HoSoId == kstl.HoSoThanhToanId));
                if (hsttItem == null) return new Response(message: "Không tìm thấy hồ sơ", data: "", errorcode: "002", success: false);

                //cập nhật lại vào bản ghi ký số tờ trình
                var kstlItem = _context.KySoTaiLieu.FirstOrDefault(x => (x.HoSoThanhToan == hsttItem && x.CapKy == kstl.CapKy && x.DaXoa == false && x.DaKy == false));
                kstlItem.TaiLieuKy = kstl.TaiLieuKy;
                kstlItem.NoiDungKy = kstl.NoiDungKy;
                kstlItem.NgayKy = DateTime.Now;
                kstlItem.NguoiKy = _context.ApplicationUser.FirstOrDefault(x => (x.Id == kstl.NguoiKyId));
                kstlItem.DaKy = true;
                if (kstlItem.TaiLieuGoc == null) kstlItem.TaiLieuGoc = kstl.TaiLieuGoc;

                //cập nhật lại vào hồ sơ
                hsttItem.TaiLieuKy = kstl.TaiLieuKy;
                hsttItem.NgayKy = DateTime.Now;
                if (kstlItem.CapKy == 3) hsttItem.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.ChuaPheDuyet;
                hsttItem.CapKy += 1;                

                _context.SaveChanges();

                var nextkstlItem = _context.KySoTaiLieu.FirstOrDefault(x => (x.HoSoThanhToan == hsttItem && x.CapKy == kstl.CapKy + 1 && x.DaXoa == false && x.DaKy == false));
                if (nextkstlItem != null)
                {
                    nextkstlItem.TaiLieuGoc = kstl.TaiLieuKy;
                    _context.SaveChanges();
                }

                return new Response(message: "Ký thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }
        public ResponseGetKySoTaiLieu GetKySoTaiLieuPaging(SearchKySoTaiLieu request)
        {
            try
            {
                var listKySoTaiLieu = from ksct in _context.KySoTaiLieu
                                      join hstt in _context.HoSoThanhToan on ksct.HoSoThanhToan.HoSoId equals hstt.HoSoId
                                      where (ksct.CapKy == request.CapKy)
                                      && (hstt.CapKy >= request.CapKyTrongHoSo)
                                      && (ksct.DaXoa == false)
                                      && hstt.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.ThamTraHoSo
                                      && hstt.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.ChuaTiepNhan
                                      select new DanhSachKySoTaiLieuViewModel
                                      {
                                        KySoTaiLieuId = ksct.KySoTaiLieuId,
                                        HoSoThanhToanId = ksct.HoSoThanhToan.HoSoId,
                                        MaHoSoTT = ksct.HoSoThanhToan.MaHoSo,
                                        TenHoSoTT = ksct.HoSoThanhToan.TenHoSo,
                                        SoTienNguyenTe = ksct.HoSoThanhToan.SoTien,
                                        SoTienThanhToan = ksct.HoSoThanhToan.SoTienThucTe,
                                        NoiDungKy = ksct.NoiDungKy,
                                        NgayKy = ksct.NgayKy,
                                        NguoiKyId = ksct.NguoiKy.Id,
                                        TenNguoiKy = ksct.NguoiKy.UserName,
                                        // TaiLieuGoc = ksct.TaiLieuGoc,
                                        // TaiLieuKy = ksct.TaiLieuKy,
                                        DaXoa = ksct.DaXoa,
                                        CapKy = ksct.CapKy,
                                        DaKy = ksct.DaKy == true ? 1 : 0,
                                        NgayTao = ksct.NgayTao,
                                        LoaiTien = ksct.HoSoThanhToan.LoaiTienTe,
                                        CapKyHoSoTT = ksct.HoSoThanhToan.CapKy,
                                        // FileKyHoSoTT = ksct.HoSoThanhToan.TaiLieuKy,
                                        // FileGocHoSoTT = ksct.HoSoThanhToan.TaiLieuGoc
                                      };
                if (request.DaKy != -99 ) listKySoTaiLieu = listKySoTaiLieu.Where(x => (x.DaKy == request.DaKy));
                if (!String.IsNullOrEmpty(request.TuKhoa)) listKySoTaiLieu = listKySoTaiLieu.Where(x => (x.NoiDungKy == request.TuKhoa) || x.MaHoSoTT == request.TuKhoa);
                if (request.TuNgay != DateTime.MinValue) listKySoTaiLieu = listKySoTaiLieu.Where(x => (x.NgayTao.Date >= request.TuNgay.Date));
                if (request.DenNgay != DateTime.MinValue) listKySoTaiLieu = listKySoTaiLieu.Where(x => (x.NgayTao.Date <= request.DenNgay.Date));
                listKySoTaiLieu = listKySoTaiLieu.Where(x => x.CapKy == request.CapKy);
                listKySoTaiLieu = listKySoTaiLieu.OrderByDescending(x => x.NgayTao);
                var totalRecord = listKySoTaiLieu.ToList().Count();
                if (request.PageIndex > 0)
                {
                    listKySoTaiLieu = listKySoTaiLieu.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }
                var response = listKySoTaiLieu.ToList();

                return new ResponseGetKySoTaiLieu(message: "", errorcode: "", success: true, items: response, totalRecord: totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetKySoTaiLieu(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }

        public ResponseGetKySoTaiLieu GetKySoTaiLieuById(Guid KySoTaiLieuId)
        {
             try
            {
                var listKySoTaiLieu = from ksct in _context.KySoTaiLieu
                                      where ksct.KySoTaiLieuId == KySoTaiLieuId
                                      select new DanhSachKySoTaiLieuViewModel
                                      {
                                        KySoTaiLieuId = ksct.KySoTaiLieuId,
                                        HoSoThanhToanId = ksct.HoSoThanhToan.HoSoId,
                                        MaHoSoTT = ksct.HoSoThanhToan.MaHoSo,
                                        TenHoSoTT = ksct.HoSoThanhToan.TenHoSo,
                                        SoTienNguyenTe = ksct.HoSoThanhToan.SoTien,
                                        SoTienThanhToan = ksct.HoSoThanhToan.SoTienThucTe,
                                        NoiDungKy = ksct.NoiDungKy,
                                        NgayKy = ksct.NgayKy,
                                        NguoiKyId = ksct.NguoiKy.Id,
                                        TenNguoiKy = ksct.NguoiKy.UserName,
                                        TaiLieuGoc = ksct.TaiLieuGoc,
                                        TaiLieuKy = ksct.TaiLieuKy,
                                        DaXoa = ksct.DaXoa,
                                        CapKy = ksct.CapKy,
                                        DaKy = ksct.DaKy == true ? 1 : 0,
                                        NgayTao = ksct.NgayTao,
                                        LoaiTien = ksct.HoSoThanhToan.LoaiTienTe,
                                        CapKyHoSoTT = ksct.HoSoThanhToan.CapKy,
                                        FileKyHoSoTT = ksct.HoSoThanhToan.TaiLieuKy,
                                      };
                var response = listKySoTaiLieu.FirstOrDefault();
                return new ResponseGetKySoTaiLieu(message: "", errorcode: "", success: true, items: response, totalRecord: 1);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetKySoTaiLieu(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }
        public ResponseGetKySoTaiLieu GetKySoTaiLieuByHoSoId(string HoSoId)
        {
             try
            {
                var listKySoTaiLieu = from ksct in _context.KySoTaiLieu
                                      where ksct.HoSoThanhToan.HoSoId.ToString() == HoSoId && ksct.DaXoa != true
                                      select new DanhSachKySoTaiLieuViewModel
                                      {
                                        KySoTaiLieuId = ksct.KySoTaiLieuId,
                                        HoSoThanhToanId = ksct.HoSoThanhToan.HoSoId,
                                        MaHoSoTT = ksct.HoSoThanhToan.MaHoSo,
                                        TenHoSoTT = ksct.HoSoThanhToan.TenHoSo,
                                        SoTienNguyenTe = ksct.HoSoThanhToan.SoTien,
                                        SoTienThanhToan = ksct.HoSoThanhToan.SoTienThucTe,
                                        NoiDungKy = ksct.NoiDungKy,
                                        NgayKy = ksct.NgayKy,
                                        NguoiKyId = ksct.NguoiKy.Id,
                                        TenNguoiKy = ksct.NguoiKy.UserName,
                                       // TaiLieuGoc = ksct.TaiLieuGoc,
                                       // TaiLieuKy = ksct.TaiLieuKy,
                                        DaXoa = ksct.DaXoa,
                                        CapKy = ksct.CapKy,
                                        DaKy = ksct.DaKy == true ? 1 : 0,
                                        NgayTao = ksct.NgayTao,
                                        LoaiTien = ksct.HoSoThanhToan.LoaiTienTe,
                                        CapKyHoSoTT = ksct.HoSoThanhToan.CapKy,
                                        FileKyHoSoTT = ksct.HoSoThanhToan.TaiLieuKy,
                                      };
                var response = listKySoTaiLieu;
                return new ResponseGetKySoTaiLieu(message: "", errorcode: "", success: true, items: response, totalRecord: response.Count());
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetKySoTaiLieu(message: e.Message, errorcode: "001", success: false, items: null, totalRecord: 0);
            }
        }

        public Response CancelToTrinh(KySoTaiLieuParams request, string nguoiHuy)
        {
            try
            {
                var hoSoDetail = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == request.HoSoThanhToanId);
                if(hoSoDetail == null){
                    return new Response(message: "Lỗi", data: "không tìm thấy dữ liệu", errorcode: "001", success: false);
                }
                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                        .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {request.ThaoTacBuocPheDuyetId}");
                }
                var toTrinhByHoSoId = from tt in _context.KySoTaiLieu
                                    where tt.HoSoThanhToan.HoSoId == request.HoSoThanhToanId
                                    select tt;
                // cập nhật trạng thái của 3 bản ghi trong bảng KySoTaiLieu
                foreach (var item in toTrinhByHoSoId.ToList())
                {
                    item.DaXoa = true;
                }
                // khi từ chối phê duyệt tờ trình, sẽ xóa bỏ tờ trình đó khỏi hô danh mục tài liệu của hồ sơ
                var chiTietGiayTo = _context.ChiTietGiayToHSTT.FirstOrDefault(x => x.GiayTo.GiayToId == thaoTacBuocPheDuyet.GiayToId && x.HoSoThanhToan.HoSoId == request.HoSoThanhToanId);
                _context.ChiTietGiayToHSTT.Remove(chiTietGiayTo);

                // cập nhật lý do không ký vào bảng KySoTaiLieu
                if (request.KySoTaiLieuId != Guid.Empty){
                    var kySoTaiLieuDetail = _context.KySoTaiLieu.FirstOrDefault(x => x.KySoTaiLieuId == request.KySoTaiLieuId);
                    kySoTaiLieuDetail.NoiDungKy = request.NoiDungKy;
                    kySoTaiLieuDetail.NguoiKy = _context.ApplicationUser.FirstOrDefault(x => (x.Id == nguoiHuy));
                }  
                
                _context.SaveChanges();
                return new Response(message: "Hủy phê duyệt tờ trình thành công", data: "", errorcode: "", success: true);
            }
            catch (System.Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new Response(message: "Lỗi", data: e.Message, errorcode: "001", success: false);
            }

        }

        public Response KySoPhieuThamTraHoSo(KySoPhieuThamTraPram kstl)
        {
            try
            {
                var hsttItem = _context.HoSoThanhToan.FirstOrDefault(x => (x.HoSoId == kstl.HoSoThanhToanId));
                if (hsttItem == null) return new Response(message: "Không tìm thấy hồ sơ", data: "", errorcode: "002", success: false);

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == kstl.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {kstl.ThaoTacBuocPheDuyetId}");
                }

                var nguoiPheDuyet = _context.Users.FirstOrDefault(x => x.Id == kstl.NguoiKyId);
                var hoSoDetail = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == kstl.HoSoThanhToanId);
                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoDetail.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoDetail.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hoSoDetail.QuaTrinhPheDuyetId).ToList();
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
                }
                hoSoDetail.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                {
                    BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                    HoSoId = hoSoDetail.HoSoId,
                    ThoiGianTao = DateTime.Now,
                    TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                    ThoiGianXuLy = DateTime.Now,
                    NguoiXuLyId = new Guid(nguoiPheDuyet.Id),
                };
                _context.QuaTrinhPheDuyet.Add(qt);

                // ký xong cấp 1 cập nhật vào hô sơ trạng thái ký phiếu thẩm tra
                if (kstl.CapKy == TrangThaiChiTietGiayToThamTra.TrinhKyCap1 || kstl.CapKy == TrangThaiChiTietGiayToThamTra.TrinhKyCap2)
                {
                    hsttItem.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                }
                // ký xong cấp 2 cập nhật vào hô sơ trạng thái trình ký
                //if (kstl.CapKy == TrangThaiChiTietGiayToThamTra.TrinhKyCap2)
                //{
                //    hsttItem.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                //}
                //cập nhật lại vào bản ghi chi tiết giấy tờ 
                var ctgtItem = _context.ChiTietGiayToHSTT.FirstOrDefault(x => (x.HoSoThanhToan.HoSoId == kstl.HoSoThanhToanId && x.GiayTo.GiayToId == kstl.GiayToId));
                if (ctgtItem == null ) return new Response(message: "Không tìm tài liệu", data: "", errorcode: "006", success: false);
                ctgtItem.FileDinhKem = kstl.TaiLieuKy;
                ctgtItem.TrangThaiGiayTo = kstl.CapKy;
                _context.SaveChanges();

                return new Response(message: "Ký thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response GetPhieuThamTraByIdHSTT(Guid HoSoTTId, Guid GiayToId)
        {
           try
            {
                KySoPhieuThamTraHoSoViewModel ksptt = new KySoPhieuThamTraHoSoViewModel();
                var ctgtItem = from cthstt in _context.ChiTietGiayToHSTT
                                // join htt in _context.HoSoThanhToan on cthstt.HoSoThanhToan.HoSoId equals htt.HoSoId 
                                // join gt in _context.GiayTo on cthstt.GiayTo.GiayToId equals gt.GiayToId
                                // join user in _context.ApplicationUser on cthstt.NguoiCapNhat.Id equals user.Id
                                where cthstt.HoSoThanhToan.HoSoId == HoSoTTId && cthstt.GiayTo.GiayToId == GiayToId
                                select new KySoPhieuThamTraHoSoViewModel {
                                    TaiLieuKy = cthstt.FileDinhKem,
                                    CapKy = cthstt.TrangThaiGiayTo,
                                    HoSoThanhToanId = cthstt.HoSoThanhToan.HoSoId,
                                    GiayToId = cthstt.GiayTo.GiayToId,
                                    TenNguoiKy = cthstt.NguoiCapNhat.FirstName + " " + cthstt.NguoiCapNhat.LastName,
                                    NguoiKyId = cthstt.NguoiCapNhat.Id,
                                };
                    var listCtgt = ctgtItem.ToList();
                    if (listCtgt.Count() <= 0 ){
                        return new Response(message: "không tìm thấy dữ liệu", data: null, errorcode: "002", success: false);
                    }
                var pdItem = _context.PheDuyetHoSoTT.Where(x => x.HoSoThanhToan.HoSoId == HoSoTTId && x.NguoiThucHien.Id == listCtgt[0].NguoiKyId && Convert.ToInt32(x.TrangThaiHoSo) == BCXN.Statics.TrangThaiHoSo.ThamTraHoSo);
                ksptt.HoSoThanhToanId = listCtgt[0].HoSoThanhToanId;
                ksptt.GiayToId = listCtgt[0].GiayToId;
                ksptt.CapKy = listCtgt[0].CapKy;
                ksptt.NoiDungKy = pdItem.ToList().LastOrDefault().NoiDung;
                ksptt.NguoiKyId = listCtgt[0].NguoiKyId;
                ksptt.TenNguoiKy = listCtgt[0].TenNguoiKy;
                ksptt.TaiLieuKy = listCtgt[0].TaiLieuKy;

                return new Response(message: "Ký thành công", data:ksptt, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }
    }
}
