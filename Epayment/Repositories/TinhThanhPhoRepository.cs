using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class TinhThanhPhoRepository : ITinhThanhPhoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TinhThanhPhoRepository> _logger;
        public TinhThanhPhoRepository(ApplicationDbContext context, ILogger<TinhThanhPhoRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }
        public ResponsePostViewModel CreateTinhThanhPho(TinhThanhPhoViewModel tinhTP)
        {
            try
            {
                var quocGiaItem = _context.QuocGia.FirstOrDefault(
                    x => x.QuocGiaId == tinhTP.QuocGia.QuocGiaId
                );

                if(quocGiaItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy quốc gia", 400);
                }

                var tinhTpItem = new TinhThanhPho
                {
                    QuocGia = quocGiaItem,
                    MaTinhTp = tinhTP.MaTinhTp,
                    TenTinhTp = tinhTP.TenTinhTp,
                    TrangThai = tinhTP.TrangThai,
                    DaXoa = tinhTP.DaXoa
                };

                _context.TinhThanhPho.Add(tinhTpItem);
                _context.SaveChanges();

                return new ResponsePostViewModel("Thêm tỉnh thành phố thành công", 200);
            }
            catch(Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePostViewModel DeleteTinhThanhPho(Guid id)
        {
            try
            {
                var tinhTpItem = _context.TinhThanhPho.FirstOrDefault(item => item.TinhTpId == id);

                if(tinhTpItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy tỉnh thành phố", 400);
                }

                _context.TinhThanhPho.Remove(tinhTpItem);
                _context.SaveChanges();

                return new ResponsePostViewModel("Xoá tỉnh thành phố thành công", 200);
            }
            catch(Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public List<TinhThanhPhoViewModel> GetTinhThanhPho()
        {
            try{
                var listTinhTP = from tinhTp in _context.TinhThanhPho
                                 select new TinhThanhPhoViewModel
                                 {
                                    TinhTpId = tinhTp.TinhTpId,
                                    QuocGia = tinhTp.QuocGia,
                                    MaTinhTp = tinhTp.MaTinhTp,
                                    TenTinhTp = tinhTp.TenTinhTp,
                                    TrangThai = tinhTp.TrangThai,
                                    DaXoa = tinhTp.DaXoa
                                 };
                
                var response = listTinhTP.ToList();

                return response;
            }
            catch(Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponseTinhThanhPhoViewModel GetTinhThanhPhoWithPagination(ParamGetTinhThanhPhoViewModel request)
        {
            try{
                var listTinhTP = from tinhTp in _context.TinhThanhPho
                                 select new TinhThanhPhoGetModel
                                 {
                                    TinhTpId = tinhTp.TinhTpId,
                                    TenQuocGia = _context.QuocGia.FirstOrDefault(qg => qg.QuocGiaId == tinhTp.QuocGia.QuocGiaId).TenQuocGia,
                                    MaQuocGia = _context.QuocGia.FirstOrDefault(qg => qg.QuocGiaId == tinhTp.QuocGia.QuocGiaId).MaQuocGia,
                                    MaTinhTp = tinhTp.MaTinhTp,
                                    TenTinhTp = tinhTp.TenTinhTp,
                                    TrangThai = tinhTp.TrangThai,
                                    DaXoa = tinhTp.DaXoa
                                 };
                
                if(request.TrangThai != null)
                {
                    listTinhTP = listTinhTP.Where(s => s.TrangThai == request.TrangThai);
                }
                if(!String.IsNullOrEmpty(request.TenTinhTP))
                {
                    listTinhTP = listTinhTP.Where(s => s.TenTinhTp.Contains(request.TenTinhTP));
                }

                var totalRecord = listTinhTP.ToList().Count();

                if(request.PageIndex > 0){
                    listTinhTP = listTinhTP.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }

                var response = listTinhTP.ToList();

                return new ResponseTinhThanhPhoViewModel(response, 200, totalRecord);
            }
            catch(Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponsePostViewModel UpdateTinhThanhPho(TinhThanhPhoViewModel tinhTP)
        {
            try
            {
                var tinhTpItem = _context.TinhThanhPho.FirstOrDefault(item => item.TinhTpId == tinhTP.TinhTpId);

                if(tinhTpItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy tỉnh thành phố", 400);
                }

                var quocGiaItem = _context.QuocGia.Where(
                    x => x.QuocGiaId == tinhTP.QuocGia.QuocGiaId
                ).FirstOrDefault();

                if(quocGiaItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy quốc gia", 400);
                }

                tinhTpItem.MaTinhTp = tinhTP.MaTinhTp;
                tinhTpItem.TenTinhTp = tinhTP.TenTinhTp;
                tinhTpItem.QuocGia = quocGiaItem;
                tinhTpItem.TrangThai = tinhTP.TrangThai;
                tinhTpItem.DaXoa = tinhTP.DaXoa;

                _context.SaveChanges();

                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch(Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
    }
}