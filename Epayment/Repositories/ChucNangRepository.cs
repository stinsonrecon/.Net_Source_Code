using BCXN.Data;
using BCXN.Models;
using BCXN.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BCXN.Repositories
{
    public class ChucNangRepository : IChucNangRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChucNangRepository> _logger;
        public ChucNangRepository(ApplicationDbContext context, ILogger<ChucNangRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public ResponsePostViewModel Add(ChucNangGetViewModel chucNang)
        {
            try
            {
                var chucNangItem = new ChucNang
                {
                    TieuDe = chucNang.TieuDe,
                    ClaimValue = chucNang.ClaimValue,
                    ChucNangChaId = chucNang.ChucNangChaId,
                    MoTa = chucNang.MoTa,
                    TrangThai = chucNang.TrangThai,
                    LinkUrl = chucNang.LinkUrl,
                    Icon = chucNang.Icon,
                    Order = chucNang.Order,
                    Type = chucNang.Type,
                };
                _context.ChucNang.Add(chucNangItem);
                _context.SaveChanges();

                return new ResponsePostViewModel("Thêm chức năng mới thành công", 200);
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public ResponsePostViewModel Delete(int id)
        {
            try
            {
                var chucNangItem = _context.ChucNang.FirstOrDefault(s => s.Id == id);
                if (chucNangItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy chức năng", 404);
                }
                var chucNangChild = _context.ChucNang.FirstOrDefault(s => s.ChucNangChaId == chucNangItem.Id);
                if(chucNangChild != null)
                {
                    return new ResponsePostViewModel("Xóa không thành công", 500);
                }
                _context.ChucNang.Remove(chucNangItem);
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public ResponsePostViewModel Edit(ChucNangGetViewModel chucNang)
        {
            try
            {
                var chucNangItem = _context.ChucNang.FirstOrDefault(s => s.Id == chucNang.Id);

                if (chucNangItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy chức năng", 404);
                }
                chucNangItem.TieuDe = chucNang.TieuDe;
                chucNangItem.ClaimValue = chucNang.ClaimValue;
                chucNangItem.ChucNangChaId = chucNang.ChucNangChaId;
                chucNangItem.MoTa = chucNang.MoTa;
                chucNangItem.TrangThai = chucNang.TrangThai;
                chucNangItem.LinkUrl = chucNang.LinkUrl;
                chucNangItem.Icon = chucNang.Icon;
                chucNangItem.Order = chucNang.Order;
                chucNangItem.Type = chucNang.Type;
                _context.SaveChanges();

                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception ex)
            {
 
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId)
        {
            try
            {
                var chucNangList =
                from cn in _context.ChucNang
                join cnn in _context.ChucNangNhom on cn.Id equals cnn.ChucNangId
                join role in _context.ApplicationRole on cnn.NhomQuyenId equals role.Id
                where cn.TrangThai == 1
                //where cn.Type == 1
                orderby cn.Order
                select new ChucNangViewModel
                {
                    Id = cn.Id,
                    TieuDe = cn.TieuDe,
                    ChucNangChaId = cn.ChucNangChaId,
                    MoTa = cn.MoTa,
                    TrangThai = cn.TrangThai,
                    LinkUrl = cn.LinkUrl,
                    Icon = cn.Icon,
                    NhomQuyenId = role.Id,
                    Type = cn.Type,
                    ClaimValue = cn.ClaimValue
                };
                if (NhomQuyenId != null && NhomQuyenId != "-1")
                {
                    chucNangList = chucNangList.Where(x => x.NhomQuyenId == NhomQuyenId);
                }
                var chucNang = chucNangList.ToList();
                var chucNangMap = new List<ChucNangViewModel>();

                for (int i = 0; i < chucNang.Count(); i++)
                {
                    if (chucNang[i].ChucNangChaId == 0)
                    {
                        chucNangMap.Add(chucNang[i]);
                    }
                    for (int j = 0; j < chucNang.Count(); j++)
                    {
                        if (chucNang[j].ChucNangChaId == chucNang[i].Id)
                        {
                            chucNangMap.Add(chucNang[j]);
                        }
                    }
                }
                //for (int i = 0; i < chucNang.Count(); i++)
                //    if(chucNang[i].ChucNangChaId != 0)
                //        for (int j=0;j< chucNang.Count(); j++) if (chucNang[j].ChucNangChaId == chucNang[i].Id) chucNangMap.Add(chucNang[j]);               

                return chucNangMap;
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }

        public ResponseWithPaginationViewModel GetChucNang(ChucNangSearchViewModel request)
        {
            try
            {
                var chucNangList = from cn in _context.ChucNang
                                   select new ChucNangGetViewModel
                                   {
                                       Id = cn.Id,
                                       TieuDe = cn.TieuDe,
                                       ClaimValue = cn.ClaimValue,
                                       ChucNangChaId = cn.ChucNangChaId,
                                       ChucNangChaTieuDe = _context.ChucNang.FirstOrDefault(x => x.Id == cn.ChucNangChaId).TieuDe,
                                       MoTa = cn.MoTa,
                                       TrangThai = cn.TrangThai,
                                       LinkUrl = cn.LinkUrl,
                                       Icon = cn.Icon,
                                       Order = cn.Order,
                                       Type = cn.Type
                                   };
                if(request.Type != null)
                {
                    chucNangList = chucNangList.Where(s => s.Type == request.Type);
                }
                if (request.TrangThaiCN != null)
                {
                    chucNangList = chucNangList.Where(s => s.TrangThai == request.TrangThaiCN);
                }

                if (!String.IsNullOrEmpty(request.TenCN))
                {
                    chucNangList = chucNangList.Where(s => s.TieuDe.Contains(request.TenCN));
                }
                chucNangList = chucNangList.OrderBy(x => x.Order);


                var totalRecord = chucNangList.ToList().Count();
                if (request.PageIndex > 0)
                {
                    chucNangList = chucNangList.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }
                var chucNang = chucNangList.ToList();
                return new ResponseChucNangViewModel(chucNang, 200, totalRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }

    }
}