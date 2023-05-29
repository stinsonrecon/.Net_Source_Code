using System;
using System.Collections.Generic;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace BCXN.Repositories
{
    public class DonViRepository : IDonViRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DonViRepository> _logger;
        private readonly RoleManager<ApplicationRole> roleManager;
        public DonViRepository(ApplicationDbContext context, ILogger<DonViRepository> logger)
        {
            _context = context;
            this._logger = logger;
        }

        public List<DonViViewModel> GetDonVi()
        {
            try
            {
                var donVi = from dv in _context.DonVi
                            where dv.TrangThai == 1
                            where dv.LaPhongBan == 1
                            select new DonViViewModel
                            {
                                Id = dv.Id,
                                TenDonVi = dv.TenDonVi,
                                MaDonVi = dv.MaDonVi,
                                DiaChi = dv.DiaChi,
                                SoDienThoai = dv.SoDienThoai,
                                Email = dv.Email,
                                MoTa = dv.MoTa,
                                DonViChaId = dv.DonViChaId,
                                NguoiTaoId = dv.NguoiTaoId,
                                ThoiGianTao = dv.ThoiGianTao,
                                LaPhongBan = dv.LaPhongBan,
                                TrangThai = dv.TrangThai,
                                ERPIDDonVi = dv.ERPIDDonVi,
                                ERPMaChiNhanh = dv.ERPMaChiNhanh,
                                ERPMaCongTy = dv.ERPMaCongTy,
                                DOfficeDonVi = dv.DOfficeDonVi
                            };

                var response = donVi.ToList();
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public DonVi GetDonViById(int id)
        {
            return _context.DonVi.FirstOrDefault(x => (x.Id == id));
        }

        public ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request)
        {
            try
            {
                var donVi = from dv in _context.DonVi
                            select new DonViGetViewModel
                            {
                                Id = dv.Id,
                                TenDonVi = dv.TenDonVi,
                                MaDonVi = dv.MaDonVi,
                                DiaChi = dv.DiaChi,
                                SoDienThoai = dv.SoDienThoai,
                                Email = dv.Email,
                                MoTa = dv.MoTa,
                                DonViChaId = dv.DonViChaId,
                                DonViChaTieuDe = _context.DonVi.FirstOrDefault(s => s.Id == dv.DonViChaId).TenDonVi,
                                NguoiTaoId = dv.NguoiTaoId,
                                ThoiGianTao = dv.ThoiGianTao,
                                LaPhongBan = dv.LaPhongBan,
                                TrangThai = dv.TrangThai,
                                ERPIDDonVi = dv.ERPIDDonVi,
                                ERPMaChiNhanh = dv.ERPMaChiNhanh,
                                ERPMaCongTy = dv.ERPMaCongTy,
                                DOfficeDonVi = dv.DOfficeDonVi
                            };
                // if (request.LoaiDonVi != null)
                // {
                //     donVi = donVi.Where(s => s.LoaiDonVi == request.LoaiDonVi);
                // }
                if (!String.IsNullOrEmpty(request.TenDV))
                {
                    donVi = donVi.Where(s => s.TenDonVi.Contains(request.TenDV));
                }
                if (request.TrangThai == 1)
                {
                    donVi = donVi.Where(s => s.TrangThai == request.TrangThai);
                }
                if (request.TrangThai == 0)
                {
                    donVi = donVi.Where(s => s.TrangThai == request.TrangThai || s.TrangThai == null);
                }
                donVi = donVi.OrderBy(s => s.TenDonVi);
                var totalRecord = donVi.ToList().Count();
                if (request.PageIndex > 0)
                {
                    donVi = donVi.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }
                var response = donVi.ToList();
                return new ResponseDonViViewModel(response, 200, totalRecord);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }


        public ResponsePostViewModel CreateDonVi(DonVi donVi)
        {
            try
            {
                var donViItem = new DonVi
                {
                    TenDonVi = donVi.TenDonVi,
                    MaDonVi = donVi.MaDonVi,
                    DiaChi = donVi.DiaChi,
                    SoDienThoai = donVi.SoDienThoai,
                    Email = donVi.Email,
                    MoTa = donVi.MoTa,
                    DonViChaId = donVi.DonViChaId,
                    NguoiTaoId = donVi.NguoiTaoId,
                    ThoiGianTao = DateTime.Now,
                    LaPhongBan = donVi.LaPhongBan,
                    TrangThai = donVi.TrangThai,
                    ERPIDDonVi = donVi.ERPIDDonVi,
                    ERPMaChiNhanh = donVi.ERPMaChiNhanh,
                    ERPMaCongTy = donVi.ERPMaCongTy,
                    DOfficeDonVi = donVi.DOfficeDonVi
                };
                _context.DonVi.Add(donViItem);
                _context.SaveChanges();
                return new ResponsePostViewModel("Thêm mới thành công", 200);

            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }

        }
        public ResponsePostViewModel UpdateDonVi(DonVi donVi)
        {
            try
            {
                var donViItem = _context.DonVi.FirstOrDefault(item => item.Id == donVi.Id);
                if(donViItem == null)
                {
                    return new ResponsePostViewModel("Khong tìm thấy đơn vị", 404);
                }

                donViItem.TenDonVi = donVi.TenDonVi;
                donViItem.MaDonVi = donVi.MaDonVi;
                donViItem.DiaChi = donVi.DiaChi;
                donViItem.SoDienThoai = donVi.SoDienThoai;
                donViItem.Email = donVi.Email;
                donViItem.MoTa = donVi.MoTa;
                donViItem.DonViChaId = donVi.DonViChaId;
                donViItem.LaPhongBan = donVi.LaPhongBan;
                donViItem.TrangThai = donVi.TrangThai;
                donViItem.ERPMaCongTy = donVi.ERPMaCongTy;
                donViItem.ERPMaChiNhanh = donVi.ERPMaChiNhanh;
                donViItem.ERPIDDonVi = donVi.ERPIDDonVi;
                donViItem.DOfficeDonVi = donVi.DOfficeDonVi;
                _context.SaveChanges();
                int rows = _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);

            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }

        }

        public ResponsePostViewModel DeleteDonVi(int id)
        {
            try
            {
                var donViItem = _context.DonVi.FirstOrDefault(item => item.Id == id);
                if(donViItem == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy đơn vị", 404);
                }

                _context.DonVi.Remove(donViItem);
                _context.SaveChanges();
                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 200);
            }
        }

        public List<DonViViewModel> GetBoPhanByIdDonVi(int id)
        {
            try
            {
                var donVi = from dv in _context.DonVi
                            where dv.TrangThai == 1
                            where dv.LaPhongBan == 1 && dv.DonViChaId == id
                            select new DonViViewModel
                            {
                                Id = dv.Id,
                                TenDonVi = dv.TenDonVi,
                                MaDonVi = dv.MaDonVi,
                                DiaChi = dv.DiaChi,
                                SoDienThoai = dv.SoDienThoai,
                                Email = dv.Email,
                                MoTa = dv.MoTa,
                                DonViChaId = dv.DonViChaId,
                                NguoiTaoId = dv.NguoiTaoId,
                                ThoiGianTao = dv.ThoiGianTao,
                                LaPhongBan = dv.LaPhongBan,
                                TrangThai = dv.TrangThai,
                                ERPIDDonVi = dv.ERPIDDonVi,
                                ERPMaChiNhanh = dv.ERPMaChiNhanh,
                                ERPMaCongTy = dv.ERPMaCongTy,
                                DOfficeDonVi = dv.DOfficeDonVi
                            };

                var response = donVi.ToList();
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }
    }
}
