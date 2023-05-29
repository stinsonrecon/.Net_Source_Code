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
    public class LichSuHoSoTTRepository: ILichSuHoSoTTRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LichSuHoSoTTRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public LichSuHoSoTTRepository(ApplicationDbContext context, ILogger<LichSuHoSoTTRepository> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this._logger = logger;
            _userManager = userManager;
        }

        public async Task<ResponsePostViewModel> CreateLichSuHoSoTT(ParmHoSoThanhToanViewModel request)
        {
            try
            {
                var loaihoso = _context.LoaiHoSo.FirstOrDefault(x => (x.LoaiHoSoId == request.LoaiHoSoId));
                if (loaihoso == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var taiKhoanNH = (from tknh in _context.TaiKhoanNganHang
                                            join nh in _context.NganHang on tknh.NganHang.NganHangId equals nh.NganHangId
                                            join cn in _context.ChiNhanhNganHang on tknh.ChiNhanhNganHang.ChiNhanhNganHangId equals cn.ChiNhanhNganHangId
                                            where tknh.TaiKhoanId == request.TaiKhoanNganHangId
                                            select new {tknh, tknh.NganHang, tknh.ChiNhanhNganHang}).ToList();
                var donviyc = _context.DonVi.FirstOrDefault(x => (x.Id == request.DonViYeuCauId));
                // if (donviyc == null)
                // {
                //     return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                // }
                var boPhanYC = _context.DonVi.FirstOrDefault(x => (x.Id == request.BoPhanCauId));
                // if (boPhanYC == null)
                // {
                //     return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                // }
                // var nganHang = _context.NganHang.FirstOrDefault(x => (x.NganHangId == request.NganHangId));
                // if (nganHang == null)
                // {
                //     return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                // }
                var hoSoTT = _context.HoSoThanhToan.FirstOrDefault(x => (x.HoSoId == request.HoSoId));
                if (hoSoTT == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var account = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.IdLogin));
                LichSuHoSoTT tam = new LichSuHoSoTT() { };
                tam.LichSuId = Guid.NewGuid();
                tam.HoSoThanhToan = hoSoTT;
                tam.LoaiHoSo = loaihoso;
                tam.MaHoSo = hoSoTT.MaHoSo;
                tam.TenHoSo = request.TenHoSo;
                tam.NamHoSo = request.NamHoSo;
                tam.ThoiGianLuTru = request.ThoiGianLuTru;
                tam.MucDoUuTien = request.MucDoUuTien;
                tam.HanThanhToan = request.HanThanhToan;
                tam.GhiChu = request.GhiChu;
                tam.DonViYeuCau = donviyc;
                tam.BoPhanCau  = boPhanYC;
                tam.NgayGui = request.NgayGui;
                tam.NgayTiepNhan = request.NgayTiepNhan;
                tam.NgayDuyet = request.NgayDuyet;
                tam.SoTien = request.SoTien;
                tam.NgayLapCT = request.NgayLapCT;
                tam.TrangThaiHoSo = request.TrangThaiHoSo;
                tam.BuocThucHien = request.BuocThucHien;
                tam.NgayTao = DateTime.Now;
                tam.NguoiThuHuong = taiKhoanNH[0].tknh.TenTaiKhoan;
                tam.SoTienThucTe = request.SoTienThucTe;
                tam.SoTKThuHuong = taiKhoanNH[0].tknh.SoTaiKhoan;
                tam.NganHangThuHuong = taiKhoanNH[0].NganHang.NganHangId;
                tam.HinhThucTT = request.HinhThucTT;
                tam.ThoiGianCapNhat = DateTime.Now;
                tam.NguoiCapNhat = account;
                _context.LichSuHoSoTT.Add(tam);
                _context.SaveChanges();
                return new ResponsePostViewModel("Thêm mới thành công", 200);

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostViewModel(e.ToString(), 400);
            }
        }
        public ResponseLichSuViewModel GetLichSuTTByHoSoId(string hoSoId)
        {
            try
            {
                var check = _context.LichSuHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == hoSoId);
                if (check == null){
                    return new ResponseLichSuViewModel(null, 204, 0);
                }
                var lichSuList = from hstt in _context.LichSuHoSoTT
                                    where hstt.HoSoThanhToan.HoSoId.ToString() == hoSoId
                                   select new LichSuHoSoViewModel
                                   {
                                       Id = hstt.HoSoThanhToan.HoSoId,
                                       lichSuId = hstt.LichSuId,
                                       maHoSo = hstt.MaHoSo,
                                       tenVietTat = hstt.TenHoSo,
                                       namHoSo = hstt.NamHoSo,
                                       ghiChu = hstt.GhiChu,
                                       ngayGui = hstt.NgayGui,
                                       ngayDuyet = hstt.NgayDuyet,
                                       ngayLapCT = hstt.NgayLapCT,
                                       trangThaiHoSo = hstt.TrangThaiHoSo,
                                       buocThucHien = hstt.BuocThucHien,
                                       ngayTao = hstt.NgayTao,
                                       nguoiThuHuong = hstt.NguoiThuHuong,
                                       soTKThuHuong = hstt.SoTKThuHuong,
                                       hinhThucTT = hstt.HinhThucTT,
                                    //    hinhThucChi = hstt.HinhThucChi,
                                       thoiGianLuutru = hstt.ThoiGianLuTru,
                                    //    tenNguoiTao = hstt.ChucNangChaId,
                                       ngayTiepNhan = hstt.NgayTiepNhan,
                                       thoiGianThanhToan = hstt.HanThanhToan,
                                       mucDoUuTien = hstt.MucDoUuTien,
                                       boPhanYeuCau = _context.DonVi.FirstOrDefault(x => x.Id == hstt.BoPhanCau.Id).TenDonVi,
                                       boPhanYeuCauId = _context.DonVi.FirstOrDefault(x => x.Id == hstt.BoPhanCau.Id).Id,
                                       LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hstt.LoaiHoSo.LoaiHoSoId).TenLoaiHoSo,
                                       LoaiHoSoId = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hstt.LoaiHoSo.LoaiHoSoId).LoaiHoSoId,
                                       tongTienDeNghiTT = hstt.SoTien,
                                    //    vaiTroPheDuyet = hstt.Type
                                        thoiGianCapNhat = hstt.ThoiGianCapNhat,
                                        nguoiCapNhat = hstt.NguoiCapNhat
                                   };
                return new ResponseLichSuViewModel(lichSuList.ToList(), 200, 1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }
    }
}