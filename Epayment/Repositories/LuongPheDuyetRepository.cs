using System.Collections.Generic;
using BCXN.Data;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Models;
using System;
using Microsoft.AspNetCore.Identity;
using BCXN.Models;

namespace Epayment.Repositories
{
    public class LuongPheDuyetRepository : ILuongPheDuyetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LuongPheDuyetRepository> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        public LuongPheDuyetRepository(ApplicationDbContext context,
                                        ILogger<LuongPheDuyetRepository> logger,
                                        UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            this.userManager = userManager;
        }
        public ResponsePostViewModel deleteLuongPheDuyet(string LoaiHoSoId, string LuongPheDuyetId)
        {
            try
            {
                var luongPheDuyet = _context.LuongPheDuyet.FirstOrDefault(x => x.BuocThucHienId.ToString() == LuongPheDuyetId);
                luongPheDuyet.DaXoa = 0;
                _context.SaveChanges();
                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new ResponsePostViewModel(ex.Message, 500);
            }
        }

        public ResponseLuongPheDuyetViewModel getLuongPheDuyet(string LoaiHoSoId)
        {
            try
            {
                var luongpd = from lpd in _context.LuongPheDuyet
                              where lpd.LoaiHoSo.LoaiHoSoId.ToString() == LoaiHoSoId
                              where lpd.DaXoa == 1
                              select new LuongPheDuyetViewModel
                              {
                                  BuocThucHienId = lpd.BuocThucHienId,
                                  VaiTroId = _context.VaiTro.FirstOrDefault(x => x.VaiTroId == lpd.VaiTro.VaiTroId).VaiTroId,
                                  TenVaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId == lpd.VaiTro.VaiTroId).TenVaiTro,
                                  LoaiHoSoId = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == lpd.LoaiHoSo.LoaiHoSoId).LoaiHoSoId,
                                  TenLoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == lpd.LoaiHoSo.LoaiHoSoId).TenLoaiHoSo,
                                  TrangThaiHoSoId = _context.TrangThaiHoSo.FirstOrDefault(x => x.TrangThaiHoSoId == lpd.TrangThaiHoSo.TrangThaiHoSoId).TrangThaiHoSoId,
                                  TenTrangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => x.TrangThaiHoSoId == lpd.TrangThaiHoSo.TrangThaiHoSoId).TenTrangThaiHoSo,
                                  TenBuoc = lpd.TenBuoc,
                                  ChuyenSangERP = lpd.ChuyenSangERP,
                                  CoThamTra = lpd.CoThamTra,
                                  ThuTu = lpd.ThuTu
                              };
                var totalRecord = luongpd.ToList().Count();
                if (totalRecord == 0)
                {
                    return new ResponseLuongPheDuyetViewModel(null, 200, 0);
                }
                return new ResponseLuongPheDuyetViewModel(luongpd.ToList(), 200, totalRecord);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new ResponseLuongPheDuyetViewModel(null, 500, 0);
            }
        }

        // public ResponsePostViewModel updateLuongPheDuyet(UpdateLuongPheDuyet request)
        // {
        //     try
        //     {
        //         var luongPd = _context.LuongPheDuyet.FirstOrDefault(x => x.BuocThucHienId.ToString() == request.LuongPheDuyetId);
        //         if (luongPd == null)
        //         {
        //             return new ResponsePostViewModel("Không tìm thấy luồng cần phê duyệt", 204);
        //         }
        //         luongPd.VaiTro = _context.VaiTro.FirstOrDefault(x => x.VaiTroId.ToString() == request.VaiTroId);
        //         luongPd.TrangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => x.TrangThaiHoSoId.ToString() == request.TrangThaiHoSoId);
        //         luongPd.TenBuoc = request.TenBuoc;
        //         luongPd.ChuyenSangERP = request.ChuyenSangERP;
        //         luongPd.CoThamTra = request.CoThamTra;
        //         luongPd.ThuTu = request.ThuTu;
        //         _context.SaveChanges();
        //         return new ResponsePostViewModel("Cập nhật thành công", 200);
        //     }
        //     catch (System.Exception ex)
        //     {
        //         _logger.LogError("Lỗi:", ex);
        //         return new ResponsePostViewModel(ex.Message, 500);
        //     }
        // }
        public ResponsePostViewModel CreateLuongPheDuyet(List<CreateLuongPheDuyet> request)
        {
            try
            {
                foreach (var Item in request)
                {
                    var loaihoso = _context.LoaiHoSo.FirstOrDefault(x => (x.LoaiHoSoId.ToString() == Item.LoaiHoSoId));
                    if (loaihoso == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }
                    var trangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => (x.TrangThaiHoSoId.ToString() == Item.TrangThaiHoSoId));
                    if (trangThaiHoSo == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }
                    Item.VaiTroId = Guid.NewGuid().ToString();
                    _context.VaiTro.Add(
                        new VaiTro
                        {
                            VaiTroId = new Guid(Item.VaiTroId),
                            TenVaiTro = Item.TenVaiTro,
                        }
                    );
                    _context.SaveChanges();
                    var vaitro = _context.VaiTro.FirstOrDefault(x => (x.VaiTroId.ToString() == Item.VaiTroId));
                    if (vaitro == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }
                    _context.LuongPheDuyet.Add(
                        new LuongPheDuyet
                        {
                            BuocThucHienId = Guid.NewGuid(),
                            LoaiHoSo = loaihoso,
                            VaiTro = vaitro,
                            TrangThaiHoSo = trangThaiHoSo,
                            TenBuoc = Item.TenBuoc,
                            ChuyenSangERP = Item.ChuyenSangERP,
                            CoThamTra = Item.CoThamTra,
                            ThuTu = Item.ThuTu,
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
        public ResponsePostViewModel UpdateLuongPheDuyet(List<CreateLuongPheDuyet> request)
        {
            try
            {
                foreach (var Item in request)
                {
                    var lPD = _context.LuongPheDuyet.FirstOrDefault(x => x.BuocThucHienId.ToString() == Item.BuocThucHienId);
                    if (lPD == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }
                    var loaihoso = _context.LoaiHoSo.FirstOrDefault(x => (x.LoaiHoSoId.ToString() == Item.LoaiHoSoId));
                    if (loaihoso == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }
                    var trangThaiHoSo = _context.TrangThaiHoSo.FirstOrDefault(x => (x.TrangThaiHoSoId.ToString() == Item.TrangThaiHoSoId));
                    if (trangThaiHoSo == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }
                    var vaitro = _context.VaiTro.FirstOrDefault(x => (x.VaiTroId.ToString() == Item.VaiTroId));
                    if (vaitro == null)
                    {
                        return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                    }

                    // lPD.BuocThucHienId = new Guid(Item.BuocThucHienId);
                    lPD.LoaiHoSo = loaihoso;
                    lPD.VaiTro = vaitro;
                    lPD.TrangThaiHoSo = trangThaiHoSo;
                    lPD.TenBuoc = Item.TenBuoc;
                    lPD.ChuyenSangERP = Item.ChuyenSangERP;
                    lPD.CoThamTra = Item.CoThamTra;
                    lPD.ThuTu = Item.ThuTu;
                }

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