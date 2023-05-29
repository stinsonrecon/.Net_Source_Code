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
    public class YeuCauBaoCaoRepository : IYeuCauBaoCaoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<YeuCauBaoCaoRepository> _logger;
        public YeuCauBaoCaoRepository(ApplicationDbContext context, ILogger<YeuCauBaoCaoRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public ResponseYeuCauBaoCaoViewModel GetYeuCauBaoCao(ParamsGetYeuCauBaoCaoViewModel request)
        {
            try
            {
                var listYCBC = from ycbc in _context.YeuCauBaoCao
                               select new YeuCauBaoCaoViewModel
                               {
                                   TieuDe = ycbc.TieuDe,
                                   Id = ycbc.Id,
                                   ChuKy = ycbc.ChuKy,
                                   HanHoanThanh = ycbc.HanHoanThanh,
                                   MoTa = ycbc.MoTa,
                                   TrangThai = ycbc.TrangThai,
                                   ThoiGianTao = ycbc.ThoiGianTao,
                                   NguoiTaoId = ycbc.NguoiTaoId,
                                   DaXoa = ycbc.DaXoa,
                                   Nam = ycbc.Nam,
                                   LoaiChuKy = ycbc.LoaiChuKy,
                                   FileBaoCaoHieuChinh = ycbc.FileBaoCaoHieuChinh
                               };

                if (request.LoaiBaoCao != null)
                {
                    listYCBC = listYCBC.Where(s => s.LoaiChuKy == request.LoaiBaoCao);
                }
                if (request.Nam != null)
                {
                    listYCBC = listYCBC.Where(s => s.Nam == request.Nam);
                }
                if (request.TrangThai != null)
                {
                    listYCBC = listYCBC.Where(s => s.TrangThai == request.TrangThai);
                }
                if (request.DonViId != null)
                {
                    listYCBC = listYCBC.Where(s => s.TrangThai == 3 || s.TrangThai == 2 || s.TrangThai == 4);
                }
                listYCBC = listYCBC.OrderByDescending(x => x.ThoiGianTao);
                var dataHandle = listYCBC.ToList();

                foreach (var item in dataHandle)
                {
                    if (request.DonViId != null)
                    {
                        item.BaoCaoDaXN = ((from bc in _context.BaoCao
                                            join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                                            where xn.TrangThaiXacNhan == 1
                                            where xn.YeuCauBaoCaoId == item.Id
                                            where bc.DonViId == request.DonViId
                                            select bc).Count()
                                      + "/" +
                                      (from bc in _context.BaoCao
                                       join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                                       where xn.YeuCauBaoCaoId == item.Id
                                       where bc.DonViId == request.DonViId
                                       select bc).Count()).ToString();
                    }
                    else
                    {
                        item.BaoCaoDaXN = ((from bc in _context.BaoCao
                                            join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                                            where xn.TrangThaiXacNhan == 1
                                            where xn.YeuCauBaoCaoId == item.Id
                                            select bc).Count()
                                                          + "/" +
                                                          (from bc in _context.BaoCao
                                                           join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                                                           where xn.YeuCauBaoCaoId == item.Id
                                                           select bc).Count()).ToString();
                    }
                }

                var dataFilter = new List<YeuCauBaoCaoViewModel>();
                foreach (var item in dataHandle)
                {
                    if (item.BaoCaoDaXN != "0/0")
                    {
                        dataFilter.Add(item);
                    }
                }

                var totalRecord = dataFilter.Count();

                var response = new List<YeuCauBaoCaoViewModel>();
                if (request.PageIndex > 0)
                {
                    for (int i = 0; i < dataFilter.Count(); i++)
                    {
                        if (i >= request.PageSize * (request.PageIndex - 1) && i < (request.PageSize * (request.PageIndex - 1) + request.PageSize))
                        {
                            response.Add(dataFilter[i]);
                        }
                    }
                    // yeuCauBaoGiaList = yeuCauBaoGiaList.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);

                }

                return new ResponseYeuCauBaoCaoViewModel(response, 200, totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public YeuCauBaoCaoViewModel GetYeuCauBaoCaoById(int donViId, int ycbcId)
        {
            try
            {
                var listYCBC = from ycbc in _context.YeuCauBaoCao
                               where ycbc.Id == ycbcId
                               select new YeuCauBaoCaoViewModel
                               {
                                   TieuDe = ycbc.TieuDe,
                                   Id = ycbc.Id,
                                   ChuKy = ycbc.ChuKy,
                                   HanHoanThanh = ycbc.HanHoanThanh,
                                   MoTa = ycbc.MoTa,
                                   TrangThai = ycbc.TrangThai,
                                   ThoiGianTao = ycbc.ThoiGianTao,
                                   NguoiTaoId = ycbc.NguoiTaoId,
                                   DaXoa = ycbc.DaXoa,
                                   Nam = ycbc.Nam,
                                   LoaiChuKy = ycbc.LoaiChuKy,
                                   FileBaoCaoHieuChinh = ycbc.FileBaoCaoHieuChinh
                               };

                var itemYCBC = listYCBC.FirstOrDefault();

                if (donViId > 0)
                {
                    itemYCBC.BaoCaoDaXN = ((from bc in _context.BaoCao
                                            join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                                            where xn.TrangThaiXacNhan == 1
                                            where xn.YeuCauBaoCaoId == itemYCBC.Id
                                            where bc.DonViId == donViId
                                            select bc).Count()
                              + "/" +
                              (from bc in _context.BaoCao
                               join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                               where xn.YeuCauBaoCaoId == itemYCBC.Id
                               where bc.DonViId == donViId
                               select bc).Count()).ToString();
                }
                else
                {
                    itemYCBC.BaoCaoDaXN = ((from bc in _context.BaoCao
                                            join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                                            where xn.TrangThaiXacNhan == 1
                                            where xn.YeuCauBaoCaoId == itemYCBC.Id
                                            select bc).Count()
                              + "/" +
                              (from bc in _context.BaoCao
                               join xn in _context.XacNhanBaoCao on bc.Id equals xn.BaoCaoId
                               where xn.YeuCauBaoCaoId == itemYCBC.Id
                               select bc).Count()).ToString();
                }

                return itemYCBC;
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponsePostViewModel CreateYeuCauBaoCao(YeuCauBaoCaoCreateViewModel request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var yeuCauBC = new YeuCauBaoCao
                    {
                        TieuDe = request.TieuDe,
                        Nam = request.Nam,
                        ChuKy = request.ChuKy,
                        LoaiChuKy = request.LoaiChuKy,
                        HanHoanThanh = request.HanHoanThanh,
                        MoTa = request.MoTa,
                        TrangThai = request.TrangThai,
                        ThoiGianTao = DateTime.Now,
                        NguoiTaoId = request.NguoiTaoId,
                        DaXoa = 0
                    };
                    _context.YeuCauBaoCao.Add(
                       yeuCauBC
                    );

                    _context.SaveChanges();

                    foreach (var item in request.BaoCaoPhatSinh)
                    {
                        var baoCao = new BaoCao
                        {
                            LoaiBaoCao = item.LoaiBaoCao,
                            TypeId = 2,
                            SoLieuId = 2,
                            TenBaoCao = item.TenBaoCao,
                            UrlLink = item.UrlLink,
                            MoTa = item.MoTa,
                            TrangThai = item.TrangThai,
                            DonViId = item.DonViId,
                            LinhVucId = item.LinhVucId,
                            TrangThaiHoatDong = 1,
                            ThoiGianTao = DateTime.Now,
                            NguoiTaoId = item.NguoiTaoId
                        };
                        _context.BaoCao.Add(
                            baoCao
                        );
                        _context.SaveChanges();

                        var xacNhanBC = new XacNhanBaoCao
                        {
                            BaoCaoId = baoCao.Id,
                            YeuCauBaoCaoId = yeuCauBC.Id,
                            NguoiTaoId = item.NguoiTaoId,
                            ThoiGianTao = DateTime.Now,
                            FileBaoCao = "wwwroot/fileTableAu/template.pptx"
                        };
                        _context.XacNhanBaoCao.Add(
                            xacNhanBC
                        );
                        _context.SaveChanges();

                        var xacNhanBCItem = _context.XacNhanBaoCao.FirstOrDefault(x => x.BaoCaoId == baoCao.Id && x.YeuCauBaoCaoId == yeuCauBC.Id);
                        xacNhanBCItem.SapXep = xacNhanBC.Id;
                        _context.SaveChanges();

                    }

                    foreach (var item in request.BaoCaoDinhKy)
                    {
                        var xacNhanBC = new XacNhanBaoCao
                        {
                            BaoCaoId = item.Id,
                            YeuCauBaoCaoId = yeuCauBC.Id,
                            NguoiTaoId = item.NguoiTaoId,
                            ThoiGianTao = DateTime.Now,
                            FileBaoCao = "wwwroot/fileTableAu/template.pptx"
                        };
                        _context.XacNhanBaoCao.Add(
                            xacNhanBC
                        );
                        _context.SaveChanges();

                        var xacNhanBCItem = _context.XacNhanBaoCao.FirstOrDefault(x => x.BaoCaoId == item.Id && x.YeuCauBaoCaoId == yeuCauBC.Id);
                        xacNhanBCItem.SapXep = xacNhanBC.Id;
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return new ResponsePostViewModel("Thêm mới thành công", 200);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", e);
                    return new ResponsePostViewModel(e.ToString(), 400);
                }
            }
        }

        public Response UpdateFileBaoCaoTongHopHieuChinh(int ycbcID, string path)
        {
            try
            {
                var ycbc = _context.YeuCauBaoCao.FirstOrDefault(item => (item.Id == ycbcID));
                if (ycbc == null || ycbc.DaXoa == 1)
                {
                    _logger.LogError($"[UpdateFileBaoCaoTongHopHieuChinh] yêu cầu báo cáo không tồn tại hoặc đã xóa");
                    return new Response(message: "Lỗi: yêu cầu báo cáo không tồn tại hoặc đã xóa", data: "", errorcode: "001", success: false);
                }
                ycbc.FileBaoCaoHieuChinh = path;
                _context.SaveChanges();
                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[UpdateFileBaoCaoTongHopHieuChinh] Không thể cập nhật đường dẫn file báo cáo tổng hợp hiệu chỉnh {ex.StackTrace}");
                return new Response(message: "Không thể cập nhật đường dẫn file báo cáo tổng hợp hiệu chỉnh", data: ex.StackTrace, errorcode: "002", success: false);
            }
        }

        public Response GetFileBaoCaoTongHopHieuChinh(int ycbcID)
        {
            try
            {
                var ycbc = _context.YeuCauBaoCao.FirstOrDefault(item => (item.Id == ycbcID));
                if (ycbc == null || ycbc.DaXoa == 1)
                {
                    _logger.LogError($"[UpdateFileBaoCaoTongHopHieuChinh] yêu cầu báo cáo không tồn tại hoặc đã xóa");
                    return new Response(message: "Lỗi: yêu cầu báo cáo không tồn tại hoặc đã xóa", data: "", errorcode: "001", success: false);
                }
                return new Response(message: "Thành công", data: ycbc.FileBaoCaoHieuChinh, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[GetFileBaoCaoTongHopHieuChinh] Không thể lấy đường dẫn file báo cáo tổng hợp hiệu chỉnh {ex.StackTrace}");
                return new Response(message: "Không thể lấy đường dẫn file báo cáo tổng hợp hiệu chỉnh", data: ex.StackTrace, errorcode: "002", success: false);
            }
        }

        public ResponsePostViewModel UpdateYeuCauBaoCao(int ycbcId, YeuCauBaoCaoCreateViewModel request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var yeuCauBC = _context.YeuCauBaoCao.FirstOrDefault(x => x.Id == ycbcId);
                    if (yeuCauBC == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy yêu cầu báo cáo", 400);
                    }
                    yeuCauBC.Nam = request.Nam;
                    yeuCauBC.ChuKy = request.ChuKy;
                    yeuCauBC.LoaiChuKy = request.LoaiChuKy;
                    yeuCauBC.HanHoanThanh = request.HanHoanThanh;
                    yeuCauBC.MoTa = request.MoTa;
                    yeuCauBC.TrangThai = request.TrangThai;

                    _context.SaveChanges();



                    var listXNBCByYcbcId = _context.XacNhanBaoCao.Where(x => x.YeuCauBaoCaoId == ycbcId).ToList();
                    var listBCPS = (from bc in _context.BaoCao
                                    join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                    where xnbc.YeuCauBaoCaoId == ycbcId
                                    where bc.TypeId == 2
                                    select bc.Id).ToList();

                    foreach (var item in listBCPS)
                    {
                        if (request.BaoCaoPhatSinh.FirstOrDefault(x => x.Id == item) == null)
                        {
                            _context.BaoCao.RemoveRange(_context.BaoCao.Where(x => x.Id == item));
                            _context.SaveChanges();
                            _context.LichSuXacNhanBaoCao.RemoveRange(_context.LichSuXacNhanBaoCao.Where(x => x.BaoCaoId == item && x.YeuCauBaoCaoId == ycbcId));
                            _context.SaveChanges();
                        }
                    }

                    var listBCDK = (from bc in _context.BaoCao
                                    join xnbc in _context.XacNhanBaoCao on bc.Id equals xnbc.BaoCaoId
                                    where xnbc.YeuCauBaoCaoId == ycbcId
                                    where bc.TypeId == 1
                                    select bc.Id).ToList();

                    foreach (var item in listBCDK)
                    {
                        if (request.BaoCaoDinhKy.FirstOrDefault(x => x.Id == item) == null)
                        {
                            _context.LichSuXacNhanBaoCao.RemoveRange(_context.LichSuXacNhanBaoCao.Where(x => x.BaoCaoId == item && x.YeuCauBaoCaoId == ycbcId));
                            _context.SaveChanges();
                        }
                    }

                    _context.XacNhanBaoCao.RemoveRange(_context.XacNhanBaoCao.Where(x => x.YeuCauBaoCaoId == ycbcId));
                    _context.SaveChanges();

                    // foreach (var item in listBCPS)
                    // {
                    //     _context.BaoCao.RemoveRange(_context.BaoCao.Where(x => x.Id == item.BaoCaoId && x.TypeId == 2));
                    //     _context.SaveChanges();
                    // }

                    foreach (var item in request.BaoCaoPhatSinh)
                    {
                        if (item.Id == 0)
                        {
                            var baoCao = new BaoCao
                            {
                                LoaiBaoCao = item.LoaiBaoCao,
                                TypeId = 2,
                                SoLieuId = 2,
                                TenBaoCao = item.TenBaoCao,
                                MoTa = item.MoTa,
                                TrangThai = 1,
                                DonViId = item.DonViId,
                                LinhVucId = item.LinhVucId,
                                TrangThaiHoatDong = 1,
                                ThoiGianTao = DateTime.Now,
                                NguoiTaoId = item.NguoiTaoId
                            };
                            _context.BaoCao.Add(
                                baoCao
                            );
                            _context.SaveChanges();

                            var xacNhanBC = new XacNhanBaoCao
                            {
                                BaoCaoId = baoCao.Id,
                                YeuCauBaoCaoId = yeuCauBC.Id,
                                NguoiTaoId = item.NguoiTaoId,
                                GhiChu = item.GhiChu,
                                TrangThaiDuLieu = item.TrangThaiDuLieu,
                                TrangThaiXacNhan = item.TrangThaiXacNhan,
                                ThoiGianXacNhan = item.ThoiGianXacNhan,
                                ThoiGianTao = item.ThoiGianXNTao,
                                SapXep = item.SapXep,
                                FileBaoCao = "wwwroot/fileTableAu/template.pptx"
                            };
                            _context.XacNhanBaoCao.Add(
                                xacNhanBC
                            );
                            _context.SaveChanges();

                            var xacNhanBCItem = _context.XacNhanBaoCao.FirstOrDefault(x => x.BaoCaoId == baoCao.Id && x.YeuCauBaoCaoId == yeuCauBC.Id);
                            xacNhanBCItem.SapXep = xacNhanBC.Id;
                            _context.SaveChanges();
                        }
                        if (item.Id > 0)
                        {
                            var baoCaoItem = _context.BaoCao.FirstOrDefault(x => x.Id == item.Id);
                            if (baoCaoItem != null)
                            {
                                baoCaoItem.TenBaoCao = item.TenBaoCao;
                                baoCaoItem.DonViId = item.DonViId;
                                baoCaoItem.LinhVucId = item.LinhVucId;
                                _context.SaveChanges();
                            }

                            var xacNhanBC = new XacNhanBaoCao
                            {
                                BaoCaoId = baoCaoItem.Id,
                                YeuCauBaoCaoId = yeuCauBC.Id,
                                NguoiTaoId = item.NguoiTaoId,
                                GhiChu = item.GhiChu,
                                TrangThaiDuLieu = item.TrangThaiDuLieu,
                                TrangThaiXacNhan = item.TrangThaiXacNhan,
                                ThoiGianXacNhan = item.ThoiGianXacNhan,
                                ThoiGianTao = item.ThoiGianXNTao,
                                SapXep = item.SapXep,
                                FileBaoCao = item.FileBaoCao
                            };
                            _context.XacNhanBaoCao.Add(
                                xacNhanBC
                            );
                            _context.SaveChanges();

                            var xacNhanBCItem = _context.XacNhanBaoCao.FirstOrDefault(x => x.BaoCaoId == baoCaoItem.Id && x.YeuCauBaoCaoId == yeuCauBC.Id);
                            xacNhanBCItem.SapXep = xacNhanBC.Id;
                            _context.SaveChanges();
                        }
                    }

                    foreach (var item in request.BaoCaoDinhKy)
                    {
                        var xacNhanBC = new XacNhanBaoCao
                        {
                            BaoCaoId = item.Id,
                            YeuCauBaoCaoId = yeuCauBC.Id,
                            NguoiTaoId = item.NguoiTaoId,
                            GhiChu = item.GhiChu,
                            TrangThaiDuLieu = item.TrangThaiDuLieu,
                            TrangThaiXacNhan = item.TrangThaiXacNhan,
                            ThoiGianXacNhan = item.ThoiGianXacNhan,
                            ThoiGianTao = item.ThoiGianXNTao,
                            SapXep = item.SapXep,
                            FileBaoCao = (item.FileBaoCao != null && item.FileBaoCao != "") ? item.FileBaoCao : "wwwroot/fileTableAu/template.pptx"
                        };
                        _context.XacNhanBaoCao.Add(
                            xacNhanBC
                        );
                        _context.SaveChanges();

                        var xacNhanBCItem = _context.XacNhanBaoCao.FirstOrDefault(x => x.BaoCaoId == item.Id && x.YeuCauBaoCaoId == yeuCauBC.Id);
                        xacNhanBCItem.SapXep = xacNhanBC.Id;
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

        public ResponsePostViewModel DeleteYeuCauBaoCao(int ycbcId)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var yeuCauBC = _context.YeuCauBaoCao.FirstOrDefault(x => x.Id == ycbcId);
                    if (yeuCauBC == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy yêu cầu báo cáo", 400);
                    }
                    _context.YeuCauBaoCao.Remove(yeuCauBC);

                    _context.SaveChanges();

                    _context.XacNhanBaoCao.RemoveRange(_context.XacNhanBaoCao.Where(x => x.YeuCauBaoCaoId == ycbcId));
                    _context.SaveChanges();

                    var listBCPS = _context.XacNhanBaoCao.Where(x => x.YeuCauBaoCaoId == ycbcId).ToList();
                    foreach (var item in listBCPS)
                    {
                        _context.BaoCao.RemoveRange(_context.BaoCao.Where(x => x.Id == item.BaoCaoId && x.TypeId == 2));
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    return new ResponsePostViewModel("Xóa thành công", 200);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", e);
                    return new ResponsePostViewModel(e.ToString(), 400);
                }
            }
        }
        public ResponsePostViewModel XacNhanYeuCauBaoCao(int xnbcId, XacNhanBaoCaoViewModel request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var bcxn = _context.XacNhanBaoCao.FirstOrDefault(x => x.Id == xnbcId);
                    if (bcxn == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy báo cáo xác nhận", 400);
                    }
                    bcxn.TrangThaiDuLieu = request.TrangThaiDuLieu;
                    bcxn.TrangThaiXacNhan = 1;
                    bcxn.GhiChu = request.GhiChu;
                    bcxn.ThoiGianXacNhan = DateTime.Now;
                    if (request.FileBaoCao != null && request.FileBaoCao != "")
                        bcxn.FileBaoCao = request.FileBaoCao;
                    _context.SaveChanges();

                    _context.LichSuXacNhanBaoCao.Add(new LichSuXacNhanBaoCao
                    {
                        BaoCaoId = bcxn.BaoCaoId,
                        YeuCauBaoCaoId = bcxn.YeuCauBaoCaoId,
                        GhiChu = request.GhiChu,
                        TrangThaiDuLieu = request.TrangThaiDuLieu,
                        TrangThaiXacNhan = 1,
                        ThoiGianXacNhan = DateTime.Now,
                        NguoiTaoId = request.NguoiTaoId,
                        ThoiGianTao = DateTime.Now,
                        FileBaoCao = request.FileBaoCao
                    }); ;
                    _context.SaveChanges();
                    transaction.Commit();
                    return new ResponsePostViewModel("Xác nhận thành công", 200);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", e);
                    return new ResponsePostViewModel(e.ToString(), 400);
                }
            }
        }
        public ResponsePostViewModel UpdateXNBC(int xnbcId, XacNhanBaoCaoViewModel request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var bcxn = _context.XacNhanBaoCao.FirstOrDefault(x => x.Id == xnbcId);
                    if (bcxn == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy báo cáo xác nhận", 400);
                    }
                    bcxn.TrangThaiDuLieu =null;
                    bcxn.TrangThaiXacNhan = null;
                    bcxn.GhiChu = "";
                    bcxn.ThoiGianXacNhan =null;
                    bcxn.ThoiGianCapNhatDuLieu = DateTime.Now;
                    if (request.FileBaoCao != null && request.FileBaoCao != "")
                        bcxn.FileBaoCao = request.FileBaoCao;
                    _context.SaveChanges();

                    transaction.Commit();
                    return new ResponsePostViewModel("Xác nhận thành công", 200);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError("Lỗi:", e);
                    return new ResponsePostViewModel(e.ToString(), 400);
                }
            }
        }

        public ResponsePostViewModel UpdateTrangThai(int ycbcId, int trangThai)
        {
            try
            {
                var ycbc = _context.YeuCauBaoCao.FirstOrDefault(x => x.Id == ycbcId);
                if (ycbc == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy yêu cầu báo cáo", 400);
                }
                ycbc.TrangThai = trangThai;
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostViewModel(e.ToString(), 400);
            }
        }
    }
}
