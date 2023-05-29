using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace BCXN.Repositories
{
    public class LinhVucBaoCaoRepository : ILinhVucBaoCaoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LinhVucBaoCaoRepository> _logger;
        public LinhVucBaoCaoRepository(ApplicationDbContext context, ILogger<LinhVucBaoCaoRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public List<LinhVucBaoCaoViewModel> GetLinhVucBaoCao()
        {
            try
            {
                var listBC = from lvbc in _context.LinhVucBaoCao
                             select new LinhVucBaoCaoViewModel
                             {
                                 Id = lvbc.Id,
                                 TieuDe = lvbc.TieuDe,
                                 LinhVucChaId = lvbc.LinhVucChaId,
                                 TrangThaiHoatDong = lvbc.TrangThaiHoatDong,
                                 DaXoa = lvbc.DaXoa,
                                 ThoiGianTao = lvbc.ThoiGianTao,
                                 NguoiTaoId = lvbc.NguoiTaoId
                             };


                var response = listBC.ToList();

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponseLinhVucBaoCaoViewModel GetLinhVucWithPagination(ParamGetLinhVucBaoCaoViewModel request)
        {
            try
            {
                var listLVBC = from lvbc in _context.LinhVucBaoCao
                             select new LinhVucBaoCaoGetViewModel
                             {
                                 Id = lvbc.Id,
                                 TieuDe = lvbc.TieuDe,
                                 LinhVucChaId = lvbc.LinhVucChaId,
                                 LinhVucChaTieuDe = _context.LinhVucBaoCao.FirstOrDefault(s => s.Id == lvbc.LinhVucChaId).TieuDe,
                                 TrangThaiHoatDong = lvbc.TrangThaiHoatDong,
                                 DaXoa = lvbc.DaXoa,
                                 ThoiGianTao = lvbc.ThoiGianTao,
                                 NguoiTaoId = lvbc.NguoiTaoId
                             };
                if(request.TrangThai != null)
                {
                    listLVBC = listLVBC.Where(s => s.TrangThaiHoatDong == request.TrangThai);
                }
                if (!String.IsNullOrEmpty(request.TenLV))
                {
                    listLVBC = listLVBC.Where(s => s.TieuDe.Contains(request.TenLV));
                }

                var totalRecord = listLVBC.ToList().Count();

                if(request.PageIndex > 0)
                {
                    listLVBC = listLVBC.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }

                var response = listLVBC.ToList();

                return new ResponseLinhVucBaoCaoViewModel(response,200, totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponsePostViewModel UpdateLinhVucBaoCao(LinhVucBaoCaoViewModel linhVuc)
        {
            try
            {
                var linhVucBaoCaoItem = _context.LinhVucBaoCao.FirstOrDefault(item => item.Id == linhVuc.Id);
                if(linhVucBaoCaoItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy lĩnh vực báo cáo", 404);
                }

                linhVucBaoCaoItem.TieuDe = linhVuc.TieuDe;
                linhVucBaoCaoItem.LinhVucChaId = linhVuc.LinhVucChaId;
                linhVucBaoCaoItem.TrangThaiHoatDong = linhVuc.TrangThaiHoatDong;
                linhVucBaoCaoItem.DaXoa = linhVuc.DaXoa;
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePostViewModel CreateLinhVucBaocao(LinhVucBaoCaoViewModel linhVuc)
        {
            try
            {
                var linhVucBaoCaoItem = new LinhVucBaoCao
                {
                    TieuDe = linhVuc.TieuDe,
                    LinhVucChaId = linhVuc.LinhVucChaId,
                    TrangThaiHoatDong = linhVuc.TrangThaiHoatDong,
                    DaXoa = linhVuc.DaXoa,
                    ThoiGianTao = DateTime.Now,
                    NguoiTaoId = linhVuc.NguoiTaoId
                };

                _context.LinhVucBaoCao.Add(linhVucBaoCaoItem);
                _context.SaveChanges();
                return new ResponsePostViewModel("Thêm lĩnh vực báo cáo thành công", 200);

            }catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePostViewModel DeleteLinhVucBaoCao(int id)
        {
            try
            {
                var linhVucBaoCaoItem = _context.LinhVucBaoCao.FirstOrDefault(item => item.Id == id);
                if(linhVucBaoCaoItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy lĩnh vực báo cáo", 404);
                }
                _context.LinhVucBaoCao.Remove(linhVucBaoCaoItem);
                _context.SaveChanges();
                return new ResponsePostViewModel("Xóa lĩnh vực báo cáo thành công", 200);
            }catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
    }
}
