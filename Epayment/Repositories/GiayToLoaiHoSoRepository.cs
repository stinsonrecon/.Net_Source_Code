using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class GiayToLoaiHoSoRepository: IGiayToLoaiHoSoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GiayToLoaiHoSoRepository> _logger;
        public GiayToLoaiHoSoRepository(ApplicationDbContext context, ILogger<GiayToLoaiHoSoRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public ResponsePostViewModel CreateGiayToLoaiHoSo(CreateGiayToLoaiHS request)
        {
            try
            {
                var loaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId.ToString() == request.LoaiHoSoId);
                var giayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId.ToString() == request.GiayToId);
                _context.GiayToLoaiHoSo.Add(
                    new GiayToLoaiHoSo
                    {
                        GiayToLoaiHoSoId = new Guid(),
                        LoaiHoSo = loaiHoSo,
                        GiayTo = giayTo,
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

        public ResponsePostViewModel DeleteGiayToLoaiHoSo(Guid id)
        {
            try
            {
                var Item = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayToLoaiHoSoId == id);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                _context.GiayToLoaiHoSo.Remove(Item);
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSo()
        {
            try
            {
                var list = from bc in _context.GiayToLoaiHoSo
                               select new GiayToLoaiHoSoViewModel
                               {
                                   giayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == bc.GiayTo.GiayToId).TenGiayTo,
                                   giayToId = _context.GiayTo.FirstOrDefault(x => x.GiayToId == bc.GiayTo.GiayToId).GiayToId,
                                   loaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == bc.LoaiHoSo.LoaiHoSoId).TenLoaiHoSo,
                                   loaiHoSoId = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == bc.LoaiHoSo.LoaiHoSoId).LoaiHoSoId,
                                   ngayTao = bc.NgayTao,
                               };
                return list.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSoById(Guid id)
        {
            try
            {
                var listGiayTo = from bc in _context.GiayToLoaiHoSo
                                where bc.GiayToLoaiHoSoId == id
                               select new GiayToLoaiHoSoViewModel
                               {
                                   giayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == bc.GiayTo.GiayToId).TenGiayTo,
                                   giayToId = _context.GiayTo.FirstOrDefault(x => x.GiayToId == bc.GiayTo.GiayToId).GiayToId,
                                   loaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == bc.LoaiHoSo.LoaiHoSoId).TenLoaiHoSo,
                                   loaiHoSoId = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == bc.LoaiHoSo.LoaiHoSoId).LoaiHoSoId,
                                   ngayTao = bc.NgayTao,
                               };
                
                // listGiayTo = listGiayTo.OrderBy(x => x.giayToLoaiHoSoId);

                return listGiayTo.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public ResponsePostViewModel UpdateGiayToLoaiHoSo(Guid id, UpdateGiayToLoaiHS request)
        {
            try
            {
                var Item = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayToLoaiHoSoId == id);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var loaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId.ToString() == request.LoaiHoSoId);
                var giayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId.ToString() == request.GiayToId);

                Item.LoaiHoSo = loaiHoSo;
                Item.GiayTo = giayTo;
                Item.NgayTao = DateTime.Now;
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
    }
}