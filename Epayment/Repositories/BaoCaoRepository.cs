using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace BCXN.Repositories
{
    public class BaoCaoRepository : IBaoCaoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BaoCaoRepository> _logger;
        public BaoCaoRepository(ApplicationDbContext context, ILogger<BaoCaoRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public ResponseBaoCaoViewModel GetBaoCao(ParamsGetBaoCaoViewModel request)
        {
            try
            {
                var listBC = from bc in _context.BaoCao
                             select new BaoCaoViewModel
                             {
                                 Id = bc.Id,
                                 LoaiBaoCao = bc.LoaiBaoCao,
                                 TypeId = bc.TypeId,
                                 SoLieuId = bc.SoLieuId,
                                 TenBaoCao = bc.TenBaoCao,
                                 UrlLink = bc.UrlLink,
                                 MoTa = bc.MoTa,
                                 TrangThai = bc.TrangThai,
                                 DonViId = bc.DonViId,
                                 TenDonVi = _context.DonVi.FirstOrDefault(s => s.Id == bc.DonViId).TenDonVi,
                                 LinhVucId = bc.LinhVucId,
                                 TenLinhVuc = _context.LinhVucBaoCao.FirstOrDefault(s => s.Id == bc.LinhVucId).TieuDe,
                                 ChuKy = bc.ChuKy,
                                 ViewName = bc.ViewName,
                                 ViewId = bc.ViewId,
                                 SiteName = bc.SiteName,
                                 SiteId = bc.SiteId,
                                 TrangThaiHoatDong = bc.TrangThaiHoatDong,
                                 ThoiGianTao = bc.ThoiGianTao,
                                 NguoiTaoId = bc.NguoiTaoId,
                                 ThoiGianCapNhat = bc.ThoiGianCapNhat,
                                 NguoiCapNhatId = bc.NguoiCapNhatId,
                                 Order = bc.Order
                             };

                if (request.LoaiBaoCao != null)
                {
                    listBC = listBC.Where(s => s.LoaiBaoCao == request.LoaiBaoCao);
                }
                if (request.TrangThai != null)
                {
                    listBC = listBC.Where(s => s.TrangThaiHoatDong == request.TrangThai);
                }

                if (request.LinhVucId != null)
                {
                    listBC = listBC.Where(s => s.LinhVucId == request.LinhVucId);
                }

                if (request.DonViId != null)
                {
                    listBC = listBC.Where(s => s.DonViId == request.DonViId);
                }

                if (request.TypeId != null)
                {
                    listBC = listBC.Where(s => s.TypeId == request.TypeId);
                }

                if (!String.IsNullOrEmpty(request.TenBC))
                {
                    listBC = listBC.Where(s => s.TenBaoCao.Contains(request.TenBC));
                }

                listBC = listBC.OrderBy(s => s.Order);

                var totalRecord = listBC.ToList().Count();
                if (request.PageIndex > 0)
                {
                    listBC = listBC.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }
                var response = listBC.ToList();

                return new ResponseBaoCaoViewModel(response, 200, totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }
        public ResponseBaoCaoViewModel GetBaoCaoTableau()
        {
            try
            {
                var listBC = from bc in _context.BaoCao
                             select new BaoCaoViewModel
                             {
                                 Id = bc.Id,
                                 LoaiBaoCao = bc.LoaiBaoCao,
                                 TypeId = bc.TypeId,
                                 SoLieuId = bc.SoLieuId,
                                 TenBaoCao = bc.TenBaoCao,
                                 UrlLink = bc.UrlLink,
                                 MoTa = bc.MoTa,
                                 TrangThai = bc.TrangThai,
                                 DonViId = bc.DonViId,
                                 TenDonVi = _context.DonVi.FirstOrDefault(s => s.Id == bc.DonViId).TenDonVi,
                                 LinhVucId = bc.LinhVucId,
                                 TenLinhVuc = _context.LinhVucBaoCao.FirstOrDefault(s => s.Id == bc.LinhVucId).TieuDe,
                                 ChuKy = bc.ChuKy,
                                 ViewName = bc.ViewName,
                                 ViewId = bc.ViewId,
                                 SiteName = bc.SiteName,
                                 SiteId = bc.SiteId,
                                 TrangThaiHoatDong = bc.TrangThaiHoatDong,
                                 ThoiGianTao = bc.ThoiGianTao,
                                 NguoiTaoId = bc.NguoiTaoId,
                                 ThoiGianCapNhat = bc.ThoiGianCapNhat,
                                 NguoiCapNhatId = bc.NguoiCapNhatId,
                                 Order = bc.Order
                             };

                listBC = listBC.Where(s => s.TypeId == 1);//Lay bao cao tu dong tu BI
                var response = listBC.ToList();
                return new ResponseBaoCaoViewModel(response, 200, 0);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<GetBaoCaoByLinhVuc> GetBaoCaoByLinhVuc()
        {
            try
            {
                var listBCByLinhVuc = from lv in _context.LinhVucBaoCao
                                      select new GetBaoCaoByLinhVuc
                                      {
                                          Id = lv.Id,
                                          TieuDe = lv.TieuDe,
                                          LinhVucChaId = lv.LinhVucChaId,
                                          TrangThaiHoatDong = lv.TrangThaiHoatDong,
                                          BaoCao = (from bcByLv in _context.BaoCao
                                                    where bcByLv.LinhVucId == lv.Id
                                                    where bcByLv.TypeId == 1
                                                    select new BaoCaoViewModel
                                                    {
                                                        Id = bcByLv.Id,
                                                        Order = bcByLv.Order,
                                                        LoaiBaoCao = bcByLv.LoaiBaoCao,
                                                        TypeId = bcByLv.TypeId,
                                                        TenBaoCao = bcByLv.TenBaoCao,
                                                        UrlLink = bcByLv.UrlLink,
                                                        MoTa = bcByLv.MoTa,
                                                        TrangThai = bcByLv.TrangThai,
                                                        DonViId = bcByLv.DonViId,
                                                        TenDonVi = _context.DonVi.FirstOrDefault(x => x.Id == bcByLv.DonViId).TenDonVi,
                                                        LinhVucId = bcByLv.LinhVucId,
                                                        TenLinhVuc = _context.LinhVucBaoCao.FirstOrDefault(x => x.Id == bcByLv.LinhVucId).TieuDe,
                                                        ChuKy = bcByLv.ChuKy,
                                                        ViewName = bcByLv.ViewName,
                                                        ViewId = bcByLv.ViewId,
                                                        SiteId = bcByLv.SiteId,
                                                        TrangThaiHoatDong = bcByLv.TrangThaiHoatDong,
                                                    }).OrderBy(x => x.Order).ToList()
                                      };

                return listBCByLinhVuc.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }
        public List<BaoCaoViewModel> GetBaoCaoByYeuCauBC(int ycbcId, int typeId)
        {
            try
            {
                var listBCByYeuCauBC = from bc in _context.BaoCao
                                       join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                       where xnbc.YeuCauBaoCaoId == ycbcId
                                       //    where bc.TypeId == typeId
                                       select new BaoCaoViewModel
                                       {
                                           Id = bc.Id,
                                           LoaiBaoCao = bc.LoaiBaoCao,
                                           TypeId = bc.TypeId,
                                           SoLieuId = bc.SoLieuId,
                                           TenBaoCao = bc.TenBaoCao,
                                           UrlLink = bc.UrlLink,
                                           MoTa = bc.MoTa,
                                           TrangThai = bc.TrangThai,
                                           DonViId = bc.DonViId,
                                           TenDonVi = _context.DonVi.FirstOrDefault(x => x.Id == bc.DonViId).TenDonVi,
                                           LinhVucId = bc.LinhVucId,
                                           TenLinhVuc = _context.LinhVucBaoCao.FirstOrDefault(x => x.Id == bc.LinhVucId).TieuDe,
                                           ChuKy = bc.ChuKy,
                                           ViewName = bc.ViewName,
                                           ViewId = bc.ViewId,
                                           SiteId = bc.SiteId,
                                           TrangThaiHoatDong = bc.TrangThaiHoatDong,
                                           ThoiGianTao = bc.ThoiGianTao,
                                           NguoiTaoId = bc.NguoiTaoId,
                                           ThoiGianCapNhat = DateTime.Now,
                                           NguoiCapNhatId = bc.NguoiCapNhatId,
                                           GhiChu = xnbc.GhiChu,
                                           TrangThaiDuLieu = xnbc.TrangThaiDuLieu,
                                           TrangThaiXacNhan = xnbc.TrangThaiXacNhan,
                                           ThoiGianXacNhan = xnbc.ThoiGianXacNhan,
                                           NguoiTaoXNId = xnbc.NguoiTaoId,
                                           ThoiGianXNTao = xnbc.ThoiGianTao,
                                           SapXep = xnbc.SapXep,
                                           FileBaoCao = xnbc.FileBaoCao,
                                           ThoiGianCapNhatDuLieu = xnbc.ThoiGianCapNhatDuLieu,
                                           Order = bc.Order
                                       };
                if (typeId > 0)
                {
                    listBCByYeuCauBC = listBCByYeuCauBC.Where(x => x.TypeId == typeId);
                }

                return listBCByYeuCauBC.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }
        public ResponsePostViewModel UpdateBaoCaoAuto(int BaoCaoId, string filename)
        {
            try
            {
                var listBCByYeuCauBC = from xnbc in _context.XacNhanBaoCao
                                       join ycbc in _context.YeuCauBaoCao on xnbc.YeuCauBaoCaoId equals ycbc.Id
                                       where ycbc.TrangThai == 2 && ycbc.HanHoanThanh.Value >= DateTime.Now.Date && xnbc.BaoCaoId == BaoCaoId
                                       //&& (xnbc.TrangThaiXacNhan == 0 || (xnbc.TrangThaiXacNhan == 1 && xnbc.TrangThaiDuLieu == 0))

                                       //    where bc.TypeId == typeId
                                       select new XacNhanBaoCaoViewModel
                                       {
                                           Id = xnbc.Id,
                                           ThoiGianTao = xnbc.ThoiGianTao,
                                           NguoiTaoId = xnbc.NguoiTaoId,
                                           GhiChu = xnbc.GhiChu,
                                           TrangThaiDuLieu = xnbc.TrangThaiDuLieu,
                                           TrangThaiXacNhan = xnbc.TrangThaiXacNhan,
                                           ThoiGianXacNhan = xnbc.ThoiGianXacNhan,
                                           SapXep = xnbc.SapXep,
                                           FileBaoCao = xnbc.FileBaoCao,
                                       };
                IList<XacNhanBaoCaoViewModel> list = listBCByYeuCauBC.ToList();
                if (list != null && list.Count > 0)
                {
                    _logger.LogInformation("Bao cao " + BaoCaoId + " Khong co du lieu");
                    foreach (XacNhanBaoCaoViewModel xnbc in list)
                    {
                        var xn = _context.XacNhanBaoCao.Where(e => e.Id == xnbc.Id).FirstOrDefault();
                        xn.FileBaoCao = filename;
                        xn.TrangThaiDuLieu = null;
                        xn.TrangThaiXacNhan = null;
                        xn.ThoiGianXacNhan = null;
                        xn.ThoiGianCapNhatDuLieu = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
                return new ResponsePostViewModel("SUCCESS", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex.Message);
                return new ResponsePostViewModel("ERRORR: " + ex.Message, 200);

            }
        }
        

        public List<BaoCaoXNViewModel> GetBaoCaoXacNhan(int donViId, int ycbcId, int trangThai)
        {
            try
            {
                var listBCXN = from bc in _context.BaoCao
                               join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                               where xnbc.YeuCauBaoCaoId == ycbcId
                               //    where bc.TypeId == typeId
                               select new BaoCaoXNViewModel
                               {
                                   Id = xnbc.Id,
                                   BaoCaoId = bc.Id,
                                   YeuCauBaoCaoId = xnbc.YeuCauBaoCaoId,
                                   LoaiBaoCao = bc.LoaiBaoCao,
                                   TypeId = bc.TypeId,
                                   SoLieuId = bc.SoLieuId,
                                   TenBaoCao = bc.TenBaoCao,
                                   UrlLink = bc.UrlLink,
                                   MoTa = bc.MoTa,
                                   TrangThai = bc.TrangThai,
                                   DonViId = bc.DonViId,
                                   TenDonVi = _context.DonVi.FirstOrDefault(x => x.Id == bc.DonViId).TenDonVi,
                                   LinhVucId = bc.LinhVucId,
                                   TenLinhVuc = _context.LinhVucBaoCao.FirstOrDefault(x => x.Id == bc.LinhVucId).TieuDe,
                                   ChuKy = bc.ChuKy,
                                   ViewName = bc.ViewName,
                                   ViewId = bc.ViewId,
                                   SiteId = bc.SiteId,
                                   SiteName = bc.SiteName,
                                   TrangThaiHoatDong = bc.TrangThaiHoatDong,
                                   ThoiGianTao = bc.ThoiGianTao,
                                   NguoiTaoId = bc.NguoiTaoId,
                                   TrangThaiDuLieu = xnbc.TrangThaiDuLieu,
                                   TrangThaiXacNhan = xnbc.TrangThaiXacNhan,
                                   ThoiGianXacNhan = xnbc.ThoiGianXacNhan,
                                   SapXep = xnbc.SapXep,
                                   FileBaoCao = xnbc.FileBaoCao,
                                   ThoiGianCapNhatDuLieu = xnbc.ThoiGianCapNhatDuLieu,
                               };
                if (donViId > 0)
                {
                    listBCXN = listBCXN.Where(x => x.DonViId == donViId);
                }
                if (trangThai > 0)
                {
                    if (trangThai == 2)
                    {
                        listBCXN = listBCXN.Where(x => x.TrangThaiXacNhan == null);
                    }
                    else
                    {
                        listBCXN = listBCXN.Where(x => x.TrangThaiXacNhan == trangThai);
                    }
                }
                listBCXN = listBCXN.OrderBy(x => x.SapXep);

                return listBCXN.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<ThongKeBCViewModel> ThongKeBC(int ycbcId)
        {
            try
            {

                var dsThongKeQuery = from dv in _context.DonVi
                                     join bc in _context.BaoCao on dv.Id equals bc.DonViId
                                     join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                     where xnbc.YeuCauBaoCaoId == ycbcId
                                     select new ThongKeBCViewModel
                                     {
                                         DonViId = dv.Id,
                                         TenDonVi = dv.TenDonVi,

                                     };
                dsThongKeQuery = dsThongKeQuery.Distinct();
                var dsThongKe = dsThongKeQuery.ToList();
                foreach (var item in dsThongKe)
                {
                    item.DaXacNhan = (from bc in _context.BaoCao
                                      join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                      where bc.DonViId == item.DonViId
                                      where xnbc.YeuCauBaoCaoId == ycbcId
                                      where xnbc.TrangThaiXacNhan == 1
                                      select bc).ToList().Count();
                    item.ChuaXacNhan = (from bc in _context.BaoCao
                                        join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                        where bc.DonViId == item.DonViId
                                        where xnbc.YeuCauBaoCaoId == ycbcId
                                        where xnbc.TrangThaiXacNhan == null
                                        select bc).ToList().Count();
                    item.SoLieuDung = (from bc in _context.BaoCao
                                       join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                       where bc.DonViId == item.DonViId
                                       where xnbc.YeuCauBaoCaoId == ycbcId
                                       where xnbc.TrangThaiXacNhan == 1
                                       where xnbc.TrangThaiDuLieu == 1
                                       select bc).ToList().Count();
                    item.SoLieuChuaDung = (from bc in _context.BaoCao
                                           join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                           where bc.DonViId == item.DonViId
                                           where xnbc.YeuCauBaoCaoId == ycbcId
                                           where xnbc.TrangThaiXacNhan == 1
                                           where xnbc.TrangThaiDuLieu == 2
                                           select bc).ToList().Count();
                }

                return dsThongKe;
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponsePostViewModel SapXepBaoCaoXacNhan(List<XacNhanBaoCaoViewModel> request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in request)
                    {
                        var xNBCItem = _context.XacNhanBaoCao.FirstOrDefault(x => x.Id == item.Id);
                        if (xNBCItem == null)
                        {
                            return new ResponsePostViewModel("Không tìm thấy xác nhận báo cáo", 400);
                        }

                        xNBCItem.SapXep = item.SapXep;

                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return new ResponsePostViewModel("Cập nhật thành công", 200);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", e);
                    return new ResponsePostViewModel(e.ToString(), 400);
                }
            }
        }

        public ResponsePostViewModel SapXepDanhMucBaoCao(List<BaoCaoViewModel> request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in request)
                    {
                        var bcItem = _context.BaoCao.FirstOrDefault(x => x.Id == item.Id);
                        if (bcItem == null)
                        {
                            return new ResponsePostViewModel("Không tìm thấy báo cáo", 400);
                        }

                        bcItem.Order = item.Order;

                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return new ResponsePostViewModel("Cập nhật thành công", 200);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", e);
                    return new ResponsePostViewModel(e.ToString(), 400);
                }
            }
        }

        public ResponsePostViewModel CreateBaoCao(BaoCao request)
        {
            try
            {
                _context.BaoCao.Add(
                    new BaoCao
                    {
                        LoaiBaoCao = request.LoaiBaoCao,
                        TypeId = request.TypeId,
                        SoLieuId = request.SoLieuId,
                        TenBaoCao = request.TenBaoCao,
                        UrlLink = request.UrlLink,
                        MoTa = request.MoTa,
                        TrangThai = request.TrangThai,
                        DonViId = request.DonViId,
                        LinhVucId = request.LinhVucId,
                        ChuKy = request.ChuKy,
                        ViewName = request.ViewName,
                        ViewId = request.ViewId,
                        SiteName = request.SiteName,
                        SiteId = request.SiteId,
                        TrangThaiHoatDong = request.TrangThaiHoatDong,
                        ThoiGianTao = DateTime.Now,
                        NguoiTaoId = request.NguoiTaoId,
                        Order = request.Order
                    }
                );
                _context.SaveChanges();
                return new ResponsePostViewModel("Thêm mới thành công", 200);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostViewModel(e.ToString(), 400);
            }

        }
        public ResponsePostViewModel UpdateBaoCao(int id, BaoCao request)
        {
            try
            {
                var baoCaoItem = _context.BaoCao.FirstOrDefault(x => x.Id == id);
                if (baoCaoItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy báo cáo", 400);
                }

                baoCaoItem.LoaiBaoCao = request.LoaiBaoCao;
                baoCaoItem.TypeId = request.TypeId;
                baoCaoItem.SoLieuId = request.SoLieuId;
                baoCaoItem.TenBaoCao = request.TenBaoCao;
                baoCaoItem.UrlLink = request.UrlLink;
                baoCaoItem.MoTa = request.MoTa;
                baoCaoItem.TrangThai = request.TrangThai;
                baoCaoItem.DonViId = request.DonViId;
                baoCaoItem.LinhVucId = request.LinhVucId;
                baoCaoItem.ChuKy = request.ChuKy;
                baoCaoItem.ViewName = request.ViewName;
                baoCaoItem.ViewId = request.ViewId;
                baoCaoItem.SiteName = request.SiteName;
                baoCaoItem.SiteId = request.SiteId;
                baoCaoItem.TrangThaiHoatDong = request.TrangThaiHoatDong;
                baoCaoItem.ThoiGianCapNhat = DateTime.Now;
                baoCaoItem.NguoiCapNhatId = request.NguoiCapNhatId;
                baoCaoItem.Order = request.Order;

                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePostViewModel DeleteBaoCao(int id)
        {
            try
            {
                var baoCaoItem = _context.BaoCao.FirstOrDefault(item => item.Id == id);
                if (baoCaoItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy báo cáo", 404);
                }
                _context.BaoCao.Remove(baoCaoItem);
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
    }
}
