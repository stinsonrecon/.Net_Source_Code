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
    public class LichSuXacNhanRepository : ILichSuXacNhanRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LichSuXacNhanRepository> _logger;
        public LichSuXacNhanRepository(ApplicationDbContext context, ILogger<LichSuXacNhanRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public List<LichSuXacNhanBaoCaoViewModel> GetLichSuXacNhan(int ycbcId, int bcId)
        {
            try
            {
                var listLSXN = from lsxn in _context.LichSuXacNhanBaoCao
                               where lsxn.BaoCaoId == bcId
                               where lsxn.YeuCauBaoCaoId == ycbcId
                               select new LichSuXacNhanBaoCaoViewModel
                               {
                                   Id = lsxn.Id,
                                   BaoCaoId = lsxn.BaoCaoId,
                                   YeuCauBaoCaoId = lsxn.YeuCauBaoCaoId,
                                   GhiChu = lsxn.GhiChu,
                                   TrangThaiDuLieu = lsxn.TrangThaiDuLieu,
                                   TrangThaiXacNhan = lsxn.TrangThaiXacNhan,
                                   ThoiGianXacNhan = lsxn.ThoiGianXacNhan,
                                   NguoiTaoId = lsxn.NguoiTaoId,
                                   TenNguoiTao = _context.ApplicationUser.FirstOrDefault(x => x.Id == lsxn.NguoiTaoId).UserName,
                                   ThoiGianTao = lsxn.ThoiGianTao,
                                   FileBaoCao = lsxn.FileBaoCao
                                   
                               };

                return listLSXN.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }
    }
}
