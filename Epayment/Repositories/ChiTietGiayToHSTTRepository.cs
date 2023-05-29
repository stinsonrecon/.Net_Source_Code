using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class ChiTietGiayToHSTTRepository : IChiTietGiayToHSTTRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChiTietGiayToHSTTRepository> _logger;
        private readonly IConfiguration _configuration;
        public ChiTietGiayToHSTTRepository(ApplicationDbContext context, ILogger<ChiTietGiayToHSTTRepository> logger, IConfiguration configuration)
        {
            _context = context;
            this._logger = logger;
            _configuration = configuration;
        }

        public ResponsePostHSTTViewModel CreateChiTietGiayToHSTT(ParmChiTietGiayToHSTTViewModel request, List<string> url)
        {
            try
            {
                var Hstt = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
                if (Hstt == null)
                {
                    return new ResponsePostHSTTViewModel("Không tìm thấy dữ liệu", 400, null);
                }
                var GT = _context.GiayTo.FirstOrDefault(x => (x.GiayToId.ToString() == request.GiayToId));
                if (GT == null)
                {
                    return new ResponsePostHSTTViewModel("Không tìm thấy dữ liệu", 400, null);
                }
                var NCN = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiCapNhatId));
                ChiTietGiayToHSTT tam = new ChiTietGiayToHSTT();
                // if (!GT.MaGiayTo.Equals(_configuration.GetSection("GiayToSetting:MaGiayTo").Value))
                // {
                    tam.ChiTietHoSoId = Guid.NewGuid();
                    tam.TrangThaiGiayTo = request.TrangThaiGiayTo;
                    tam.HoSoThanhToan = Hstt;
                    tam.GiayTo = GT;
                    tam.NguoiCapNhat = NCN;
                    tam.FileDinhKem = string.Join(",", url);
                    tam.NgayCapNhat = DateTime.Now;
                    _context.ChiTietGiayToHSTT.Add(tam);
                // }
                // if (_context.ChiTietGiayToHSTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == request.HoSoThanhToanId && x.GiayTo.MaGiayTo == _configuration.GetSection("GiayToSetting:MaGiayTo").Value) == null)
                // {
                //     ChiTietGiayToHSTT item = new ChiTietGiayToHSTT();
                //     var ToTrinh = _context.GiayTo.FirstOrDefault(x => (x.MaGiayTo == _configuration.GetSection("GiayToSetting:MaGiayTo").Value));
                //     item.ChiTietHoSoId = Guid.NewGuid();
                //     item.TrangThaiGiayTo = request.TrangThaiGiayTo;
                //     item.HoSoThanhToan = Hstt;
                //     item.GiayTo = ToTrinh;
                //     item.NguoiCapNhat = NCN;
                //     item.FileDinhKem = "";
                //     item.NgayCapNhat = DateTime.Now;
                //     _context.ChiTietGiayToHSTT.Add(item);
                // }
                _context.SaveChanges();
                return new ResponsePostHSTTViewModel("Thêm mới thành công", 200, tam.ChiTietHoSoId.ToString());

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostHSTTViewModel(e.ToString(), 400, null);
            }
        }

        public ResponsePostHSTTViewModel CreateToTrinhChiTietGiay(ParmChiTietGiayToHSTTViewModel request)
        {
            try
            {
                var Hstt = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
                if (Hstt == null)
                {
                    return new ResponsePostHSTTViewModel("Không tìm thấy dữ liệu", 400, null);
                }
                // var GT = _context.GiayTo.FirstOrDefault(x => (x.GiayToId.ToString() == request.GiayToId));
                // if (GT == null)
                // {
                //     return new ResponsePostHSTTViewModel("Không tìm thấy dữ liệu", 400, null);
                // }
                var NCN = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiCapNhatId));

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet.FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt có Id = {request.ThaoTacBuocPheDuyetId}");
                }

                ChiTietGiayToHSTT item = new ChiTietGiayToHSTT();
                //var toTrinh = _context.GiayTo.FirstOrDefault(x => (x.MaGiayTo == _configuration.GetSection("GiayToSetting:MaGiayTo").Value));
                var toTrinh = _context.GiayTo.FirstOrDefault(x => x.GiayToId == thaoTacBuocPheDuyet.GiayToId);
                item.ChiTietHoSoId = Guid.NewGuid();
                item.TrangThaiGiayTo = toTrinh.TrangThai;
                item.HoSoThanhToan = Hstt;
                item.GiayTo = toTrinh;
                item.NguoiCapNhat = NCN;
                item.FileDinhKem = "";
                item.NgayCapNhat = DateTime.Now;
                _context.ChiTietGiayToHSTT.Add(item);

                _context.SaveChanges();
                return new ResponsePostHSTTViewModel("Thêm mới thành công", 200, null);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostHSTTViewModel(e.ToString(), 400, null);
            }
        }
        public Response UpdateTaiLieuToTrinh(HoSoThanhToan hsttItem, string filepath, Guid thaoTacBuocPheDuyetId, string nguoiThucHienId)
        {
            try
            {
                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet.FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == thaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt có Id = {thaoTacBuocPheDuyetId}");
                }

                //var gtItem = _context.GiayTo.FirstOrDefault(x => (x.MaGiayTo == _configuration["GiayToSetting:MaGiayTo"]));
                var gtItem = _context.GiayTo.FirstOrDefault(x => x.GiayToId == thaoTacBuocPheDuyet.GiayToId);
                var ctgthsttItem = _context.ChiTietGiayToHSTT.FirstOrDefault(x => (x.HoSoThanhToan == hsttItem && x.GiayTo == gtItem));
                if(ctgthsttItem == null)
                {
                    ChiTietGiayToHSTT item = new ChiTietGiayToHSTT();
                    //var toTrinh = _context.GiayTo.FirstOrDefault(x => (x.MaGiayTo == _configuration.GetSection("GiayToSetting:MaGiayTo").Value));
                    var toTrinh = _context.GiayTo.FirstOrDefault(x => x.GiayToId == thaoTacBuocPheDuyet.GiayToId);
                    item.ChiTietHoSoId = Guid.NewGuid();
                    item.TrangThaiGiayTo = toTrinh.TrangThai;
                    item.HoSoThanhToan = hsttItem;
                    item.GiayTo = toTrinh;
                    item.NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => x.Id == nguoiThucHienId);
                    item.FileDinhKem = filepath;
                    item.NgayCapNhat = DateTime.Now;
                    _context.ChiTietGiayToHSTT.Add(item);
                }
                else
                {
                    ctgthsttItem.FileDinhKem = filepath;
                }
                _context.SaveChanges();
                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception e)
            {
                return new Response(message: "Lỗi", data: "Không thể cập nhật tài liệu", errorcode: "001", success: false);
            }
        }

        public ResponsePostViewModel UpdateChiTietGiayToHSTT(ParmChiTietGiayToHSTTViewModel request, List<string> url)
        {
            try
            {
                var Item = _context.ChiTietGiayToHSTT.FirstOrDefault(x => x.ChiTietHoSoId.ToString() == request.ChiTietHoSoId);

                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var Hstt = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
                if (Hstt == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var GT = _context.GiayTo.FirstOrDefault(x => (x.GiayToId.ToString() == request.GiayToId));
                if (GT == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var NCN = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiCapNhatId));
                Item.ChiTietHoSoId = new Guid(request.ChiTietHoSoId);
                Item.TrangThaiGiayTo = request.TrangThaiGiayTo;
                Item.HoSoThanhToan = Hstt;
                Item.GiayTo = GT;
                Item.NguoiCapNhat = NCN;
                Item.FileDinhKem = string.Join(",", url);
                Item.NgayCapNhat = DateTime.Now;
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostViewModel(e.ToString(), 400);
            }

        }

        public ResponsePostViewModel DeleteChiTietGiayToHSTT(Guid id)
        {
            try
            {
                var Item = _context.ChiTietGiayToHSTT.FirstOrDefault(x => x.ChiTietHoSoId == id);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                _context.ChiTietGiayToHSTT.Remove(Item);
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponseChiTietHSTTViewModel GetChiTietGiayToHSTT(ChiTietHSTTSearchViewModel request)
        {
            try
            {
                var ChiTietHSTTList = from cthstt in _context.ChiTietGiayToHSTT
                                      join htt in _context.HoSoThanhToan on cthstt.HoSoThanhToan.HoSoId equals htt.HoSoId
                                      //join LHS in _context.LoaiHoSo on htt.LoaiHoSo.LoaiHoSoId  equals LHS.LoaiHoSoId into emp
                                      where cthstt.TrangThaiGiayTo != -1 && cthstt.HoSoThanhToan.HoSoId == request.idHSTT
                                      select new ChiTietGiayToHSTTViewModel
                                      {
                                          ChiTietHoSoId = cthstt.ChiTietHoSoId,
                                          IdHoSoTT = htt.HoSoId,
                                          LoaiHoSoId = htt.LoaiHoSo.LoaiHoSoId,
                                          TenHoSoTT = htt.TenHoSo,
                                          IdGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).GiayToId,
                                          TenGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).TenGiayTo,
                                          Nguon = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).Nguon,
                                          TrangThaiGiayTo = cthstt.TrangThaiGiayTo,
                                          MaGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).MaGiayTo,
                                          FileDinhKem = cthstt.FileDinhKem,
                                          NgayCapNhat = cthstt.NgayCapNhat,
                                          NguoiCapNhat = cthstt.NguoiCapNhat.FirstName + " " + cthstt.NguoiCapNhat.LastName,
                                          ThuTu = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == cthstt.GiayTo.GiayToId && x.LoaiHoSo.LoaiHoSoId == htt.LoaiHoSo.LoaiHoSoId).ThuTu,
                                          BatBuoc = _context.GiayToLoaiHoSo.FirstOrDefault(x => x.GiayTo.GiayToId == cthstt.GiayTo.GiayToId && x.LoaiHoSo.LoaiHoSoId == htt.LoaiHoSo.LoaiHoSoId).BatBuoc
                                      };
                if (ChiTietHSTTList.Any())
                {
                    if (request.idHSTT != null)
                    {
                        ChiTietHSTTList = ChiTietHSTTList.Where(s => s.IdHoSoTT == request.idHSTT);
                    }
                    ChiTietHSTTList = ChiTietHSTTList.OrderBy(x => x.NgayCapNhat);

                    var totalRecord = ChiTietHSTTList.ToList().Count();
                    if (request.PageIndex > 0)
                    {
                        ChiTietHSTTList = ChiTietHSTTList.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                    }
                    var chiTietPaging = ChiTietHSTTList.OrderBy(x => x.ThuTu).ToList();
                    if (totalRecord == 0)
                    {
                        return new ResponseChiTietHSTTViewModel(chiTietPaging, 204, totalRecord);
                    }
                    return new ResponseChiTietHSTTViewModel(chiTietPaging, 200, totalRecord);
                }
                else
                {
                    return new ResponseChiTietHSTTViewModel(null, 204, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }
        public ResponsePostViewModel DeleteChiTietGiayToByIdHSTT(Guid HoSoTTId)
        {
            try
            {
                var Item = from CTGT in _context.ChiTietGiayToHSTT
                           join GT in _context.GiayTo on CTGT.GiayTo.GiayToId equals GT.GiayToId
                           where CTGT.HoSoThanhToan.HoSoId == HoSoTTId && GT.MaGiayTo.ToLower() != _configuration.GetSection("GiayToSetting:MaGiayTo").Value.ToLower()
                           select CTGT;
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                foreach (var removeItem in Item)
                {
                    _context.ChiTietGiayToHSTT.Remove(removeItem);
                }
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
        public Response CreatePhieuThamTraHSTT(ParmPhieuThamTraHSTTViewModel request, string url)
        {
            try
            {
                var Hstt = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
                if (Hstt == null)
                {
                    return new Response(message:"Không tìm thấy dữ liệu hô sơ thanh toán",data: null, errorcode: "05" , success: false);
                }

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet.FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt có Id = {request.ThaoTacBuocPheDuyetId}");
                }

                var GT = _context.GiayTo.FirstOrDefault(x => x.GiayToId == thaoTacBuocPheDuyet.GiayToId);
                if (GT == null)
                {
                    return new Response(message:"Không tìm thấy dữ liệu giấy tờ",data: null, errorcode: "06" , success: false);
                }
                var NCN = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiCapNhatId));

                ChiTietGiayToHSTT tam = new ChiTietGiayToHSTT();
                tam.ChiTietHoSoId = Guid.NewGuid();
                tam.TrangThaiGiayTo = GT.TrangThai;
                tam.HoSoThanhToan = Hstt;
                tam.GiayTo = GT;
                tam.NguoiCapNhat = NCN;
                tam.FileDinhKem = string.Join(",", url);
                tam.NgayCapNhat = DateTime.Now;
                _context.ChiTietGiayToHSTT.Add(tam);
                _context.SaveChanges();
                return new Response(message:"Thêm mới thành công",data: null, errorcode: "00" , success: true);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new Response(message:"lỗi lưu dữ liệu",data: null, errorcode: "07" , success: false);
            }
        }
    }
}