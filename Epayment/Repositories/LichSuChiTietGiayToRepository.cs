using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Data;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Epayment.Repositories
{
    public class LichSuChiTietGiayToRepository : ILichSuChiTietGiayToRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LichSuHoSoTTRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public LichSuChiTietGiayToRepository (ApplicationDbContext context,ILogger<LichSuHoSoTTRepository> logger,UserManager<ApplicationUser> userManager )
        {
            _context = context;
            _logger = logger;
            _userManager =userManager;
        }

        public async Task<ResponsePostViewModel> CreateLichSuChiTietGiayToHSTT(ParmChiTietGiayToHSTTViewModel request, List<string> url)
        {
            try
            {
                var Hstt = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoThanhToanId);
                if (Hstt == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var giayTo = _context.GiayTo.FirstOrDefault(x => (x.GiayToId.ToString() == request.GiayToId));
                var account = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiCapNhatId));

                LichSuChiTietGiayTo tam = new LichSuChiTietGiayTo() { };
                tam.ChiTietHoSoId = Guid.NewGuid();
                tam.HoSoThanhToan = Hstt;
                tam.GiayTo = giayTo;
                tam.TrangThaiGiayTo = request.TrangThaiGiayTo;
                tam.FileDinhKem = string.Join(",", url);
                tam.NgayCapNhat = DateTime.Now;
                tam.NguoiCapNhat = account;
                _context.LichSuChiTietGiayTo.Add(tam);
                _context.SaveChanges();
                return new ResponsePostViewModel("Thêm mới thành công", 200);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostViewModel(e.ToString(), 400);
            }
        }

        public ResponseLichSuChiTietGiayToHSTTViewModel GetLichSuChiTietGiayHSTTByHoSoId(string hoSoId, string giayToId)
        {
          try
            {
                var listItem = from cthstt in _context.LichSuChiTietGiayTo
                                join CTGT in _context.GiayTo on cthstt.GiayTo.GiayToId equals CTGT.GiayToId
                                where CTGT.TrangThai != -1
                                where cthstt.HoSoThanhToan.HoSoId.ToString() == hoSoId
                                where CTGT.GiayToId.ToString() == giayToId
                               select new LichSuChiTietGiayToHSTT
                               {
                                   //LichSuChiTietId = cthstt.ChiTietHoSoId,
                                   ChiTietHoSoId = cthstt.ChiTietHoSoId,
                                   IdHoSoTT = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == cthstt.HoSoThanhToan.HoSoId).HoSoId,
                                   TenHoSoTT = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == cthstt.HoSoThanhToan.HoSoId).TenHoSo,
                                   IdGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).GiayToId,
                                   TenGiayTo = _context.GiayTo.FirstOrDefault(x => x.GiayToId == cthstt.GiayTo.GiayToId).TenGiayTo,
                                   TrangThaiGiayTo = cthstt.TrangThaiGiayTo,
                                   FileDinhKem =  cthstt.FileDinhKem,
                                   NgayCapNhat = cthstt.NgayCapNhat,
                                   NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => x.Id == cthstt.NguoiCapNhat.Id).UserName,
                               };
                if (listItem.Count() > 0)
                {
                    return new ResponseLichSuChiTietGiayToHSTTViewModel(listItem.ToList(), 200,listItem.Count());
                }
                else
                {
                    return new ResponseLichSuChiTietGiayToHSTTViewModel(null, 204, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }  
        }
    }
}