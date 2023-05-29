using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class NguoiHuongThuRepository : INguoiHuongThuRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NguoiHuongThuRepository> _logger;
        public NguoiHuongThuRepository(ApplicationDbContext context, ILogger<NguoiHuongThuRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public List<NguoiHuongThuViewModel> GetListNguoiHuongThu()
        {
            try
            {
                var list = from nht in _context.NguoiThuHuong
                               select new NguoiHuongThuViewModel
                               {
                                   NguoiThuHuongId = nht.NguoiThuHuongId,
                                   SoTKThuHuong = nht.SoTKThuHuong,
                                   TenNguoiThuHuong = nht.TenNguoiThuHuong,
                                   SoTienThanhToan = nht.SoTienThanhToan,
                                   HinhThucTT = nht.HinhThucTT
                               };
                
                list = list.OrderBy(x => x.SoTKThuHuong);

                return list.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        
    }
}