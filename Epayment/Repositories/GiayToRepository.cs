using System;
using System.Collections.Generic;
using BCXN.Data;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using Epayment.ViewModels;
using Epayment.ModelRequest;

namespace BCXN.Repositories
{
    public class GiayToRepository : IGiayToRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GiayToRepository> _logger;
        public GiayToRepository(ApplicationDbContext context, ILogger<GiayToRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public ResponsePostViewModel CreateGiayTo(CreateGiayTo request)
        {
            try
            {
                var maGT = _context.GiayTo.FirstOrDefault(x => x.MaGiayTo.ToUpper() == request.MaGiayTo.ToUpper());
                if (maGT != null)
                {
                    return new ResponsePostViewModel("Đã tồn tại mã giấy tờ", 400);
                }
                _context.GiayTo.Add(
                    new GiayTo
                    {
                        GiayToId = new Guid(),
                        MaGiayTo = request.MaGiayTo,
                        TenGiayTo = request.TenGiayTo,
                        KySo = request.KySo,
                        Nguon = request.Nguon,
                        TrangThai = request.TrangThai,
                        NguoiTaoId = request.NguoiTaoId,
                        NgayTao = DateTime.Now,
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

        public ResponsePostViewModel UpdateGiayTo(CreateGiayTo request)
        {
            try
            {
                //var userId = _context.ApplicationUser.FirstOrDefault(x => x.Id == request.NguoiTaoId);
                var GiayToItem = _context.GiayTo.FirstOrDefault(x => x.GiayToId == request.GiayToId);
                if (GiayToItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                if (!GiayToItem.MaGiayTo.Equals(request.MaGiayTo))
                {
                    var maGT = _context.GiayTo.FirstOrDefault(x => x.MaGiayTo.ToUpper() == request.MaGiayTo.ToUpper());
                    if (maGT != null)
                    {
                        return new ResponsePostViewModel("Đã tồn tại mã giấy tờ", 400);
                    }
                }
                //GiayToItem.GiayToId = id;
                GiayToItem.MaGiayTo = request.MaGiayTo;
                GiayToItem.TenGiayTo = request.TenGiayTo;
                GiayToItem.KySo = request.KySo;
                GiayToItem.Nguon = request.Nguon;
                GiayToItem.TrangThai = request.TrangThai;
                GiayToItem.NguoiTaoId = request.NguoiTaoId;
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
        public ResponsePostViewModel DeleteGiayTo(Guid id)
        {
            try
            {
                var GiayToItem = _context.GiayTo.FirstOrDefault(x => x.GiayToId == id);
                if (GiayToItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                GiayToItem.TrangThai = -1;
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponseGiayToViewModel GetGiayTo(SearchGiayTo request)
        {
            try
            {
                var listGiayTo = from bc in _context.GiayTo
                                 where bc.TrangThai != -1
                                 select new GiayToViewModel
                                 {
                                     Id = bc.GiayToId,
                                     TenGiayTo = bc.TenGiayTo,
                                     MaGiayTo = bc.MaGiayTo,
                                     KySo = bc.KySo,
                                     Nguon = bc.Nguon,
                                     TrangThai = bc.TrangThai,
                                     NgayTao = bc.NgayTao,
                                     NguoiTaoId = bc.NguoiTaoId,
                                     TenNguoiTao = _context.Users.FirstOrDefault(x => x.Id == bc.NguoiTaoId).UserName,
                                 };
                if (listGiayTo.Any())
                {
                    if (request.TrangThai == 0)
                    {
                        listGiayTo = listGiayTo.Where(x => x.TrangThai == 1 || x.TrangThai == 2);
                    }
                    else
                    {
                        listGiayTo = listGiayTo.Where(x => x.TrangThai == request.TrangThai);
                    }
                    if (!String.IsNullOrEmpty(request.TuKhoa))
                    {
                        listGiayTo = listGiayTo.Where(x => x.MaGiayTo.Contains(request.TuKhoa) || x.TenGiayTo.Contains(request.TuKhoa));
                    }
                    if (request.TuNgay != DateTime.MinValue)
                    {
                        listGiayTo = listGiayTo.Where(x => x.NgayTao >= request.TuNgay);
                    }
                    if (request.DenNgay != DateTime.MinValue)
                    {
                        listGiayTo = listGiayTo.Where(x => x.NgayTao <= request.DenNgay);
                    }
                }

                listGiayTo = listGiayTo.OrderBy(x => x.TenGiayTo);
                var totalRecord = listGiayTo.ToList().Count();
                if (request.PageIndex > 0)
                {
                    listGiayTo = listGiayTo.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }
                var GiayToPaging = listGiayTo.ToList();
                if (totalRecord == 0)
                {
                    return new ResponseGiayToViewModel(null, 204, 0);
                }
                return new ResponseGiayToViewModel(GiayToPaging, 200, totalRecord);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<GiayToViewModel> GetGiayToById(Guid id)
        {
            try
            {
                var listGiayTo = from bc in _context.GiayTo
                                 where bc.GiayToId == id && bc.TrangThai != -1
                                 select new GiayToViewModel
                                 {
                                     Id = bc.GiayToId,
                                     TenGiayTo = bc.TenGiayTo,
                                     NgayTao = bc.NgayTao,
                                     MaGiayTo = bc.MaGiayTo,
                                     KySo = bc.KySo,
                                     Nguon = bc.Nguon,
                                     TrangThai = bc.TrangThai,
                                     NguoiTaoId = bc.NguoiTaoId,
                                     TenNguoiTao = _context.Users.FirstOrDefault(x => x.Id == bc.NguoiTaoId).UserName,
                                 };

                listGiayTo = listGiayTo.OrderBy(x => x.TenGiayTo);

                return listGiayTo.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<GiayToViewModel> GetGiayToByIdLoaiHoSo(Guid LoaiHoSoId)
        {
            try
            {
                var listGiayTo = from bc in _context.GiayTo
                                 join GTLH in _context.GiayToLoaiHoSo on bc.GiayToId equals GTLH.GiayTo.GiayToId
                                 where GTLH.LoaiHoSo.LoaiHoSoId == LoaiHoSoId && bc.TrangThai != -1
                                 select new GiayToViewModel
                                 {
                                     Id = bc.GiayToId,
                                     TenGiayTo = bc.TenGiayTo,
                                     MaGiayTo = bc.MaGiayTo,
                                     KySo = bc.KySo,
                                     Nguon = bc.Nguon,
                                     TrangThai = bc.TrangThai,
                                     NguoiTaoId = bc.NguoiTaoId,
                                     TenNguoiTao = _context.ApplicationUser.FirstOrDefault(x => x.Id == bc.NguoiTaoId).UserName,
                                     NgayTao = bc.NgayTao,
                                     BatBuoc = GTLH.BatBuoc,
                                     ThuTu = GTLH.ThuTu
                                 };

                //listGiayTo = listGiayTo.OrderBy(x => x.ThuTu);

                return listGiayTo.Distinct().OrderBy(x => x.ThuTu).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<GiayToViewModel> GetAllGiayTo()
        {
            try
            {
                var listGiayTo = from bc in _context.GiayTo
                                where bc.TrangThai != -1
                               select new GiayToViewModel
                               {
                                   Id = bc.GiayToId,
                                   TenGiayTo  = bc.TenGiayTo,
                                   MaGiayTo = bc.MaGiayTo,
                                   KySo = bc.KySo,
                                   Nguon = bc.Nguon,
                                   TrangThai = bc.TrangThai,
                                   NgayTao = bc.NgayTao,
                                   NguoiTaoId = bc.NguoiTaoId,
                                   TenNguoiTao = _context.Users.FirstOrDefault(x => x.Id == bc.NguoiTaoId).UserName,
                               };
                listGiayTo = listGiayTo.OrderBy(x => x.TenGiayTo);
                return listGiayTo.ToList();
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}