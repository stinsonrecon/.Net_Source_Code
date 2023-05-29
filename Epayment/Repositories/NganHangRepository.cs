using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AutoMapper;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace Epayment.Repositories
{
    public interface INganHangRepository
    {
        List<NganHangViewModel> GetNganHang();
        Response CreateNganHang(NganHangParams nganhang);
        Response CreateChiNhanhNganHang(ChiNhanhNganHangParams chinhanhnganhang);
        Response CreateTaiKhoanNganHang(TaiKhoanNganHangParams taikhoannganhang);
        ResponseGetNganHang GetAllNganHang(NganHangPagination nganHangPagination);
        ResponseGetChiNhanh GetAllChiNhanh(ChiNhanhNganHangPagination chiNhanhNganHangPagination);
        ResponseGetTaiKhoanNganHang GetAllTaiKhoanNganHang(TaiKhoanNganHangPagination taiKhoanNganHangPagination);
        Response UpdateNganHang(NganHangViewModel nganHang);
        Response UpdateChiNhanh(ChiNhanhViewModel chiNhanh);
        Response UpdateTaiKhoanNganHang(TaiKhoanNganHangViewModel taiKhoanNganHang);
        Response DeleteNganHang(Guid nganHangId);
        Response DeleteChiNhanh(Guid chiNhanhNganHangId);
        Response DeleteTaiKhoanNganHang(Guid taiKhoanNganHangId);
        Response ApproveTaiKhoanNganHang(Guid taiKhoanNganHangId, int trangThai);
        // List<QuocGiaViewModel> GetAllQuocGia();
        // List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId);
    }
    public class NganHangRepository: INganHangRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NganHangRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NganHangRepository(ApplicationDbContext context, ILogger<NganHangRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public List<NganHangViewModel> GetNganHang()
        {
            try
            {
                var listNganHang = from bc in _context.NganHang
                               select new NganHangViewModel
                               {
                                   NganHangId = bc.NganHangId,
                                   MaNganHang  = bc.MaNganHang,
                                   ThoiGianTao = bc.ThoiGianTao,
                                   ThoiGianTiepNhan = bc.ThoiGianTiepNhan,
                                   TrangThaiTiepNhan = bc.TrangThaiTiepNhan,
                                   TenNganHang = bc.TenNganHang,
                                   TenVietTat = bc.TenVietTat
                               };
                
                listNganHang = listNganHang.OrderBy(x => x.MaNganHang);

                return listNganHang.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }

        public Response CreateNganHang(NganHangParams nganhang)
        {
            try
            {
                var nganHangItem = _context.NganHang.FirstOrDefault(x => x.MaNganHang.ToUpper().Trim() == nganhang.MaNganHang.ToUpper().Trim() && x.DaXoa == false);
                if ( nganHangItem != null)
                {
                    return new Response(message: "Mã ngân hàng đã tồn tại", data: "", errorcode: "002", success: false);
                } 
                _context.NganHang.Add(new NganHang {
                    MaNganHang = nganhang.MaNganHang.ToLower().Trim(),
                    MaNganHangERP = nganhang.MaNganHangERP,
                    LookupType = nganhang.LookupType,
                    TrangThaiTiepNhan = nganhang.TrangThaiTiepNhan,
                    ThoiGianTao = DateTime.Now,
                    ThoiGianTiepNhan = nganhang.ThoiGianTiepNhan,
                    GhiChu = nganhang.GhiChu,
                    TenNganHang = nganhang.TenNganHang,
                    TenVietTat = nganhang.TenVietTat,
                    PhuongThucKetNoi = nganhang.PhuongThucKetNoi
                });
                _context.SaveChanges();
                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response UpdateTaiKhoanNganHangNhanTrenERP(TaiKhoanNganHangParams taikhoannganhang)
        {
            try
            {
                Dictionary<string, object> payload = new Dictionary<string, object> {
                    { "UserId", taikhoannganhang.NguoiTaoId },
                    { "MaDonViERP", taikhoannganhang.MaDonViERP },
                    { "MaChiNhanhNganHang", "" },
                    { "TenNganHang", taikhoannganhang.TenNganHang },
                    { "TenChiNhanhNganHang", taikhoannganhang.TenChiNhanhNganHang },
                    { "SwiftCode", taikhoannganhang.SwiftCode },
                    { "DiaChi", taikhoannganhang.DiaChi },
                    { "ThanhPho", taikhoannganhang.ThanhPho },
                    { "QuocGia", taikhoannganhang.QuocGia },
                    { "SoDienThoai", taikhoannganhang.SDT },
                    { "Email", taikhoannganhang.Email },
                    { "TenNganHangVietTat", taikhoannganhang.TenNganHangVietTat },
                    { "LoaiTaiKhoan", taikhoannganhang.LoaiTaiKhoan },
                    { "SoTaiKhoan" , taikhoannganhang.SoTaiKhoan },
                    { "LaTaiKhoanNhan", taikhoannganhang.LaTaiKhoanNhan },
                    { "TenTaiKhoan", taikhoannganhang.TenTaiKhoan },
                    { "Active", taikhoannganhang.Active },
                    { "OldActive", taikhoannganhang.OldActive }
                };
                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    _configuration["ErpSettings:UpdateTaiKhoanNganHangNhanAddress"],
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    resultResponse.Wait();
                    var erpResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                    dynamic erpResponseJson = JToken.Parse(erpResponseString);
                    if (erpResponseJson.Success == false) return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "007", success: false);
                    return new Response(message: "Đồng bộ thành công", data: "", errorcode: "", success: true);
                }
                else
                {
                    return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "001", success: false);
                }

            }
            catch (Exception e)
            {
                return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "002", success: false);
            }
        }

        public Response InsertTaiKhoanNganHangTrenERP(TaiKhoanNganHangParams taikhoannganhang)
        {
            try
            {
                Dictionary<string, object> payload = new Dictionary<string, object> {
                    { "UserId", taikhoannganhang.NguoiTaoId },
                    { "MaDonViERP", taikhoannganhang.MaDonViERP },
                    { "MaChiNhanhNganHang", "" },
                    { "TenNganHang", taikhoannganhang.TenNganHang },
                    { "TenChiNhanhNganHang", taikhoannganhang.TenChiNhanhNganHang },
                    { "SwiftCode", taikhoannganhang.SwiftCode },
                    { "DiaChi", taikhoannganhang.DiaChi },
                    { "ThanhPho", taikhoannganhang.ThanhPho },
                    { "QuocGia", taikhoannganhang.QuocGia },
                    { "SoDienThoai", taikhoannganhang.SDT },
                    { "Email", taikhoannganhang.Email },
                    { "TenNganHangVietTat", taikhoannganhang.TenNganHangVietTat },
                    { "LoaiTaiKhoan", taikhoannganhang.LoaiTaiKhoan },
                    { "SoTaiKhoan" , taikhoannganhang.SoTaiKhoan },
                    { "LaTaiKhoanNhan", taikhoannganhang.LaTaiKhoanNhan },
                    { "TenTaiKhoan", taikhoannganhang.TenTaiKhoan },
                    { "Active", taikhoannganhang.Active }
                };
                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    _configuration["ErpSettings:InsertTaiKhoanNganHangAddress"],
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    resultResponse.Wait();
                    var erpResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                    dynamic erpResponseJson = JToken.Parse(erpResponseString);
                    if (erpResponseJson.Success == false) return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "007", success: false);
                    return new Response(message: "Đồng bộ thành công", data: "", errorcode: "", success: true);
                }
                else
                {
                    return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "001", success: false);
                }

            }
            catch(Exception e)
            {
                return new Response(message: "Lỗi: Chưa đồng bộ được chi nhánh ngân hàng tới erp", data: "", errorcode: "002", success: false);
            }
        }

        public Response CreateChiNhanhNganHang(ChiNhanhNganHangParams chinhanhnganhang)
        {
            try
            {
                var nganhangItem = _context.NganHang.FirstOrDefault(x => (x.NganHangId == chinhanhnganhang.NganHangId));
                if (nganhangItem == null) return new Response(message: "Không tìm thấy ngân hàng", data: "", errorcode: "002", success: false);
                var chiNhanhItem = _context.ChiNhanhNganHang.FirstOrDefault( x => x.NganHang.NganHangId == chinhanhnganhang.NganHangId && x.MaChiNhanhErp.ToUpper() == chinhanhnganhang.MaChiNhanhErp.ToUpper().Trim() && x.DaXoa == false);
                if (chiNhanhItem != null) return new Response(message: "Đã tồn tại mã chi nhánh trong ngân hàng", data: "", errorcode: "003", success: false);
                _context.ChiNhanhNganHang.Add(new ChiNhanhNganHang {
                    NganHang = nganhangItem,
                    MaChiNhanhErp = chinhanhnganhang.MaChiNhanhErp.ToUpper().Trim(),
                    TenChiNhanh = chinhanhnganhang.TenChiNhanh,
                    TrangThai = 1,
                    LoaiTaiKhoan = chinhanhnganhang.LoaiTaiKhoan
                });
                _context.SaveChanges();
                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response CreateTaiKhoanNganHang(TaiKhoanNganHangParams taikhoannganhang)
        {
            try
            {
                var nguoitaoItem = _context.ApplicationUser.FirstOrDefault(x => (x.Id == taikhoannganhang.NguoiTaoId));
                if (nguoitaoItem == null) return new Response(message: "Lỗi", data: "Không tìm thấy người tạo", errorcode: "001", success: false);

                var taiKhoanDetail = from tk in _context.TaiKhoanNganHang
                                    where tk.NganHang.NganHangId == taikhoannganhang.NganHangId
                                    where tk.SoTaiKhoan == taikhoannganhang.SoTaiKhoan
                                    select tk;
                if(taiKhoanDetail.ToList().Count > 0) {
                    return new Response(message: "Số tài khoản này đã tồn tại trong hệ thống", data: "", errorcode: "002", success: false);
                }
                var nganhangItem = _context.NganHang.FirstOrDefault(x => (x.NganHangId == taikhoannganhang.NganHangId));
                var donviItem = _context.DonVi.FirstOrDefault(x => (x.Id == taikhoannganhang.DonViId));
                if (donviItem == null)
                {
                    return new Response(message: "Không tìm thấy đơn vị", data: "", errorcode: "003", success: false);
                }
                var chinhanhnganhangItem = _context.ChiNhanhNganHang.FirstOrDefault(x => (x.ChiNhanhNganHangId == taikhoannganhang.ChiNhanhNganHangId));

                if (nganhangItem == null || chinhanhnganhangItem == null) return new Response(message: "Không tìm thấy ngân hàng hoặc chi nhánh", data: "", errorcode: "004", success: false);
                var quocGiaItem = _context.QuocGia.FirstOrDefault(x => x.QuocGiaId == taikhoannganhang.QuocGiaId);
                if (quocGiaItem == null)
                {
                    return new Response(message: "Không tìm thấy quốc gia", data: "", errorcode: "005", success: false);
                }
                var tinhTPItem = _context.TinhThanhPho.FirstOrDefault( x => x.TinhTpId == taikhoannganhang.TinhTPId);
                if (tinhTPItem == null)
                {
                    return new Response(message: "Không tìm thấy tỉnh/thành phố", data: "", errorcode: "006", success: false);
                }
                _context.TaiKhoanNganHang.Add(new TaiKhoanNganHang
                {
                    NganHang = nganhangItem,
                    ChiNhanhNganHang = chinhanhnganhangItem,
                    TenTaiKhoan = taikhoannganhang.TenTaiKhoan,
                    SoTaiKhoan = taikhoannganhang.SoTaiKhoan,
                    ThoiGianTao = DateTime.Now,
                    GhiChu = taikhoannganhang.GhiChu,
                    TrangThai = 1,
                    DonVi = donviItem,
                    QuocGia = quocGiaItem,
                    TinhTp = tinhTPItem,
                    Sdt = taikhoannganhang.SDT,
                    DiaChi = taikhoannganhang.DiaChi,
                    Email = taikhoannganhang.Email,
                    LaTaiKhoanNhan = taikhoannganhang.LaTaiKhoanNhan
                });
                _context.SaveChanges();

                // đồng bộ dữ liệu sang erp
                TaiKhoanNganHangParams p = new TaiKhoanNganHangParams();
                p.Active = true;
                p.MaDonViERP = donviItem.ERPMaChiNhanh;
                p.NguoiTaoId = taikhoannganhang.NguoiTaoId;
                p.MaChiNhanhNganHang = "";
                p.TenNganHang = nganhangItem.TenNganHang;
                p.TenChiNhanhNganHang = chinhanhnganhangItem.TenChiNhanh;
                p.SwiftCode = chinhanhnganhangItem.MaChiNhanhErp;
                p.DiaChi = taikhoannganhang.DiaChi;
                p.ThanhPho = tinhTPItem.TenTinhTp;
                p.QuocGia = quocGiaItem.TenQuocGia;
                p.Email = taikhoannganhang.Email;
                p.TenNganHangVietTat = nganhangItem.TenVietTat;
                p.LoaiTaiKhoan = chinhanhnganhangItem.LoaiTaiKhoan;
                p.SoTaiKhoan = taikhoannganhang.SoTaiKhoan;
                p.LaTaiKhoanNhan = taikhoannganhang.LaTaiKhoanNhan;
                p.TenTaiKhoan = taikhoannganhang.TenTaiKhoan;
                p.SDT = taikhoannganhang.SDT;
                Response r = InsertTaiKhoanNganHangTrenERP(p);

                return new Response(message: "Thêm mới thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "009", success: false);
            }
        }

        public ResponseGetNganHang GetAllNganHang(NganHangPagination nganHangPagination)
        {
            try
            {
                var listNganHang = from s in _context.NganHang
                                   select new NganHangViewModel
                                   {
                                       NganHangId = s.NganHangId,
                                       MaNganHang = s.MaNganHang,
                                       MaNganHangERP = s.MaNganHangERP,
                                       LookupType = s.LookupType,
                                       ThoiGianTao = s.ThoiGianTao,
                                       ThoiGianTiepNhan = s.ThoiGianTiepNhan,
                                       TrangThaiTiepNhan = s.TrangThaiTiepNhan,
                                       TenNganHang = s.TenNganHang,
                                       GhiChu = s.GhiChu,
                                       TenVietTat = s.TenVietTat,
                                       DaXoa = s.DaXoa,
                                       PhuongThucKetNoi = s.PhuongThucKetNoi
                                   };

                listNganHang = listNganHang.OrderBy(x => x.MaNganHang);
                if (nganHangPagination.NganHangId != null) listNganHang = listNganHang.Where(s => s.NganHangId == nganHangPagination.NganHangId);
                if (nganHangPagination.MaNganHang != null) listNganHang = listNganHang.Where(s => s.MaNganHang == nganHangPagination.MaNganHang);
                if (nganHangPagination.ThoiGianTao != null) listNganHang = listNganHang.Where(s => s.ThoiGianTao == nganHangPagination.ThoiGianTao);
                if (nganHangPagination.ThoiGianTiepNhan != null) listNganHang = listNganHang.Where(s => s.ThoiGianTiepNhan == nganHangPagination.ThoiGianTiepNhan);
                if (nganHangPagination.TrangThaiTiepNhan != null) listNganHang = listNganHang.Where(s => s.TrangThaiTiepNhan == nganHangPagination.TrangThaiTiepNhan);
                if (nganHangPagination.TenNganHang != null) listNganHang = listNganHang.Where(s => s.TenNganHang == nganHangPagination.TenNganHang);
                if (nganHangPagination.GhiChu != null) listNganHang = listNganHang.Where(s => s.GhiChu == nganHangPagination.GhiChu);
                if (nganHangPagination.TenVietTat != null) listNganHang = listNganHang.Where(s => s.TenVietTat == nganHangPagination.TenVietTat);
                if (nganHangPagination.DaXoa != null) listNganHang = listNganHang.Where(s => s.DaXoa == nganHangPagination.DaXoa);
                if (nganHangPagination.LookupType != null) listNganHang = listNganHang.Where(s => s.LookupType == nganHangPagination.LookupType);
                if (nganHangPagination.MaNganHangERP != null) listNganHang = listNganHang.Where(s => s.MaNganHangERP == nganHangPagination.MaNganHangERP);
                if (nganHangPagination.SearchString != null) listNganHang = listNganHang.Where(s => (s.MaNganHang.Contains(nganHangPagination.SearchString) || s.TenNganHang.Contains(nganHangPagination.SearchString)));

                var totalRecord = listNganHang.ToList().Count();
                if (nganHangPagination.PageIndex > 0)
                {
                    listNganHang = listNganHang.Skip(nganHangPagination.PageSize * (nganHangPagination.PageIndex - 1)).Take(nganHangPagination.PageSize);
                }
                var response = listNganHang.ToList();

                return new ResponseGetNganHang("", "", true, totalRecord, response);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetNganHang(e.Message, "001", false, 0, null);
            }
        }

        public ResponseGetChiNhanh GetAllChiNhanh(ChiNhanhNganHangPagination chiNhanhNganHangPagination)
        {
            try
            {
                var listChiNhanh = from s in _context.ChiNhanhNganHang
                                   select new ChiNhanhViewModel
                                   {
                                       ChiNhanhNganHangId = s.ChiNhanhNganHangId,
                                       DaXoa = s.DaXoa,
                                       MaChiNhanhErp = s.MaChiNhanhErp,
                                       TrangThai = s.TrangThai,
                                       NganHangId = s.NganHang.NganHangId,
                                       TenChiNhanh = s.TenChiNhanh,
                                       TenNganHang = s.NganHang.TenNganHang,
                                       LoaiTaiKhoan = s.LoaiTaiKhoan
                                   };

                if (chiNhanhNganHangPagination.ChiNhanhNganHangId != null) listChiNhanh = listChiNhanh.Where(s => s.ChiNhanhNganHangId == chiNhanhNganHangPagination.ChiNhanhNganHangId);
                if (chiNhanhNganHangPagination.MaChiNhanhErp != null) listChiNhanh = listChiNhanh.Where(s => s.MaChiNhanhErp == chiNhanhNganHangPagination.MaChiNhanhErp);
                if (chiNhanhNganHangPagination.DaXoa != null) listChiNhanh = listChiNhanh.Where(s => s.DaXoa == chiNhanhNganHangPagination.DaXoa);
                if (chiNhanhNganHangPagination.TrangThai != null) listChiNhanh = listChiNhanh.Where(s => s.TrangThai == chiNhanhNganHangPagination.TrangThai);
                if (chiNhanhNganHangPagination.NganHangId != null) listChiNhanh = listChiNhanh.Where(s => s.NganHangId == chiNhanhNganHangPagination.NganHangId);
                if (chiNhanhNganHangPagination.TenChiNhanh != null) listChiNhanh = listChiNhanh.Where(s => s.TenChiNhanh == chiNhanhNganHangPagination.TenChiNhanh);
                if (chiNhanhNganHangPagination.LoaiTaiKhoan != null) listChiNhanh = listChiNhanh.Where(s => s.LoaiTaiKhoan == chiNhanhNganHangPagination.LoaiTaiKhoan);
                if (chiNhanhNganHangPagination.SearchString != null) listChiNhanh = listChiNhanh.Where(s => (s.MaChiNhanhErp.Contains(chiNhanhNganHangPagination.SearchString) || s.TenChiNhanh.Contains(chiNhanhNganHangPagination.SearchString)));

                var totalRecord = listChiNhanh.ToList().Count();
                if (chiNhanhNganHangPagination.PageIndex > 0)
                {
                    listChiNhanh = listChiNhanh.Skip(chiNhanhNganHangPagination.PageSize * (chiNhanhNganHangPagination.PageIndex - 1)).Take(chiNhanhNganHangPagination.PageSize);
                }
                var response = listChiNhanh.ToList();

                return new ResponseGetChiNhanh("", "", true, totalRecord, response);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetChiNhanh(e.Message, "001", false, 0, null);
            }
        }

        public ResponseGetTaiKhoanNganHang GetAllTaiKhoanNganHang(TaiKhoanNganHangPagination taiKhoanNganHangPagination)
        {
            try
            {
                var listTaiKhoanNganHang = from s in _context.TaiKhoanNganHang
                                   select new TaiKhoanNganHangViewModel
                                   {
                                       ChiNhanhNganHangId = s.ChiNhanhNganHang.ChiNhanhNganHangId,
                                       GhiChu = s.GhiChu,
                                       TrangThai = s.TrangThai,
                                       NganHangId = s.NganHang.NganHangId,
                                       SoTaiKhoan = s.SoTaiKhoan,
                                       TaiKhoanId = s.TaiKhoanId,
                                       TenTaiKhoan = s.TenTaiKhoan,
                                       ThoiGianTao = s.ThoiGianTao,
                                       TenChiNhanh = s.ChiNhanhNganHang.TenChiNhanh,
                                       TenNganHang = s.NganHang.TenNganHang,
                                       DonViId = s.DonVi.Id,
                                       TenDonVi = s.DonVi.TenDonVi,
                                       MaChiNhanh = s.ChiNhanhNganHang.MaChiNhanhErp,
                                       MaNganHang = s.NganHang.MaNganHang,
                                       TenNganHangVietTat = s.NganHang.TenVietTat,
                                       QuocGiaId = s.QuocGia.QuocGiaId,
                                       MaQuocGia = s.QuocGia.MaQuocGia,
                                       TenQuocGia = s.QuocGia.TenQuocGia,
                                       TinhTPId = s.TinhTp.TinhTpId,
                                       MaTinhTP = s.TinhTp.MaTinhTp,
                                       TenTinhTP = s.TinhTp.TenTinhTp,
                                       SDT = s.Sdt,
                                       DiaChi = s.DiaChi,
                                       Email = s.Email,
                                       LaTaiKhoanNhan = s.LaTaiKhoanNhan,
                                       LoaiTaiKhoan = s.ChiNhanhNganHang.LoaiTaiKhoan
                                   };

                if (taiKhoanNganHangPagination.ChiNhanhNganHangId != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.ChiNhanhNganHangId == taiKhoanNganHangPagination.ChiNhanhNganHangId);
                if (taiKhoanNganHangPagination.GhiChu != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.GhiChu == taiKhoanNganHangPagination.GhiChu);
                if (taiKhoanNganHangPagination.TrangThai != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.TrangThai == taiKhoanNganHangPagination.TrangThai);
                if (taiKhoanNganHangPagination.NganHangId != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.NganHangId == taiKhoanNganHangPagination.NganHangId);
                if (taiKhoanNganHangPagination.SoTaiKhoan != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.SoTaiKhoan == taiKhoanNganHangPagination.SoTaiKhoan);
                if (taiKhoanNganHangPagination.TaiKhoanId != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.TaiKhoanId == taiKhoanNganHangPagination.TaiKhoanId);
                if (taiKhoanNganHangPagination.TenTaiKhoan != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.TenTaiKhoan == taiKhoanNganHangPagination.TenTaiKhoan);
                if (taiKhoanNganHangPagination.ThoiGianTao != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.ThoiGianTao == taiKhoanNganHangPagination.ThoiGianTao);
                if (taiKhoanNganHangPagination.DonViId != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => s.DonViId == taiKhoanNganHangPagination.DonViId);
                if (taiKhoanNganHangPagination.SearchString != null) listTaiKhoanNganHang = listTaiKhoanNganHang.Where(s => (s.TenTaiKhoan.Contains(taiKhoanNganHangPagination.SearchString) || s.SoTaiKhoan.Contains(taiKhoanNganHangPagination.SearchString) || s.TenChiNhanh.Contains(taiKhoanNganHangPagination.SearchString) || s.TenNganHang.Contains(taiKhoanNganHangPagination.SearchString)));

                var totalRecord = listTaiKhoanNganHang.ToList().Count();
                if (taiKhoanNganHangPagination.PageIndex > 0)
                {
                    listTaiKhoanNganHang = listTaiKhoanNganHang.Skip(taiKhoanNganHangPagination.PageSize * (taiKhoanNganHangPagination.PageIndex - 1)).Take(taiKhoanNganHangPagination.PageSize);
                }
                var response = listTaiKhoanNganHang.OrderBy(x => x.TenTaiKhoan).ToList();

                return new ResponseGetTaiKhoanNganHang("", "", true, totalRecord, response);
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponseGetTaiKhoanNganHang(e.Message, "001", false, 0, null);
            }
        }

        public Response UpdateNganHang(NganHangViewModel nganHang)
        {
            try
            {
                var item = _context.NganHang.Where(s => s.NganHangId == nganHang.NganHangId).FirstOrDefault();
                if(item == null)
                {
                    return new Response(message: "Không tìm thấy ngân hàng", data: "", errorcode: "002", success: false);
                }
                // _mapper.Map(nganHang, item);
                item.MaNganHang = nganHang.MaNganHang;
                item.MaNganHangERP = nganHang.MaNganHangERP;
                item.LookupType = nganHang.LookupType;
                item.GhiChu = nganHang.GhiChu;
                item.TenNganHang = nganHang.TenNganHang;
                item.TenVietTat = nganHang.TenVietTat;
                item.TrangThaiTiepNhan = nganHang.TrangThaiTiepNhan;
                item.PhuongThucKetNoi = nganHang.PhuongThucKetNoi;
                _context.SaveChanges();
                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response UpdateChiNhanh(ChiNhanhViewModel chiNhanh)
        {
            try
            {
                var item = _context.ChiNhanhNganHang.Where(s => s.ChiNhanhNganHangId == chiNhanh.ChiNhanhNganHangId).FirstOrDefault();
                if (item == null)
                {
                    return new Response(message: "Không tìm thấy chi nhánh ngân hàng", data: "", errorcode: "002", success: false);
                }
                _mapper.Map(chiNhanh, item);
                var nganHang = _context.NganHang.Where(s => s.NganHangId == chiNhanh.NganHangId).FirstOrDefault();
                if(nganHang == null)
                {
                    return new Response(message: "Không tìm thấy ngân hàng", data: "", errorcode: "003", success: false);
                }
                item.NganHang = nganHang;
                _context.SaveChanges();
                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response UpdateTaiKhoanNganHang(TaiKhoanNganHangViewModel taiKhoanNganHang)
        {
            try
            {
                var nguoitaoItem = _context.ApplicationUser.FirstOrDefault(x => (x.Id == taiKhoanNganHang.NguoiTaoId));
                if (nguoitaoItem == null) return new Response(message: "Lỗi", data: "Không tìm thấy người tạo", errorcode: "001", success: false);

                var item = _context.TaiKhoanNganHang.Where(s => s.TaiKhoanId == taiKhoanNganHang.TaiKhoanId).FirstOrDefault();
                if (item == null)
                {
                    return new Response(message: "Không tìm thấy tài khoản ngân hàng", data: "", errorcode: "002", success: false);
                }
                //_mapper.Map(taiKhoanNganHang, item);
                var chiNhanhNganHang = _context.ChiNhanhNganHang.Where(s => s.ChiNhanhNganHangId == taiKhoanNganHang.ChiNhanhNganHangId).FirstOrDefault();
                if (chiNhanhNganHang == null)
                {
                    return new Response(message: "Không tìm thấy chi nhánh ngân hàng", data: "", errorcode: "003", success: false);
                }
                var donviItem = _context.DonVi.FirstOrDefault(x => (x.Id == taiKhoanNganHang.DonViId));
                if (donviItem == null)
                {
                    return new Response(message: "Không tìm thấy đơn vị", data: "", errorcode: "004", success: false);
                }
                var nganHangItem = _context.NganHang.FirstOrDefault(x => x.NganHangId == taiKhoanNganHang.NganHangId);
                if(nganHangItem == null){
                    return new Response(message: "Không tìm thấy ngân hàng", data: "", errorcode: "005", success: false);
                }
                var quocGiaItem = _context.QuocGia.FirstOrDefault(x => x.QuocGiaId == taiKhoanNganHang.QuocGiaId);
                if (quocGiaItem == null)
                {
                    return new Response(message: "Không tìm thấy quốc gia", data: "", errorcode: "006", success: false);
                }
                var tinhTPItem = _context.TinhThanhPho.FirstOrDefault( x => x.TinhTpId == taiKhoanNganHang.TinhTPId);
                if (tinhTPItem == null)
                {
                    return new Response(message: "Không tìm thấy tỉnh/thành phố", data: "", errorcode: "007", success: false);
                }
                item.TenTaiKhoan = taiKhoanNganHang.TenTaiKhoan;
                item.SoTaiKhoan = taiKhoanNganHang.SoTaiKhoan;
                item.TrangThai = taiKhoanNganHang.TrangThai;
                item.GhiChu = taiKhoanNganHang.GhiChu;
                item.DonVi = donviItem;
                item.ChiNhanhNganHang = chiNhanhNganHang;
                item.NganHang = nganHangItem;
                item.QuocGia = quocGiaItem;
                item.TinhTp = tinhTPItem;
                item.Sdt = taiKhoanNganHang.SDT;
                item.DiaChi = taiKhoanNganHang.DiaChi;
                item.Email = taiKhoanNganHang.Email;
                _context.SaveChanges();

                // đồng bộ dữ liệu sang erp
                TaiKhoanNganHangParams p = new TaiKhoanNganHangParams();
                p.Active = true;
                if (taiKhoanNganHang.TrangThai == 2) p.Active = false;
                p.OldActive = true;
                if (item.TrangThai == 2) p.OldActive = false;
                p.MaDonViERP = donviItem.ERPMaChiNhanh;
                p.NguoiTaoId = taiKhoanNganHang.NguoiTaoId;
                p.MaChiNhanhNganHang = "";
                p.TenNganHang = nganHangItem.TenNganHang;
                p.TenChiNhanhNganHang = chiNhanhNganHang.TenChiNhanh;
                p.SwiftCode = chiNhanhNganHang.MaChiNhanhErp;
                p.DiaChi = item.DiaChi;
                p.ThanhPho = tinhTPItem.TenTinhTp;
                p.QuocGia = quocGiaItem.TenQuocGia;
                p.Email = item.Email;
                p.TenNganHangVietTat = nganHangItem.TenVietTat;
                p.LoaiTaiKhoan = chiNhanhNganHang.LoaiTaiKhoan;
                p.SoTaiKhoan = item.SoTaiKhoan;
                p.LaTaiKhoanNhan = item.LaTaiKhoanNhan;
                p.TenTaiKhoan = item.TenTaiKhoan;
                p.SDT = item.Sdt;
                if (item.LaTaiKhoanNhan == true) UpdateTaiKhoanNganHangNhanTrenERP(p);
                else InsertTaiKhoanNganHangTrenERP(p);

                return new Response(message: "Cập nhật thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "009", success: false);
            }
        }


        public Response DeleteNganHang(Guid nganHangId)
        {
            try
            {
                var nganHang = _context.NganHang.Where(s => s.NganHangId == nganHangId).FirstOrDefault();
                if(nganHang == null)
                {
                    return new Response(message: "Không tìm thấy ngân hàng", data: "", errorcode: "002", success: false);
                }
                _context.NganHang.Remove(nganHang);
                _context.SaveChanges();
                return new Response(message: "Xóa thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response DeleteChiNhanh(Guid chiNhanhNganHangId)
        {
            try
            {
                var chiNhanhNganHang = _context.ChiNhanhNganHang.Where(s => s.ChiNhanhNganHangId == chiNhanhNganHangId).FirstOrDefault();
                if (chiNhanhNganHang == null)
                {
                    return new Response(message: "Không tìm thấy chi nhánh ngân hàng", data: "", errorcode: "002", success: false);
                }
                _context.ChiNhanhNganHang.Remove(chiNhanhNganHang);
                _context.SaveChanges();
                return new Response(message: "Xóa thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response DeleteTaiKhoanNganHang(Guid taiKhoanNganHangId)
        {
            try
            {
                var taiKhoanNganHang = _context.TaiKhoanNganHang.Where(s => s.TaiKhoanId == taiKhoanNganHangId).FirstOrDefault();
                if (taiKhoanNganHang == null)
                {
                    return new Response(message: "Không tìm thấy tài khoản ngân hàng", data: "", errorcode: "002", success: false);
                }
                _context.TaiKhoanNganHang.Remove(taiKhoanNganHang);
                _context.SaveChanges();
                return new Response(message: "Xóa thành công", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response ApproveTaiKhoanNganHang(Guid taiKhoanNganHangId, int trangThai)
        {
            try
            {
                var Item = _context.TaiKhoanNganHang.FirstOrDefault(s => s.TaiKhoanId == taiKhoanNganHangId);
                if (Item == null)
                {
                    return new Response(message: "Không tìm thấy tài khoản ngân hàng", data: "", errorcode: "002", success: false);
                }
                Item.TrangThai = trangThai;
                _context.SaveChanges();
                return new Response(message: "Cập nhập thành công", data: "", errorcode: "00", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        // public List<QuocGiaViewModel> GetAllQuocGia()
        // {
        //     try
        //     {
        //         var listQuocGia = from QG in _context.QuocGia
        //                             select new QuocGiaViewModel {
        //                                 QuocGiaId = QG.QuocGiaId,
        //                                 MaQuocGia = QG.MaQuocGia,
        //                                 TenQuocGia = QG.TenQuocGia,
        //                                 TrangThai = QG.TrangThai,
        //                                 DaXoa = QG.DaXoa
        //                             };
        //         listQuocGia = listQuocGia.OrderBy(x => x.MaQuocGia);
        //         return listQuocGia.ToList();
        //     }
        //     catch (System.Exception e)
        //     {
        //         _logger.LogError("Lỗi:", e);
        //         throw;
        //     }
        // }

        // public List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId)
        // {
        //     try
        //     {
        //         var listTinhTP = from TP in _context.TinhThanhPho
        //                             where TP.QuocGia.QuocGiaId == quocGiaId
        //                             select new TinhThanhPhoViewModel {
        //                                 TinhTpId = TP.TinhTpId,
        //                                 QuocGia = TP.QuocGia,
        //                                 MaTinhTp = TP.MaTinhTp,
        //                                 TenTinhTp = TP.TenTinhTp,
        //                                 TrangThai = TP.TrangThai,
        //                                 DaXoa = TP.DaXoa
        //                             };
        //         listTinhTP = listTinhTP.OrderBy(x => x.MaTinhTp);
        //         return listTinhTP.ToList();
        //     }
        //     catch (System.Exception e)
        //     {
        //         _logger.LogError("Lỗi:", e);
        //         throw;
        //     }
        // }
    }
}