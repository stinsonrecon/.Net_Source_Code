using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using BCXN.Models;

namespace Epayment.Repositories
{
    public class LoaiHoSoRepository : ILoaiHoSoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoaiHoSoRepository> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        public LoaiHoSoRepository(ApplicationDbContext context, ILogger<LoaiHoSoRepository> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this._logger = logger;
            this.userManager = userManager;
        }

        public ResponsePostViewModel CreateLoaiHoSo(CreateLoaiHoSo request)
        {
            try
            {
                var checkName = _context.LoaiHoSo.FirstOrDefault(x => x.MaLoaiHoSo == request.MaLoaiHoSo);
                if (checkName != null)
                {
                    return new ResponsePostViewModel("Mã Loại hồ sơ đã tồn tại trên hệ thống", 400);
                }
                var LoaiHoSoId = Guid.NewGuid();
                _context.LoaiHoSo.Add(
                    new LoaiHoSo
                    {
                        MaLoaiHoSo = request.MaLoaiHoSo,
                        LoaiHoSoId = LoaiHoSoId,
                        TenLoaiHoSo = request.TenLoaiHoSo,
                        NgayTao = DateTime.Now,
                        MoTa = request.Mota,
                        TrangThai = request.TrangThai,
                        // DaXoa = request.DaXoa
                        DaXoa = 1
                    }
                );
                _context.SaveChanges();
                foreach (var i in request.TaiLieuYeuCau)
                {
                    _context.GiayToLoaiHoSo.Add(
                        new GiayToLoaiHoSo
                        {
                            GiayToLoaiHoSoId = Guid.NewGuid(),
                            LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == LoaiHoSoId),
                            GiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId.ToString() == i.GiayToId),
                            NgayTao = DateTime.Now,
                            BatBuoc = i.BatBuoc,
                            PheDuyetKySo = i.PheDuyetKySo,
                            Nguon = i.Nguon,
                            MoTa = i.MoTa,
                            ThuTu = i.ThuTu
                        }
                    );
                }
                // _context.SaveChanges();
                foreach (var i in request.LuongPheDuyet)
                {
                    if (i.VaiTroId != null)
                    {
                        var vaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == i.VaiTroId);
                        if (vaiTro == null)
                        {
                            _context.VaiTro.Add(
                            new VaiTro
                            {
                                VaiTroId = new Guid(i.VaiTroId),
                                TenVaiTro = _context.ApplicationRole.FirstOrDefault(x => x.Id == i.VaiTroId).Name,
                            }
                         );
                            _context.SaveChanges();
                        }
                    }
                    _context.LuongPheDuyet.Add(
                        new LuongPheDuyet
                        {
                            BuocThucHienId = Guid.NewGuid(),
                            VaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == i.VaiTroId),
                            TrangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => x.TrangThaiHoSoId.ToString() == i.TrangThaiHoSoId),
                            LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == LoaiHoSoId),
                            TenBuoc = i.TenBuoc,
                            ChuyenSangERP = i.ChuyenSangERP,
                            CoThamTra = i.CoThamTra,
                            ThuTu = i.ThuTu,
                            DaXoa = 1
                        }
                    );
                }
                _context.SaveChanges();
                return new ResponsePostViewModel("Thêm mới thành công", 200);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostViewModel(e.ToString(), 400);
            }
        }

        public ResponsePostViewModel UpdateLoaiHoSo(CreateLoaiHoSo request)
        {
            try
            {
                var Item = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId.ToString() == request.LoaiHoSoId);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy báo cáo", 400);
                }
                if (!Item.MaLoaiHoSo.Equals(request.MaLoaiHoSo))
                {
                    var checkName = _context.LoaiHoSo.FirstOrDefault(x => x.MaLoaiHoSo == request.MaLoaiHoSo);
                    if (checkName != null)
                    {
                        return new ResponsePostViewModel("Mã Loại hồ sơ đã tồn tại trên hệ thống", 400);
                    }
                }

                // Item.LoaiHoSoId = new Guid(request.LoaiHoSoId);
                Item.TenLoaiHoSo = request.TenLoaiHoSo;
                Item.MaLoaiHoSo = request.MaLoaiHoSo;
                Item.MoTa = request.Mota;
                // Item.NgayTao = DateTime.Now;
                Item.TrangThai = request.TrangThai;
                Item.DaXoa = 1;
                // _context.SaveChanges();

                var taiLieu = from tl in _context.GiayToLoaiHoSo
                              where tl.LoaiHoSo.LoaiHoSoId == Item.LoaiHoSoId
                              select tl.GiayToLoaiHoSoId;
                if (request.TaiLieuYeuCau.Count() == 0)
                {
                    foreach (var removeAll in taiLieu.ToList())
                    {
                        var del = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayToLoaiHoSoId == removeAll);
                        _context.GiayToLoaiHoSo.Remove(del);
                    }
                }
                else
                {
                    foreach (var i in request.TaiLieuYeuCau)
                    {
                        if (taiLieu.Any(x => x.ToString().ToUpper() == i.GiayToLoaiHoSoId.ToUpper()))
                        {
                            foreach (var j in taiLieu.ToList())
                            {
                                if (i.GiayToLoaiHoSoId.ToUpper() == j.ToString().ToUpper())
                                {
                                    var updateTaiLieu = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayToLoaiHoSoId == j);
                                    updateTaiLieu.GiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId.ToString() == i.GiayToId);
                                    updateTaiLieu.BatBuoc = i.BatBuoc;
                                    updateTaiLieu.Nguon = i.Nguon;
                                    updateTaiLieu.PheDuyetKySo = i.PheDuyetKySo;
                                    updateTaiLieu.MoTa = i.MoTa;
                                    updateTaiLieu.ThuTu = i.ThuTu;
                                }
                                else if (i.GiayToLoaiHoSoId.ToUpper() != j.ToString().ToUpper() && !request.TaiLieuYeuCau.Any(x => x.GiayToLoaiHoSoId.ToUpper() == j.ToString().ToUpper()))
                                {
                                    var deleteTaiLieu = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayToLoaiHoSoId == j);
                                    _context.GiayToLoaiHoSo.Remove(deleteTaiLieu);
                                }
                            }
                        }
                        else
                        {
                            if (i.GiayToLoaiHoSoId == "")
                            {
                                _context.GiayToLoaiHoSo.Add(
                                    new GiayToLoaiHoSo
                                    {
                                        GiayToLoaiHoSoId = Guid.NewGuid(),
                                        LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == Item.LoaiHoSoId),
                                        GiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId.ToString() == i.GiayToId),
                                        NgayTao = DateTime.Now,
                                        BatBuoc = i.BatBuoc,
                                        PheDuyetKySo = i.PheDuyetKySo,
                                        Nguon = i.Nguon,
                                        MoTa = i.MoTa,
                                        ThuTu = i.ThuTu
                                    }
                                );
                            }
                        }
                    }
                }

                // _context.SaveChanges();
                // }

                var luongpd = from lpd in _context.LuongPheDuyet
                              where lpd.LoaiHoSo.LoaiHoSoId == Item.LoaiHoSoId
                              select lpd.BuocThucHienId;
                if (request.LuongPheDuyet.Count() == 0)
                {
                    foreach (var deleteAllLuong in luongpd.ToList())
                    {
                        var delLuong = _context.LuongPheDuyet.FirstOrDefault(x => x.BuocThucHienId == deleteAllLuong);
                        delLuong.DaXoa = 0;
                    }
                }
                else
                {
                    foreach (var i in request.LuongPheDuyet)
                    {
                        if (luongpd.Any(x => x.ToString().ToUpper() == i.BuocThucHienId.ToUpper()))
                        {
                            foreach (var j in luongpd.ToList())
                            {
                                if (i.BuocThucHienId.ToUpper() == j.ToString().ToUpper())
                                {
                                    if (i.VaiTroId != null)
                                    {
                                        var vaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == i.VaiTroId);
                                        if (vaiTro == null)
                                        {
                                            _context.VaiTro.Add(
                                            new VaiTro
                                            {
                                                VaiTroId = new Guid(i.VaiTroId),
                                                TenVaiTro = _context.ApplicationRole.FirstOrDefault(x => x.Id == i.VaiTroId).Name,
                                            }
                                         );
                                            _context.SaveChanges();
                                        }
                                    }
                                    var updateLuongPD = _context.LuongPheDuyet.FirstOrDefault(x => x.BuocThucHienId.ToString() == i.BuocThucHienId);
                                    updateLuongPD.VaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == i.VaiTroId);
                                    updateLuongPD.TrangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => x.TrangThaiHoSoId.ToString() == i.TrangThaiHoSoId);
                                    updateLuongPD.TenBuoc = i.TenBuoc;
                                    updateLuongPD.ChuyenSangERP = i.ChuyenSangERP;
                                    updateLuongPD.CoThamTra = i.CoThamTra;
                                    updateLuongPD.ThuTu = i.ThuTu;
                                    updateLuongPD.DaXoa = 1;
                                }
                                else if (i.BuocThucHienId.ToUpper() != j.ToString().ToUpper() && !request.LuongPheDuyet.Any(x => x.BuocThucHienId.ToUpper() == j.ToString().ToUpper()))
                                {
                                    var deleteLuong = _context.LuongPheDuyet.FirstOrDefault(x => x.BuocThucHienId == j);
                                    deleteLuong.DaXoa = 0;
                                }
                            }
                        }
                        else
                        {
                            if (i.BuocThucHienId == "")
                            {
                                if (i.VaiTroId != null)
                                {
                                    var vaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == i.VaiTroId);
                                    if (vaiTro == null)
                                    {
                                        _context.VaiTro.Add(
                                        new VaiTro
                                        {
                                            VaiTroId = new Guid(i.VaiTroId),
                                            TenVaiTro = _context.ApplicationRole.FirstOrDefault(x => x.Id == i.VaiTroId).Name,
                                        }
                                     );
                                        _context.SaveChanges();
                                    }
                                }
                                _context.LuongPheDuyet.Add(

                                    new LuongPheDuyet
                                    {
                                        BuocThucHienId = Guid.NewGuid(),
                                        VaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == i.VaiTroId),
                                        TrangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => x.TrangThaiHoSoId.ToString() == i.TrangThaiHoSoId),
                                        LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == Item.LoaiHoSoId),
                                        TenBuoc = i.TenBuoc,
                                        ChuyenSangERP = i.ChuyenSangERP,
                                        CoThamTra = i.CoThamTra,
                                        ThuTu = i.ThuTu,
                                        DaXoa = 1
                                    }
                                );
                            }
                        }
                    }
                }

                _context.SaveChanges();
                // }
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePostViewModel DeleteLoaiHoSo(Guid id)
        {
            try
            {
                var Item = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == id);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy báo cáo", 400);
                }
                // trạng thái: 1 => còn hiệu lực
                // trạng thái: 0 => hết hiệu lực
                if (Item.TrangThai == 1)
                {
                    return new ResponsePostViewModel("Loại hồ sơ này đang ở trạng thái còn hiệu lực, không thể xóa", 400);
                }
                var checkHoSoTT = _context.HoSoThanhToan.FirstOrDefault(x => x.LoaiHoSo.LoaiHoSoId == id);
                if (checkHoSoTT != null)
                {
                    return new ResponsePostViewModel("Loại hồ sơ này có chứa hồ sơ thanh toán, không thể xóa", 400);
                }
                var checkLuongPheDuyet = _context.LuongPheDuyet.FirstOrDefault(x => x.LoaiHoSo.LoaiHoSoId == id);
                if (checkLuongPheDuyet != null)
                {
                    return new ResponsePostViewModel("Loại hồ sơ này đã được cấu hình luồng phê duyệt, không thể xóa", 400);
                }
                _context.LoaiHoSo.Remove(Item);
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public List<LoaiHoSoViewModel> GetLoaiHoSo(int trangThai)
        {
            try
            {
                var loaiHoaSo = from bc in _context.LoaiHoSo
                           select new LoaiHoSoViewModel
                           {
                               MaLoaiHoSo = bc.MaLoaiHoSo,
                               LoaiHoSoId = bc.LoaiHoSoId,
                               TenLoaiHoSo = bc.TenLoaiHoSo,
                               NgayTao = bc.NgayTao,
                               TrangThai = bc.TrangThai,
                               QuyTrinhPheDuyetId = _context.QuyTrinhPheDuyet.FirstOrDefault(x => x.LoaiHoSoId == bc.LoaiHoSoId).QuyTrinhPheDuyetId
                           };

                loaiHoaSo = loaiHoaSo.OrderBy(x => x.TenLoaiHoSo);
                if (loaiHoaSo.Any())
                {
                    // trang thái 1 có hiệu lực, trạng thái 2 không có hiệu lực
                    if (trangThai == 3)
                    {
                        loaiHoaSo = loaiHoaSo.Where(x => x.TrangThai == 1 || x.TrangThai == 2);
                    }
                    else
                    {
                        loaiHoaSo = loaiHoaSo.Where(x => x.TrangThai == trangThai);
                    }
                }

                return loaiHoaSo.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<LoaiHoSoViewModel> GetLoaiHoSoById(Guid id)
        {
            try
            {
                var list = from bc in _context.LoaiHoSo
                           where bc.LoaiHoSoId == id
                           select new LoaiHoSoViewModel
                           {
                               LoaiHoSoId = bc.LoaiHoSoId,
                               MaLoaiHoSo = bc.MaLoaiHoSo,
                               TenLoaiHoSo = bc.TenLoaiHoSo,
                               NgayTao = bc.NgayTao,
                               MoTa = bc.MoTa,
                               TrangThai = bc.TrangThai,
                               GiayToLoaiHoSoResponse = (from giayToLHS in _context.GiayToLoaiHoSo
                                                         where giayToLHS.LoaiHoSo.LoaiHoSoId == id
                                                         orderby giayToLHS.ThuTu
                                                         select new GiayToLoaiHoSoResponse
                                                         {
                                                             GiayToLoaiHoSoId = giayToLHS.GiayToLoaiHoSoId.ToString().ToLower(),
                                                             GiayToId = giayToLHS.GiayTo.GiayToId.ToString().ToLower(),
                                                             TenGiayTo = giayToLHS.GiayTo.TenGiayTo,
                                                             MoTa = giayToLHS.MoTa,
                                                             BatBuoc = giayToLHS.BatBuoc,
                                                             Nguon = giayToLHS.Nguon,
                                                             PheDuyetKySo = giayToLHS.PheDuyetKySo,
                                                             ThuTu = giayToLHS.ThuTu
                                                         }).ToList(),
                               LuongPheDuyetResponse = (from luongPD in _context.LuongPheDuyet
                                                        where luongPD.LoaiHoSo.LoaiHoSoId == id
                                                        where luongPD.DaXoa == 1
                                                        orderby luongPD.ThuTu
                                                        select new LuongPheDuyetResponse
                                                        {
                                                            BuocThucHienId = luongPD.BuocThucHienId.ToString().ToLower(),
                                                            VaiTroId = luongPD.VaiTro.VaiTroId.ToString().ToLower(),
                                                            TrangThaiHoSoId = luongPD.TrangThaiHoSo.TrangThaiHoSoId.ToString().ToLower(),
                                                            TenTrangThaiHoSo = luongPD.TrangThaiHoSo.TenTrangThaiHoSo,
                                                            TenBuoc = luongPD.TenBuoc,
                                                            ChuyenSangERP = luongPD.ChuyenSangERP,
                                                            CoThamTra = luongPD.CoThamTra,
                                                            ThuTu = luongPD.ThuTu
                                                        }).OrderBy(x => x.ThuTu).ToList()
                           };

                list = list.OrderBy(x => x.TenLoaiHoSo);

                return list.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponseLoaiHoSoViewModel GetPagingLoaiHoSo(LoaiHoSoSearchViewModel request)
        {
            try
            {
                var loaiHoaSo = from lhs in _context.LoaiHoSo
                                select new LoaiHoSoViewModel
                                {
                                    MaLoaiHoSo = lhs.MaLoaiHoSo,
                                    LoaiHoSoId = lhs.LoaiHoSoId,
                                    TenLoaiHoSo = lhs.TenLoaiHoSo,
                                    NgayTao = lhs.NgayTao,
                                    MoTa = lhs.MoTa,
                                    TrangThai = lhs.TrangThai
                                };
                if (loaiHoaSo.Any())
                {
                    if (!String.IsNullOrEmpty(request.TuKhoa))
                    {
                        loaiHoaSo = loaiHoaSo.Where(x => x.TenLoaiHoSo.Contains(request.TuKhoa) || x.MaLoaiHoSo.Contains(request.TuKhoa));
                    }
                    if (request.trangThai == 3)
                    {
                        loaiHoaSo = loaiHoaSo.Where(x => x.TrangThai == 1 || x.TrangThai == 2);
                    }
                    else
                    {
                        loaiHoaSo = loaiHoaSo.Where(x => x.TrangThai == request.trangThai);
                    }
                    loaiHoaSo = loaiHoaSo.OrderBy(x => x.TenLoaiHoSo);
                    var totalRecord = loaiHoaSo.ToList().Count();
                    if (request.PageIndex > 0)
                    {
                        loaiHoaSo = loaiHoaSo.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                    }
                    var loaiHoSoPaging = loaiHoaSo.ToList();
                    if (totalRecord == 0)
                    {
                        return new ResponseLoaiHoSoViewModel(null, 204, totalRecord);
                    }
                    return new ResponseLoaiHoSoViewModel(loaiHoSoPaging, 200, totalRecord);
                }
                else
                {
                    return new ResponseLoaiHoSoViewModel(null, 204, 0);
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public ResponsePostViewModel UpdateTrangThaiLoaiHoSo(string loaiHoSoId)
        {
            try
            {
                var loaiHoSoDetail = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId.ToString() == loaiHoSoId);
                if (loaiHoSoDetail == null)
                {
                    return new ResponsePostViewModel("không tìm thấy dữ liệu", 204);
                }
                if (loaiHoSoDetail.TrangThai == 1)
                {
                    loaiHoSoDetail.TrangThai = 2;
                }
                else if (loaiHoSoDetail.TrangThai == 2)
                {
                    loaiHoSoDetail.TrangThai = 1;
                }
                _context.SaveChanges();
                return new ResponsePostViewModel("chuyển đổi trạng thái thành công", 201);
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }
    }
}