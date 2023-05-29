using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using BCXN.Statics;
using Epayment.Services;

namespace Epayment.Repositories
{
    public class HoSoThanhToanRepository : IHoSoThanhToanRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HoSoThanhToanRepository> _logger;
        private readonly IUtilsService _utilsservice;
        public HoSoThanhToanRepository(ApplicationDbContext context, ILogger<HoSoThanhToanRepository> logger, IUtilsService utilsservice)
        {
            _context = context;
            this._logger = logger;
            _utilsservice = utilsservice;
        }

        public ResponsePostHSTTViewModel CreateHoSoTT(ParmHoSoThanhToanViewModel request)
        {
            try
            {
                var loaihoso = _context.LoaiHoSo.FirstOrDefault(x => (x.LoaiHoSoId == request.LoaiHoSoId));
                if (loaihoso == null)
                {
                    return new ResponsePostHSTTViewModel("Không tìm thấy dữ liệu", 400, null);
                }
                var donviyc = _context.DonVi.FirstOrDefault(x => (x.Id == request.DonViYeuCauId));
                var boPhanYC = _context.DonVi.FirstOrDefault(x => (x.Id == request.BoPhanCauId));
                // var taiKhoanNH = _context.TaiKhoanNganHang.FirstOrDefault(x => x.TaiKhoanId == request.TaiKhoanNganHangId);
                var taiKhoanNH = (from tknh in _context.TaiKhoanNganHang
                                            join nh in _context.NganHang on tknh.NganHang.NganHangId equals nh.NganHangId
                                            join cn in _context.ChiNhanhNganHang on tknh.ChiNhanhNganHang.ChiNhanhNganHangId equals cn.ChiNhanhNganHangId
                                            where tknh.TaiKhoanId == request.TaiKhoanNganHangId
                                            select new {tknh, tknh.NganHang, tknh.ChiNhanhNganHang}).ToList();
                if (request.LanThanhToan <= 0)
                {
                    return new ResponsePostHSTTViewModel("lần thanh toán phải lớn hơn 0", 400, null);
                }
                var mahoso = "";
                var DB = _context.HoSoThanhToan.ToList();
                if ( DB.Count() == 0)
                {
                    mahoso = donviyc.MaDonVi.Replace(" ", "-") + "_" + loaihoso.MaLoaiHoSo.Replace(" ", "-") + "_" + request.NamHoSo + "_" + 1;
                }
                else
                {
                    var getMaHoSo = _context.HoSoThanhToan.OrderBy(x => x.NgayTao).ToList().Last();
                    int number = 1;
                    if (getMaHoSo.MaHoSo != null)
                    {
                        var numberMs = getMaHoSo.MaHoSo.Split("_");
                        var thutu = numberMs.ToList().Last();
                        number = Convert.ToInt32(thutu) + 1;
                    }
                    mahoso = donviyc.MaDonVi.Replace(" ", "-") + "_" + loaihoso.MaLoaiHoSo.Replace(" ", "-") + "_" + request.NamHoSo + "_" + number;
                }
                var maSoHopLe = checkMaSo(mahoso);
                // hinhthuc thanh toan 0 chuyen khoan, 1 tien mat , 2 thanh toan tai quay
                var account = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.IdLogin));
                HoSoThanhToan tam = new HoSoThanhToan() { };
                tam.HoSoId = Guid.NewGuid();
                tam.LoaiHoSo = loaihoso;
                tam.MaHoSo = maSoHopLe;
                tam.TenHoSo = request.TenHoSo;
                tam.NamHoSo = request.NamHoSo;
                tam.ThoiGianLuTru = request.ThoiGianLuTru;
                tam.MucDoUuTien = request.MucDoUuTien;
                tam.HanThanhToan = request.HanThanhToan;
                tam.GhiChu = request.GhiChu;
                tam.DonViYeuCau = donviyc;
                tam.BoPhanCauId = boPhanYC;
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
                tam.NganHang = taiKhoanNH[0].NganHang;
                tam.HinhThucTT = request.HinhThucTT;
                tam.HinhThucChi = request.HinhThucChi;
                tam.TenVietTat = request.TenVietTat;
                tam.CoHieuLuc = request.CoHieuLuc;
                tam.CoBanCung = request.CoBanCung;
                tam.LanThanhToan = request.LanThanhToan;
                tam.TenYeuCauThanhToan = request.TenYeuCauThanhToan;
                tam.NguoiTao = account;
                tam.LoaiTienTe = request.LoaiTienTe;
                tam.TaiKhoanThuHuong = taiKhoanNH[0].tknh;
                if (!String.IsNullOrEmpty(request.NoiDungThanhToan))
                    tam.NoiDungTT = BCXN.StringUtils.RemoveSign4VietnameseString(request.NoiDungThanhToan).Replace(",", "");
                else
                    tam.NoiDungTT = request.NoiDungThanhToan;
                tam.QuyTrinhPheDuyetId = request.QuyTrinhPheDuyetId;
                tam.QuaTrinhPheDuyetId = request.QuaTrinhPheDuyetId;
                tam.ThaoTacVuaThucHienId = request.ThaoTacBuocPheDuyetId;
                _context.HoSoThanhToan.Add(tam);
                    if (request.HoSoLienQuan != null){
                        foreach(var HoSoLQ in request.HoSoLienQuan){
                            HoSoThamChieu HSTC = new HoSoThamChieu();
                            HSTC.HoSoThamChieuId = Guid.NewGuid();
                            HSTC.HoSoLienQuanId = new Guid(HoSoLQ);
                            HSTC.HoSoThanhToan = tam;
                            _context.HoSoThamChieu.Add(HSTC);
                        }
                    }
                _context.SaveChanges();
                return new ResponsePostHSTTViewModel("Thêm mới thành công", 200, tam.HoSoId.ToString());

            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return new ResponsePostHSTTViewModel(e.ToString(), 400, null);
            }
        }

        public string checkMaSo(string mahoso)
        {
            while (_context.HoSoThanhToan.FirstOrDefault(x => x.MaHoSo == mahoso) != null)
            {
                var numberMs = mahoso.Split("_");
                var st = numberMs.ToList().Last();
                string ms = null;

                for (int i = 0; i < numberMs.Count(); i++)
                {
                    if (i != (numberMs.Count() - 1))
                    {
                        if (ms == null)
                        {
                            ms = numberMs[i];
                        }
                        else
                        {
                            ms = ms + "_" + numberMs[i];
                        }
                    }
                    else
                    {
                        mahoso = ms + "_" + (Convert.ToInt32(st) + 1);
                    }
                }

            }
            return mahoso;
        }
        public ResponsePostViewModel UpdateHoSoTT(ParmHoSoThanhToanViewModel request)
        {
            try
            {
                var Item = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == request.HoSoId);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                switch(Item.TrangThaiHoSo) 
                {
                    case BCXN.Statics.TrangThaiHoSo.ChuaPheDuyet:
                    case BCXN.Statics.TrangThaiHoSo.DaPheDuyet:
                    case BCXN.Statics.TrangThaiHoSo.DaThanhToan:
                    case BCXN.Statics.TrangThaiHoSo.DaTaoChungTu:
                    case BCXN.Statics.TrangThaiHoSo.DongHoSo:
                    case BCXN.Statics.TrangThaiHoSo.XoaHoSo:
                        return new ResponsePostViewModel("Không được cập nhật hồ sơ", 400);
                    //break;
                }
                // if (Item.TrangThaiHoSo != 5 || Item.TrangThaiHoSo != 3){
                //    return new ResponsePostViewModel("Không được cập nhật hồ sơ", 400); 
                // }
                var loaihoso = _context.LoaiHoSo.FirstOrDefault(x => (x.LoaiHoSoId == request.LoaiHoSoId));
                if (loaihoso == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                var donviyc = _context.DonVi.FirstOrDefault(x => (x.Id == request.DonViYeuCauId));
                var boPhanYC = _context.DonVi.FirstOrDefault(x => (x.Id == request.BoPhanCauId));
                var taiKhoanNH = (from tknh in _context.TaiKhoanNganHang
                                            join nh in _context.NganHang on tknh.NganHang.NganHangId equals nh.NganHangId
                                            join cn in _context.ChiNhanhNganHang on tknh.ChiNhanhNganHang.ChiNhanhNganHangId equals cn.ChiNhanhNganHangId
                                            where tknh.TaiKhoanId == request.TaiKhoanNganHangId
                                            select new {tknh, tknh.NganHang, tknh.ChiNhanhNganHang}).ToList();

                if (request.LanThanhToan <= 0)
                {
                    return new ResponsePostViewModel("lần thanh toán phải lớn hơn 0", 400);
                }
                var account = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.IdLogin));
                //Item.HoSoId = request.HoSoId;
                //Item.MaHoSo = request.MaHoSo;
                Item.LoaiHoSo = loaihoso;
                Item.TenHoSo = request.TenHoSo;
                Item.NamHoSo = request.NamHoSo;
                Item.ThoiGianLuTru = request.ThoiGianLuTru;
                Item.MucDoUuTien = request.MucDoUuTien;
                Item.HanThanhToan = request.HanThanhToan;
                Item.GhiChu = request.GhiChu;
                Item.DonViYeuCau = donviyc;
                Item.BoPhanCauId = boPhanYC;
                Item.NgayGui = request.NgayGui;
                Item.NgayTiepNhan = request.NgayTiepNhan;
                Item.NgayDuyet = request.NgayDuyet;
                Item.SoTien = request.SoTien;
                Item.NgayLapCT = request.NgayLapCT;
                Item.TrangThaiHoSo = request.TrangThaiHoSo;
                Item.BuocThucHien = request.BuocThucHien;
                Item.NgayTao = DateTime.Now;
                Item.NguoiThuHuong = taiKhoanNH[0].tknh.TenTaiKhoan;
                Item.SoTienThucTe = request.SoTienThucTe;
                Item.SoTKThuHuong = taiKhoanNH[0].tknh.SoTaiKhoan;
                Item.NganHang = taiKhoanNH[0].NganHang;
                Item.HinhThucTT = request.HinhThucTT;
                Item.HinhThucChi = request.HinhThucChi;
                Item.TenVietTat = request.TenVietTat;
                Item.CoHieuLuc = request.CoHieuLuc;
                Item.CoBanCung = request.CoBanCung;
                Item.LanThanhToan = request.LanThanhToan;
                Item.TenYeuCauThanhToan = request.TenYeuCauThanhToan;
                Item.NguoiTao = account;
                Item.LoaiTienTe = request.LoaiTienTe;
                Item.TaiKhoanThuHuong = taiKhoanNH[0].tknh;
                if (!String.IsNullOrEmpty(request.NoiDungThanhToan))
                    Item.NoiDungTT = BCXN.StringUtils.RemoveSign4VietnameseString(request.NoiDungThanhToan).Replace(",", "");
                else
                    Item.NoiDungTT = request.NoiDungThanhToan;
                var ItemHoSoTC = _context.HoSoThamChieu.Where(x => x.HoSoThanhToan.HoSoId.ToString() == request.HoSoId.ToString());
                _context.HoSoThamChieu.RemoveRange(ItemHoSoTC);
                if (request.HoSoLienQuan != null){
                        foreach(var HoSoLQ in request.HoSoLienQuan){
                            HoSoThamChieu HSTC = new HoSoThamChieu();
                            HSTC.HoSoThamChieuId = Guid.NewGuid();
                            HSTC.HoSoLienQuanId = new Guid(HoSoLQ);
                            HSTC.HoSoThanhToan = Item;
                            _context.HoSoThamChieu.Add(HSTC);
                        }
                    }
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
        public ResponsePostViewModel DeleteHoSoTT(Guid id, string userID)
        {
            try
            {
                var Item = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == id && x.NguoiTao.Id == userID);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu hoặc user không có quyền xóa", 400);
                }
                if (Item.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.KhoiTao ){
                    return new ResponsePostViewModel("Không được xóa hồ sơ", 400);
                }
                // new SqlCommand("DELETE FROM HoSoThanhToan WHERE HoSoId='@id'");

                // const string query = "DELETE FROM [dbo].[Customers] WHERE [id]={0}";
                // var rows = _context.ExecuteSqlCommand(query,id);

                //_context.Remove(_context.HoSoThanhToan.Single(x => x.HoSoId == id));
                // _context.HoSoThanhToan.Remove(test);
                Item.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.XoaHoSo;
                _context.SaveChanges();

                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public List<HoSoThanhToanViewModel> GetHoSoTT()
        {
            throw new System.NotImplementedException();
        }

        public ResponseHoSoViewModel GetHoSoTTById(string id)
        {
            try
            {
                var check = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == id);
                if (check == null)
                {
                    return new ResponseHoSoViewModel(null, 204, 0);
                }
                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == check.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == check.QuyTrinhPheDuyetId && i.BuocPheDuyetId == check.QuaTrinhPheDuyetId).ToList();
                var thuTu = 0;
                BuocPheDuyet buocTiepTheo = new BuocPheDuyet();
                // từ ThaoTacVuaThucHienId trong HSTT kiểm tra xem thao tác đó có cấu hình để nhảy đến bước khác không
                // nếu có thì bước tiếp theo trả ra sẽ là bước nhảy đến đó
                // nếu không thì bước tiếp theo trả ra là bước liền kề
                var thaoTacBuocPD = _context.ThaoTacBuocPheDuyet.FirstOrDefault(x => x.ThaoTacBuocPheDuyetId == check.ThaoTacVuaThucHienId);
                if (thaoTacBuocPD != null && thaoTacBuocPD.DiDenBuocPheDuyetId != null)
                {
                    buocTiepTheo = _context.BuocPheDuyet.FirstOrDefault(x => x.BuocPheDuyetId == thaoTacBuocPD.DiDenBuocPheDuyetId);
                }
                else
                {

                    if (listBuocPD.Count != 0 && buocPD.Count != 0)
                    {
                        thuTu = buocPD[0].ThuTu;
                        foreach (var item in listBuocPD)
                        {
                            // kiểm tra xem sau bước hiện tại (buocPD[0].ThuTu + 1) có còn bước nào không
                            // nếu còn thì gán bước phía sau đó vào buocTiepTheo 
                            if (item.ThuTu == buocPD[0].ThuTu + 1)
                            {
                                buocTiepTheo = item;
                            }
                        }
                        // nếu không còn thì buocTiepTheo sẽ được gán bằng bước hiện tại 
                        if (buocTiepTheo.ThuTu == 0)
                        {
                            buocTiepTheo = buocPD.FirstOrDefault();
                        }
                    }
                }
                
                //var CT = _context.ChungTu.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == id);
                DanhSachHoSoViewModel test = new DanhSachHoSoViewModel();
                var hoSoList = from hstt in _context.HoSoThanhToan
                                join tk in _context.TaiKhoanNganHang on hstt.TaiKhoanThuHuong.TaiKhoanId equals tk.TaiKhoanId
                               where hstt.HoSoId.ToString() == id
                               where hstt.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.XoaHoSo
                               select new DanhSachHoSoViewModel
                               {
                                   Id = hstt.HoSoId,
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
                                   nganHangThuHuongId =  hstt.NganHang.NganHangId,
                                   hinhThucTT = hstt.HinhThucTT,
                                   hinhThucChi = hstt.HinhThucChi,
                                   thoiGianLuutru = hstt.ThoiGianLuTru,
                                   //    tenNguoiTao = hstt.ChucNangChaId,
                                   ngayTiepNhan = hstt.NgayTiepNhan,
                                   thoiGianThanhToan = hstt.HanThanhToan,
                                   mucDoUuTien = hstt.MucDoUuTien,
                                   boPhanYeuCau =  hstt.BoPhanCauId.TenDonVi,
                                   boPhanYeuCauId =  hstt.BoPhanCauId.Id,
                                   LoaiHoSo =  hstt.LoaiHoSo.TenLoaiHoSo,
                                   LoaiHoSoId =  hstt.LoaiHoSo.LoaiHoSoId,
                                   tongTienDeNghiTT = hstt.SoTien,
                                   SoTienThucTe = hstt.SoTienThucTe,
                                   donViId =  hstt.DonViYeuCau.Id,
                                   donVi =  hstt.DonViYeuCau.TenDonVi,
                                   NguoiDuyetId = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.Id,
                                   //    vaiTroPheDuyet = hstt.Type
                                   TenVietTatHoSo = hstt.TenVietTat,
                                   CoHieuLuc = hstt.CoHieuLuc,
                                   CoBanCung = hstt.CoBanCung,
                                   LanThanhToan = hstt.LanThanhToan,
                                   TenYeuCauThanhToan = hstt.TenYeuCauThanhToan,
                                   TenNganHang =  hstt.NganHang.TenNganHang,
                                   tenNguoiTao = hstt.NguoiTao.FirstName + " " + hstt.NguoiTao.LastName,
                                   LoaiTienTe = hstt.LoaiTienTe,
                                   NgayThanhToan = hstt.NgayThanhToan,
                                   TenNguoiDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.UserName,
                                   NoiDungDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NoiDung,
                                   TKTaiKhoanId = tk.TaiKhoanId,
                                   TKTenTaiKhoan = tk.TenTaiKhoan,
                                   TKSoTk = tk.SoTaiKhoan,
                                   TKTrangThaiTaiKhoan = tk.TrangThai,
                                   ChiNhanhId = _context.ChiNhanhNganHang.FirstOrDefault(x => x.ChiNhanhNganHangId == tk.ChiNhanhNganHang.ChiNhanhNganHangId).ChiNhanhNganHangId,
                                   TenChiNhanh = _context.ChiNhanhNganHang.FirstOrDefault(x => x.ChiNhanhNganHangId == tk.ChiNhanhNganHang.ChiNhanhNganHangId).TenChiNhanh,
                                   ThucTeTenNguoiNT = _context.ChungTu.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == id).TenTaiKhoanNhan,
                                   ThucTeSoTaiKhoanNT = _context.ChungTu.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == id).SoTaiKhoanNhan,
                                   ThucTeSoTienNT = _context.ChungTu.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == id).SoTien,
                                   ThuTeTenNganHangNT = _context.ChungTu.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == id).TenNganHangNhan,
                                   ThuTeTenChiNhanhNganHangNT = _context.ChiNhanhNganHang.FirstOrDefault(x => x.MaChiNhanhErp == _context.ChungTu.FirstOrDefault(x => x.HoSoThanhToan.HoSoId.ToString() == id).MaChiNhanhNhan && x.DaXoa == false).TenChiNhanh,
                                   TaiLieuGoc = hstt.TaiLieuGoc,
                                   TaiLieuKy = hstt.TaiLieuKy,
                                   CapKy = hstt.CapKy,
                                   NgayKy = hstt.NgayKy,
                                   NguoiTaoId = hstt.NguoiTao.Id,
                                   TaiLieuKyId =  (from ks in _context.KySoTaiLieu
                                                    where (ks.HoSoThanhToan.HoSoId.ToString() == id)
                                                    && (ks.DaXoa == false)
                                                    orderby ks.CapKy
                                                    select ks.KySoTaiLieuId).ToList(),
                                    NoiDungThanhToan = hstt.NoiDungTT,
                                    HoSoThamChieu = (from TC in _context.HoSoThamChieu 
                                                    join TTHS in _context.HoSoThanhToan on TC.HoSoLienQuanId equals TTHS.HoSoId
                                                    where (TC.HoSoThanhToan.HoSoId.ToString() == id)
                                                    select new HoSoThamChieuViewModel
                                                    {
                                                        HoSoId = TTHS.HoSoId.ToString(),
                                                        MaSoHoSoTT = TTHS.MaHoSo,
                                                        TenHoSo = TTHS.TenHoSo,
                                                        SoTien = TTHS.SoTien,
                                                        SoTienThucTe = TTHS.SoTienThucTe,
                                                        TrangThaiHoSo = TTHS.TrangThaiHoSo
                                                    }).ToList(),
                                   BuocPheDuyet = new BuocPheDuyetChiTietViewModel
                                   {
                                       BuocPheDuyetId = buocTiepTheo.BuocPheDuyetId,
                                       QuyTrinhPheDuyetId = buocTiepTheo.QuyTrinhPheDuyetId,
                                       BuocPheDuyetTruocId = buocTiepTheo.BuocPheDuyetTruocId,
                                       BuocPheDuyetSauId = buocTiepTheo.BuocPheDuyetSauId,
                                       TrangThaiHoSo = buocTiepTheo.TrangThaiHoSo,
                                       TrangThaiChungTu = buocTiepTheo.TrangThaiChungTu,
                                       TenBuoc = buocTiepTheo.TenBuoc,
                                       ThuTu = buocTiepTheo.ThuTu,
                                       ThoiGianXuLy = buocTiepTheo.ThoiGianXuLy,
                                       DsThaoTacBuocPheDuyet = new List<ThaoTacBuocPheDuyetViewModel>()

                                   }
                               };
                var data = hoSoList.ToList();
                if (listBuocPD.Count != 0 && buocPD.Count != 0)
                {
                    var listThaoTac = _context.ThaoTacBuocPheDuyet.Where(x => x.BuocPheDuyetId == buocTiepTheo.BuocPheDuyetId).ToList();
                    foreach (var item in listThaoTac)
                    {
                        var thaoTac = new ThaoTacBuocPheDuyetViewModel
                        {
                            ThaoTacBuocPheDuyetId = item.ThaoTacBuocPheDuyetId,
                            BuocPheDuyetId = item.BuocPheDuyetId,
                            HanhDong = item.HanhDong,
                            KySo = item.KySo,
                            LoaiKy = item.LoaiKy,
                            GiayToId = item.GiayToId,
                            TrangThaiChungTu = item.TrangThaiChungTu,
                            TrangThaiHoSo = item.TrangThaiHoSo,
                            IsSendMail = item.IsSendMail
                        };
                        data[0].BuocPheDuyet.DsThaoTacBuocPheDuyet.Add(thaoTac);
                    }
                    data[0].BuocPheDuyet.DsNguoiThucHien = buocTiepTheo.NguoiThucHien.Split(",").Select(str => new Guid(str)).ToList();

                }
                return new ResponseHoSoViewModel(data, 200, 1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }
        public ResponseHoSoViewModel GetHoSoPaging(HoSoSearchViewModel request)
        {
            try
            {
                var hoSoList = from hstt in _context.HoSoThanhToan
                                   // join Pd in _context.PheDuyetHoSoTT on hstt.HoSoId equals Pd.HoSoThanhToan.HoSoId
                               where hstt.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.XoaHoSo
                               select new DanhSachHoSoViewModel
                               {
                                   Id = hstt.HoSoId,
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
                                   nganHangThuHuongId = hstt.NganHang.NganHangId,
                                   hinhThucTT = hstt.HinhThucTT,
                                   hinhThucChi = hstt.HinhThucChi,
                                   thoiGianLuutru = hstt.ThoiGianLuTru,
                                   //    tenNguoiTao = hstt.ChucNangChaId,
                                   ngayTiepNhan = hstt.NgayTiepNhan,
                                   thoiGianThanhToan = hstt.HanThanhToan,
                                   mucDoUuTien = hstt.MucDoUuTien,
                                   boPhanYeuCau = hstt.BoPhanCauId.TenDonVi,
                                   boPhanYeuCauId = hstt.BoPhanCauId.Id,
                                   LoaiHoSo =  hstt.LoaiHoSo.TenLoaiHoSo,
                                   LoaiHoSoId = hstt.LoaiHoSo.LoaiHoSoId,
                                   tongTienDeNghiTT = hstt.SoTien,
                                   SoTienThucTe = hstt.SoTienThucTe,
                                   donViId = hstt.DonViYeuCau.Id,
                                   donVi = hstt.DonViYeuCau.TenDonVi,
                                   NguoiDuyetId = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.Id,
                                   //    vaiTroPheDuyet = hstt.Type
                                   TenVietTatHoSo = hstt.TenVietTat,
                                   CoHieuLuc = hstt.CoHieuLuc,
                                   CoBanCung = hstt.CoBanCung,
                                   LanThanhToan = hstt.LanThanhToan,
                                   TenYeuCauThanhToan = hstt.TenYeuCauThanhToan,
                                   TenNganHang =  hstt.NganHang.TenNganHang,
                                   tenNguoiTao = hstt.NguoiTao.FirstName + " " + hstt.NguoiTao.LastName,
                                   NguoiTaoId = hstt.NguoiTao.Id,
                                   LoaiTienTe = hstt.LoaiTienTe,
                                   NgayThanhToan = hstt.NgayThanhToan,
                                   TenNguoiDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.UserName,
                                   NoiDungDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NoiDung,
                                   NoiDungThanhToan = hstt.NoiDungTT,
                                   NguoiThamTra = hstt.NguoiThamTra.FirstName + " " + hstt.NguoiTao.LastName,
                                   NguoiThamTraId = hstt.NguoiThamTra.Id,
                               };

                if (hoSoList.Any())
                {
                    if (request != null)
                    {
                        if (!String.IsNullOrEmpty(request.LoaiHoSo))
                        {
                            hoSoList = hoSoList.Where(s => s.LoaiHoSoId.ToString() == request.LoaiHoSo);
                        }
                        if (request.BoPhanYeuCau != 0)
                        {
                            hoSoList = hoSoList.Where(s => s.boPhanYeuCauId == request.BoPhanYeuCau);
                        }
                        if (request.TinhTrangPheDuyet != 0)
                        {
                            // if (request.TinhTrangPheDuyet == 1)
                            // {
                            //     hoSoList = hoSoList.Where(s => s.trangThaiHoSo == 1 || s.trangThaiHoSo == 3);
                            // }
                            // else if (request.TinhTrangPheDuyet == 2)
                            // {
                            //     hoSoList = hoSoList.Where(s => s.trangThaiHoSo == request.TinhTrangPheDuyet || s.trangThaiHoSo == 4 || s.trangThaiHoSo == 6);
                            // }
                            // else
                            // {
                            //     hoSoList = hoSoList.Where(s => s.trangThaiHoSo == request.TinhTrangPheDuyet);
                            // }
                            hoSoList = hoSoList.Where(s => s.trangThaiHoSo == request.TinhTrangPheDuyet);
                        }
                        // màn hình trạng thái phê duyệt
                        if (request.DaPheDuyet != 0)
                        {
                            if (request.DaPheDuyet == 1)
                            {
                                hoSoList = hoSoList.Where(s => s.trangThaiHoSo == BCXN.Statics.TrangThaiHoSo.ChuaPheDuyet);
                            }
                            else if (request.DaPheDuyet == 2)
                            {
                                hoSoList = hoSoList.Where(s => s.trangThaiHoSo == BCXN.Statics.TrangThaiHoSo.DaPheDuyet || s.trangThaiHoSo == BCXN.Statics.TrangThaiHoSo.DaThanhToan || s.trangThaiHoSo == BCXN.Statics.TrangThaiHoSo.DaTaoChungTu);
                                if(request.TinhTrangPheDuyet != 0)
                                {
                                    hoSoList = hoSoList.Where(s => s.trangThaiHoSo == request.TinhTrangPheDuyet);
                                }
                            }
                            else if (request.DaPheDuyet == 3)
                            {
                                hoSoList = hoSoList.Where(s => s.trangThaiHoSo != BCXN.Statics.TrangThaiHoSo.KhoiTao || s.trangThaiHoSo != BCXN.Statics.TrangThaiHoSo.YeuCauThaiDoi || s.trangThaiHoSo != BCXN.Statics.TrangThaiHoSo.ChuaTiepNhan);
                            }
                        }
                        if (request.ThamTraHoSo != null){
                            hoSoList = hoSoList.Where(s => s.trangThaiHoSo == request.ThamTraHoSo);
                        }
                        if (request.TuNgay != DateTime.MinValue)
                        {
                            hoSoList = hoSoList.Where(x => x.ngayGui >= request.TuNgay);
                        }
                        if (request.DenNgay != DateTime.MinValue)
                        {
                            hoSoList = hoSoList.Where(x => x.ngayGui <= request.DenNgay);
                        }
                        if (!String.IsNullOrEmpty(request.TuKhoa))
                        {
                            hoSoList = hoSoList.Where(s => s.maHoSo.Contains(request.TuKhoa) || s.tenVietTat.Contains(request.TuKhoa));
                        }
                        
                        // lọc hồ sơ theo id người tạo hoặc theo id người được phân công thẩm tra 
                        if (!String.IsNullOrEmpty(request.UserId)){
                            if (request.ThamTraHoSo != null && request.ThamTraHoSo == BCXN.Statics.TrangThaiHoSo.ThamTraHoSo){
                                hoSoList = hoSoList.Where(s => s.NguoiThamTraId.ToLower() == request.UserId.ToLower());
                            }else {
                               hoSoList = hoSoList.Where(s => s.NguoiTaoId.ToLower() == request.UserId.ToLower());
                            }
                        }
                        hoSoList = hoSoList.OrderByDescending(x => x.ngayTao);

                        var totalRecord = hoSoList.ToList().Count();
                        if (request.PageIndex > 0)
                        {
                            hoSoList = hoSoList.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                        }
                        var hoSoPaging = hoSoList.ToList();
                        if (totalRecord == 0)
                        {
                            return new ResponseHoSoViewModel(hoSoPaging, 204, totalRecord);
                        }
                        return new ResponseHoSoViewModel(hoSoPaging, 200, totalRecord);
                    }
                    else
                    {
                        return new ResponseHoSoViewModel(null, 204, 0);
                    }
                }
                else
                {
                    return new ResponseHoSoViewModel(null, 204, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }

        public ResponsePostViewModel ApproveHoSo(ApproveHoSoTT request)
        {
            try
            {
                var nguoiPheDuyet = _context.Users.FirstOrDefault(x => x.Id == request.NguoiThucHienId);
                var hoSoDetail = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoId);
                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoDetail.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoDetail.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hoSoDetail.QuaTrinhPheDuyetId).ToList();
                var thuTu = 0;
                BuocPheDuyet buocHienTai = new BuocPheDuyet();
                if (request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.PheDuyetToTrinh 
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.ThamTraHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoi 
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiPheDuyetToTrinh
                    //văn phòng phẩm
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiThamTraHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiTiepNhanHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiPhanCongNguoiPheDuyet)
                {
                    if (listBuocPD.Count != 0 && buocPD.Count != 0)
                    {
                        thuTu = buocPD[0].ThuTu;
                        foreach (var item in listBuocPD)
                        {
                            if ((item.ThuTu == buocPD[0].ThuTu + 1))
                            {
                                buocHienTai = item;
                            }
                        }
                    }
                    hoSoDetail.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                    hoSoDetail.ThaoTacVuaThucHienId = request.ThaoTacBuocPheDuyetId;
                    QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                    {
                        BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                        HoSoId = hoSoDetail.HoSoId,
                        ThoiGianTao = DateTime.Now,
                        TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                        ThoiGianXuLy = DateTime.Now,
                        NguoiXuLyId = new Guid(nguoiPheDuyet.Id),
                    };
                    _context.QuaTrinhPheDuyet.Add(qt);
                }
                
                var hoSoDetailJoin = from hstt in _context.HoSoThanhToan
                                     join lhs in _context.LoaiHoSo on hstt.LoaiHoSo.LoaiHoSoId equals lhs.LoaiHoSoId
                                     join dv in _context.DonVi on hstt.DonViYeuCau.Id equals dv.Id
                                     join nn in _context.NganHang on hstt.NganHang.NganHangId equals nn.NganHangId
                                     where hstt.HoSoId.ToString() == request.HoSoId
                                     select new
                                     {
                                         hstt = hstt,
                                         loaiHSId = lhs.LoaiHoSoId,
                                         donViId = dv.Id,
                                         nnId = nn.NganHangId
                                     };
                if (hoSoDetail == null)
                {
                    return new ResponsePostViewModel("không tìm thấy hồ sơ", 400);
                }
                var hoSoDetailList = hoSoDetailJoin.ToList();
                // thêm thông tin vào bảng PheDuyetHoSoTT
                // trạng thái phê duyệt: 2 phê duyêt, 3 từ chối, 4 đóng hồ sơ, 5 phê duyêt tờ trình, 6 từ chối tờ trình

                PheDuyetHoSoTT pd = new PheDuyetHoSoTT() { };
                pd.PheDuyetHoSoTTId = new Guid();
                pd.HoSoThanhToan = hoSoDetail;
                pd.NguoiThucHien = nguoiPheDuyet;
                pd.NgayThucHien = DateTime.Now;
                pd.BuocThucHien = request.BuocThucHien;

                var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                        .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);
                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {request.ThaoTacBuocPheDuyetId}");
                }

                if (request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.PheDuyetToTrinh 
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.ThamTraHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoi
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiPheDuyetToTrinh)
                {
                    pd.TrangThaiHoSo = thaoTacBuocPheDuyet.TrangThaiHoSo; //lấy qua thao tác bước phê duyệt
                }
                else
                {
                    pd.TrangThaiHoSo = request.TrangThaiHoSo.ToString(); //lấy trực tiếp truyền vào
                }

                pd.TrangThaiPheDuyet = request.TrangThaiPheDuyet.ToString();
                pd.NoiDung = request.NoiDung;
                _context.PheDuyetHoSoTT.Add(pd);

                // cập nhật trạng thái của hồ sơ trong bảng HoSoThanhToan
                if (!string.IsNullOrEmpty(request.NguoiThamTraId))
                {
                    hoSoDetail.NguoiThamTra = _context.Users.FirstOrDefault(x => x.Id == request.NguoiThamTraId);
                }

                if (!string.IsNullOrEmpty(request.NguoiPheDuyetHoSoId))
                {
                    hoSoDetail.NguoiPheDuyetHoSoId = request.NguoiPheDuyetHoSoId;
                }

                hoSoDetail.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                hoSoDetail.ThaoTacVuaThucHienId = request.ThaoTacBuocPheDuyetId;
                hoSoDetail.NgayDuyet = DateTime.Now;

                // thêm thông tin vào bảng lịch sử(LichSuHoSoTT)
                LichSuHoSoTT tam = new LichSuHoSoTT() { };
                tam.LichSuId = Guid.NewGuid();
                tam.HoSoThanhToan = hoSoDetailList[0].hstt;
                tam.LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoDetailList[0].loaiHSId);
                tam.MaHoSo = hoSoDetail.MaHoSo;
                tam.TenHoSo = hoSoDetail.TenHoSo;
                tam.NamHoSo = hoSoDetail.NamHoSo;
                tam.ThoiGianLuTru = hoSoDetail.ThoiGianLuTru;
                tam.MucDoUuTien = hoSoDetail.MucDoUuTien;
                tam.HanThanhToan = hoSoDetail.HanThanhToan;
                tam.GhiChu = hoSoDetail.GhiChu;
                tam.DonViYeuCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.BoPhanCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.NgayGui = hoSoDetail.NgayGui;
                tam.NgayTiepNhan = hoSoDetail.NgayTiepNhan;
                tam.NgayDuyet = DateTime.Now;
                tam.SoTien = hoSoDetail.SoTien;
                tam.NgayLapCT = hoSoDetail.NgayLapCT;
                if (request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.PheDuyetToTrinh 
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.ThamTraHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoi
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiPheDuyetToTrinh
                    //văn phòng phẩm
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiThamTraHoSo)
                {
                    tam.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo); //lấy qua thao tác bước phê duyệt
                }
                else
                {
                    tam.TrangThaiHoSo = request.TrangThaiHoSo; //lấy trạng thái hồ sơ trực tiếp
                }
                tam.BuocThucHien = hoSoDetail.BuocThucHien;
                tam.NgayTao = DateTime.Now;
                tam.NguoiThuHuong = hoSoDetail.NguoiThuHuong;
                tam.SoTienThucTe = hoSoDetail.SoTienThucTe;
                tam.SoTKThuHuong = hoSoDetail.SoTKThuHuong;
                tam.NganHangThuHuong = _context.NganHang.FirstOrDefault(x => x.NganHangId == hoSoDetailList[0].nnId).NganHangId;
                tam.HinhThucTT = hoSoDetail.HinhThucTT;
                tam.ThoiGianCapNhat = DateTime.Now;
                // tam.NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => (x.Id == hoSoDetail.));
                _context.LichSuHoSoTT.Add(tam);
                _context.SaveChanges();
                if (request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.PheDuyetToTrinh 
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.ThamTraHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoi 
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiPheDuyetToTrinh
                    //văn phòng phẩm
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiThamTraHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiTiepNhanHoSo
                    && request.TrangThaiPheDuyet != TrangThaiPheDuyetHoSo.TuChoiPhanCongNguoiPheDuyet)
                {
                    _utilsservice.SendMailKy(request.ThaoTacBuocPheDuyetId, new Guid(request.HoSoId));
                }
                return new ResponsePostViewModel("Chuyển đổi trạng thái hồ sơ thành công", 201);
            }
            catch (System.Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
        public ResponsePheDuyetHoSoTTViewModel GetPheDuyetHoSoTTById(string id)
        {
            try
            {
                var listPdHosoTT = from Pd in _context.PheDuyetHoSoTT
                                   where Pd.HoSoThanhToan.HoSoId.ToString() == id
                                   select new PheDuyetHoSoTTViewModel
                                   {
                                       PheDuyetHoSoTTId = Pd.PheDuyetHoSoTTId.ToString(),
                                       HoSoThanhToanId = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == Pd.HoSoThanhToan.HoSoId).HoSoId.ToString(),
                                       NguoiThucHienId = _context.ApplicationUser.FirstOrDefault(x => x.Id == Pd.NguoiThucHien.Id).Id.ToString(),
                                       TenNguoiThucHien = _context.ApplicationUser.FirstOrDefault(x => x.Id == Pd.NguoiThucHien.Id).FirstName +" "+_context.ApplicationUser.FirstOrDefault(x => x.Id == Pd.NguoiThucHien.Id).LastName,
                                       NgayThucHien = Pd.NgayThucHien,
                                       TrangThaiHoSo = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == Pd.HoSoThanhToan.HoSoId).TrangThaiHoSo,
                                       TrangThaiPheDuyet = Pd.TrangThaiPheDuyet,
                                       BuocThucHien = Pd.BuocThucHien,
                                       NoiDung = Pd.NoiDung,
                                   };
                listPdHosoTT = listPdHosoTT.OrderBy(x => x.NgayThucHien);
                //listPdHosoTT = listPdHosoTT.OrderBy(x => x.NoiDung);
                if (listPdHosoTT == null)
                {
                    return new ResponsePheDuyetHoSoTTViewModel(null, 204, 0);
                }
                return new ResponsePheDuyetHoSoTTViewModel(listPdHosoTT.ToList(), 200, listPdHosoTT.Count());
            }
            catch (Exception e)
            {
                _logger.LogError("Lỗi:", e);
                return null;
            }
        }
        public ResponseHoSoViewModel GetHoSoTTByMHSTT(string MaSoHoSoTT)
        {
            try
            {
                var check = _context.HoSoThanhToan.FirstOrDefault(x => x.MaHoSo.ToString().ToUpper() == MaSoHoSoTT.ToUpper());
                if (check == null)
                {
                    return new ResponseHoSoViewModel(null, 204, 0);
                }
                var hoSoList = from hstt in _context.HoSoThanhToan
                               where hstt.MaHoSo.ToString() == MaSoHoSoTT
                               where hstt.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.XoaHoSo
                               select new DanhSachHoSoViewModel
                               {
                                   Id = hstt.HoSoId,
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
                                   nganHangThuHuongId = hstt.NganHang.NganHangId,
                                   hinhThucTT = hstt.HinhThucTT,
                                   hinhThucChi = hstt.HinhThucChi,
                                   thoiGianLuutru = hstt.ThoiGianLuTru,
                                   //    tenNguoiTao = hstt.ChucNangChaId,
                                   ngayTiepNhan = hstt.NgayTiepNhan,
                                   thoiGianThanhToan = hstt.HanThanhToan,
                                   mucDoUuTien = hstt.MucDoUuTien,
                                   boPhanYeuCau = hstt.BoPhanCauId.TenDonVi,
                                   boPhanYeuCauId =  hstt.BoPhanCauId.Id,
                                   LoaiHoSo =  hstt.LoaiHoSo.TenLoaiHoSo,
                                   LoaiHoSoId =  hstt.LoaiHoSo.LoaiHoSoId,
                                   tongTienDeNghiTT = hstt.SoTien,
                                   SoTienThucTe = hstt.SoTienThucTe,
                                   donViId =  hstt.BoPhanCauId.Id,
                                   donVi =  hstt.BoPhanCauId.TenDonVi,
                                   NguoiDuyetId = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.Id,
                                   //    vaiTroPheDuyet = hstt.Type
                                   TenVietTatHoSo = hstt.TenVietTat,
                                   CoHieuLuc = hstt.CoHieuLuc,
                                   CoBanCung = hstt.CoBanCung,
                                   LanThanhToan = hstt.LanThanhToan,
                                   TenYeuCauThanhToan = hstt.TenYeuCauThanhToan,
                                   TenNganHang =  hstt.NganHang.TenNganHang,
                                   tenNguoiTao =  hstt.NguoiTao.UserName,
                                   LoaiTienTe = hstt.LoaiTienTe,
                                   NgayThanhToan = hstt.NgayThanhToan,
                                   TenNguoiDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.UserName,
                                   NoiDungDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NoiDung,
                                   NoiDungThanhToan = hstt.NoiDungTT,
                               };
                return new ResponseHoSoViewModel(hoSoList.ToList(), 200, 1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }
        public ResponsePostViewModel UpDateTTHoSoTT(string MaSoHoSoTT, int TrangThai, DateTime NgayThanhToan)
        {
            try
            {
                var Item = _context.HoSoThanhToan.FirstOrDefault(x => x.MaHoSo == MaSoHoSoTT);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                Item.TrangThaiHoSo = TrangThai;
                Item.NgayThanhToan = NgayThanhToan;
                // thêm thông tin vào bảng lịch sử(LichSuHoSoTT)
                var hoSoDetailJoin = from hstt in _context.HoSoThanhToan
                                     join lhs in _context.LoaiHoSo on hstt.LoaiHoSo.LoaiHoSoId equals lhs.LoaiHoSoId
                                     join dv in _context.DonVi on hstt.DonViYeuCau.Id equals dv.Id
                                     join nn in _context.NganHang on hstt.NganHang.NganHangId equals nn.NganHangId
                                     where hstt.HoSoId == Item.HoSoId
                                     select new
                                     {
                                         hstt = hstt,
                                         loaiHSId = lhs.LoaiHoSoId,
                                         donViId = dv.Id,
                                         nnId = nn.NganHangId,
                                     };
                var hoSoDetailList = hoSoDetailJoin.ToList();
                LichSuHoSoTT tam = new LichSuHoSoTT() { };
                tam.LichSuId = Guid.NewGuid();
                tam.HoSoThanhToan = hoSoDetailList[0].hstt;
                tam.LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoDetailList[0].loaiHSId);
                tam.MaHoSo = Item.MaHoSo;
                tam.TenHoSo = Item.TenHoSo;
                tam.NamHoSo = Item.NamHoSo;
                tam.ThoiGianLuTru = Item.ThoiGianLuTru;
                tam.MucDoUuTien = Item.MucDoUuTien;
                tam.HanThanhToan = Item.HanThanhToan;
                tam.GhiChu = Item.GhiChu;
                tam.DonViYeuCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.BoPhanCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.NgayGui = Item.NgayGui;
                tam.NgayTiepNhan = Item.NgayTiepNhan;
                tam.NgayDuyet = DateTime.Now;
                tam.SoTien = Item.SoTien;
                tam.NgayLapCT = DateTime.Now;
                tam.TrangThaiHoSo = TrangThai;
                tam.BuocThucHien = Item.BuocThucHien;
                tam.NgayTao = DateTime.Now;
                tam.NguoiThuHuong = Item.NguoiThuHuong;
                tam.SoTienThucTe = Item.SoTienThucTe;
                tam.SoTKThuHuong = Item.SoTKThuHuong;
                tam.NganHangThuHuong = _context.NganHang.FirstOrDefault(x => x.NganHangId == hoSoDetailList[0].nnId).NganHangId;
                tam.HinhThucTT = Item.HinhThucTT;
                tam.ThoiGianCapNhat = DateTime.Now;
                // tam.NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => (x.Id == hosottItem.NguoiTao.Id));
                _context.LichSuHoSoTT.Add(tam);
                _context.SaveChanges();

                return new ResponsePostViewModel("cập nhật trạng thái thành công", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }
        public ResponseHoSoViewModel GetHoSoTTByCTId(Guid ChungTuId)
        {
            try
            {
                var hoSoDetailJoin = from hstt in _context.HoSoThanhToan
                                     join lhs in _context.ChungTu on hstt.HoSoId equals lhs.HoSoThanhToan.HoSoId
                                     where lhs.ChungTuId == ChungTuId
                                     select new DanhSachHoSoViewModel
                                     {
                                         Id = hstt.HoSoId,
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
                                         nganHangThuHuongId =  hstt.NganHang.NganHangId,
                                         hinhThucTT = hstt.HinhThucTT,
                                         hinhThucChi = hstt.HinhThucChi,
                                         thoiGianLuutru = hstt.ThoiGianLuTru,
                                         //    tenNguoiTao = hstt.ChucNangChaId,
                                         ngayTiepNhan = hstt.NgayTiepNhan,
                                         thoiGianThanhToan = hstt.HanThanhToan,
                                         mucDoUuTien = hstt.MucDoUuTien,
                                         boPhanYeuCau =  hstt.BoPhanCauId.TenDonVi,
                                         boPhanYeuCauId =  hstt.BoPhanCauId.Id,
                                         LoaiHoSo =  hstt.LoaiHoSo.TenLoaiHoSo,
                                         LoaiHoSoId = hstt.LoaiHoSo.LoaiHoSoId,
                                         tongTienDeNghiTT = hstt.SoTien,
                                         SoTienThucTe = hstt.SoTienThucTe,
                                         donViId =  hstt.BoPhanCauId.Id,
                                         donVi =  hstt.BoPhanCauId.TenDonVi,
                                         NguoiDuyetId = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.Id,
                                         //    vaiTroPheDuyet = hstt.Type
                                         TenVietTatHoSo = hstt.TenVietTat,
                                         CoHieuLuc = hstt.CoHieuLuc,
                                         CoBanCung = hstt.CoBanCung,
                                         LanThanhToan = hstt.LanThanhToan,
                                         TenYeuCauThanhToan = hstt.TenYeuCauThanhToan,
                                         TenNganHang =  hstt.NganHang.TenNganHang,
                                         tenNguoiTao =  hstt.NguoiTao.UserName,
                                         LoaiTienTe = hstt.LoaiTienTe,
                                         NgayThanhToan = hstt.NgayThanhToan,
                                         TenNguoiDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NguoiThucHien.UserName,
                                         NoiDungDuyet = _context.PheDuyetHoSoTT.FirstOrDefault(x => x.HoSoThanhToan.HoSoId == hstt.HoSoId).NoiDung,
                                         NoiDungThanhToan = hstt.NoiDungTT,
                                     };
                return new ResponseHoSoViewModel(hoSoDetailJoin.ToList(), 200, 1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }

        public List<ThongTinTaiKhoanByHoSoIdViewModel> GetThongTinTaiKhoanByHoSoId(Guid HoSoId)
        {
            try
            { 
                var hoSoDetail = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == HoSoId);
                var taiKhoan = from tk in _context.TaiKhoanNganHang
                                join hstt in _context.HoSoThanhToan on tk.TaiKhoanId equals hstt.TaiKhoanThuHuong.TaiKhoanId
                                join nh in _context.NganHang on tk.NganHang.NganHangId equals nh.NganHangId
                                join cn in _context.ChiNhanhNganHang on tk.ChiNhanhNganHang.ChiNhanhNganHangId equals cn.ChiNhanhNganHangId
                                select new ThongTinTaiKhoanByHoSoIdViewModel{
                                     TenTaiKhoan = tk.TenTaiKhoan,
                                     SoTaiKhoan = tk.SoTaiKhoan,
                                     MaNganHang = nh.MaNganHang,
                                     TenNganHang = nh.TenNganHang,
                                     MaChiNhanh = cn.MaChiNhanhErp
                                };
                 return taiKhoan.ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return null;
            }
        }
        public ResponsePostViewModel UpdateFileHoSoTT(CreateFileHoSoTT request)
        {
            try
            {
                var Item = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId.ToString() == request.HoSoId);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                switch(Item.TrangThaiHoSo) 
                {
                    case BCXN.Statics.TrangThaiHoSo.ChuaPheDuyet:
                    case BCXN.Statics.TrangThaiHoSo.DaPheDuyet:
                    case BCXN.Statics.TrangThaiHoSo.DaThanhToan:
                    case BCXN.Statics.TrangThaiHoSo.DaTaoChungTu:
                    case BCXN.Statics.TrangThaiHoSo.DongHoSo:
                    case BCXN.Statics.TrangThaiHoSo.XoaHoSo:
                        return new ResponsePostViewModel("Không được cập nhật hồ sơ", 400);
                    //break;
                }
                Item.TaiLieuGoc = request.UrlFile;
               // Item.TaiLieuKy = request.UrlFile;
                Item.TrangThaiHoSo = request.TrangThai;
                Item.CapKy = request.CapKy;

                // thêm thông tin vào bảng lịch sử(LichSuHoSoTT)
                var hoSoDetailJoin = from hstt in _context.HoSoThanhToan
                                     join lhs in _context.LoaiHoSo on hstt.LoaiHoSo.LoaiHoSoId equals lhs.LoaiHoSoId
                                     join dv in _context.DonVi on hstt.DonViYeuCau.Id equals dv.Id
                                     join nn in _context.NganHang on hstt.NganHang.NganHangId equals nn.NganHangId
                                     where hstt.HoSoId == Item.HoSoId
                                     select new
                                     {
                                         hstt = hstt,
                                         loaiHSId = lhs.LoaiHoSoId,
                                         donViId = dv.Id,
                                         nnId = nn.NganHangId,
                                     };
                var hoSoDetailList = hoSoDetailJoin.ToList();
                LichSuHoSoTT tam = new LichSuHoSoTT() { };
                tam.LichSuId = Guid.NewGuid();
                tam.HoSoThanhToan = hoSoDetailList[0].hstt;
                tam.LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoDetailList[0].loaiHSId);
                tam.MaHoSo = Item.MaHoSo;
                tam.TenHoSo = Item.TenHoSo;
                tam.NamHoSo = Item.NamHoSo;
                tam.ThoiGianLuTru = Item.ThoiGianLuTru;
                tam.MucDoUuTien = Item.MucDoUuTien;
                tam.HanThanhToan = Item.HanThanhToan;
                tam.GhiChu = Item.GhiChu;
                tam.DonViYeuCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.BoPhanCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.NgayGui = Item.NgayGui;
                tam.NgayTiepNhan = Item.NgayTiepNhan;
                tam.NgayDuyet = DateTime.Now;
                tam.SoTien = Item.SoTien;
                tam.NgayLapCT = DateTime.Now;
                tam.TrangThaiHoSo = request.TrangThai;
                tam.BuocThucHien = Item.BuocThucHien;
                tam.NgayTao = DateTime.Now;
                tam.NguoiThuHuong = Item.NguoiThuHuong;
                tam.SoTienThucTe = Item.SoTienThucTe;
                tam.SoTKThuHuong = Item.SoTKThuHuong;
                tam.NganHangThuHuong = _context.NganHang.FirstOrDefault(x => x.NganHangId == hoSoDetailList[0].nnId).NganHangId;
                tam.HinhThucTT = Item.HinhThucTT;
                tam.ThoiGianCapNhat = DateTime.Now;
                // tam.NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => (x.Id == hosottItem.NguoiTao.Id));
                _context.LichSuHoSoTT.Add(tam);
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePaging GetAllHoSoThamChieu(HoSoThamChieuSearchViewModel request)
        {
            try
            { 
                var hoSoThamChieu =  from HSTC in _context.HoSoThanhToan 
                                     where HSTC.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.XoaHoSo && (string.IsNullOrEmpty(request.LoaiHoSo)? HSTC.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.XoaHoSo: HSTC.LoaiHoSo.LoaiHoSoId.ToString() == request.LoaiHoSo)
                           select new HoSoThamChieuViewModel
                           {
                               HoSoId = HSTC.HoSoId.ToString(),
                               MaSoHoSoTT =  HSTC.MaHoSo,
                               TenHoSo = HSTC.TenHoSo,
                               SoTien = HSTC.SoTien,
                               SoTienThucTe = HSTC.SoTienThucTe,
                               TrangThaiHoSo = HSTC.TrangThaiHoSo
                           };
                if (hoSoThamChieu.Any())
                {
                    if (request != null)
                    {
                        if (!string.IsNullOrEmpty(request.TrangThaiHoSo))
                        {
                            hoSoThamChieu = hoSoThamChieu.Where(s => s.TrangThaiHoSo == Convert.ToInt32(request.TrangThaiHoSo));
                        }
                        if (!String.IsNullOrEmpty(request.TuKhoa))
                        {
                            hoSoThamChieu = hoSoThamChieu.Where(s => s.MaSoHoSoTT.Contains(request.TuKhoa) || s.TenHoSo.Contains(request.TuKhoa));
                        }
                        hoSoThamChieu = hoSoThamChieu.OrderBy(x => x.TenHoSo);

                        var totalRecord = hoSoThamChieu.ToList().Count();
                        if(request.PageSize != 0){
                            if (request.PageIndex > 0)
                            {
                                hoSoThamChieu = hoSoThamChieu.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                            }
                        }
                        var hoSoThamChieuPaging = hoSoThamChieu.ToList();
                        if (totalRecord == 0)
                        {
                            return new ResponsePaging(message:"Không lấy dữ liệu", data:hoSoThamChieuPaging.ToList() ,errorcode:"01", success:false, totalRecord:totalRecord );
                        }
                        return new ResponsePaging(message:"Lấy dữ liệu thành công", data:hoSoThamChieuPaging.ToList() ,errorcode:"00", success:true, totalRecord:totalRecord );
                    }
                    else{
                        return new ResponsePaging(message:"Không lấy dữ liệu", data:null ,errorcode:"01", success:false, totalRecord:0 );
                    }
                }
                else {
                    return new ResponsePaging(message:"Không lấy dữ liệu", data:null ,errorcode:"01", success:false, totalRecord:0 );
                }      
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Lỗi:", ex);
                return new ResponsePaging(message:"Lỗi lấy dữ liệu", data: null ,errorcode:"02", success:false, totalRecord:0);
            }
        }

        public ResponsePostViewModel UpdateTrangThaiHsttById(Guid id, int TrangThai)
        {
            try
            {
                var Item = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == id);
                if (Item == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy dữ liệu", 400);
                }
                Item.TrangThaiHoSo = TrangThai;
                
                var hoSoDetailJoin = from hstt in _context.HoSoThanhToan
                                     join lhs in _context.LoaiHoSo on hstt.LoaiHoSo.LoaiHoSoId equals lhs.LoaiHoSoId
                                     join dv in _context.DonVi on hstt.DonViYeuCau.Id equals dv.Id
                                     join nn in _context.NganHang on hstt.NganHang.NganHangId equals nn.NganHangId
                                     where hstt.HoSoId == Item.HoSoId
                                     select new
                                     {
                                         hstt = hstt,
                                         loaiHSId = lhs.LoaiHoSoId,
                                         donViId = dv.Id,
                                         nnId = nn.NganHangId,
                                     };
                var hoSoDetailList = hoSoDetailJoin.ToList();
                LichSuHoSoTT tam = new LichSuHoSoTT() { };
                tam.LichSuId = Guid.NewGuid();
                tam.HoSoThanhToan = hoSoDetailList[0].hstt;
                tam.LoaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoDetailList[0].loaiHSId);
                tam.MaHoSo = Item.MaHoSo;
                tam.TenHoSo = Item.TenHoSo;
                tam.NamHoSo = Item.NamHoSo;
                tam.ThoiGianLuTru = Item.ThoiGianLuTru;
                tam.MucDoUuTien = Item.MucDoUuTien;
                tam.HanThanhToan = Item.HanThanhToan;
                tam.GhiChu = Item.GhiChu;
                tam.DonViYeuCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.BoPhanCau = _context.DonVi.FirstOrDefault(x => x.Id == hoSoDetailList[0].donViId);
                tam.NgayGui = Item.NgayGui;
                tam.NgayTiepNhan = Item.NgayTiepNhan;
                tam.NgayDuyet = DateTime.Now;
                tam.SoTien = Item.SoTien;
                tam.NgayLapCT = DateTime.Now;
                tam.TrangThaiHoSo = TrangThai;
                tam.BuocThucHien = Item.BuocThucHien;
                tam.NgayTao = DateTime.Now;
                tam.NguoiThuHuong = Item.NguoiThuHuong;
                tam.SoTienThucTe = Item.SoTienThucTe;
                tam.SoTKThuHuong = Item.SoTKThuHuong;
                tam.NganHangThuHuong = _context.NganHang.FirstOrDefault(x => x.NganHangId == hoSoDetailList[0].nnId).NganHangId;
                tam.HinhThucTT = Item.HinhThucTT;
                tam.ThoiGianCapNhat = DateTime.Now;
                // tam.NguoiCapNhat = _context.ApplicationUser.FirstOrDefault(x => (x.Id == hosottItem.NguoiTao.Id));
                _context.LichSuHoSoTT.Add(tam);
                _context.SaveChanges();
                return new ResponsePostViewModel("Cập nhật thành công", 200);
            }
            catch (Exception e)
            {
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }

        public ResponsePostViewModel PhanCongXuLy(ModelRequest.PhanCongXuLy request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var hoSoItem = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == request.HoSoId);
                if(hoSoItem == null) {
                    return new ResponsePostViewModel("Không tìm thấy hồ sơ", 400);
                }
                var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoItem.QuyTrinhPheDuyetId).ToList();
                var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hoSoItem.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hoSoItem.QuaTrinhPheDuyetId).ToList();
                BuocPheDuyet buocHienTai = new BuocPheDuyet();
                var thuTu = 0;
                if (listBuocPD.Count != 0 && buocPD.Count != 0)
                    {
                        thuTu = buocPD[0].ThuTu;
                        foreach (var item in listBuocPD)
                        { 
                            // nếu không thì quá trình phê duyệt trong bảng hồ sơ sẽ là bước tiếp theo
                            if ((item.ThuTu == buocPD[0].ThuTu + 1))
                            {
                                buocHienTai = item;
                            }
                        }
                        QuaTrinhPheDuyet qt = new QuaTrinhPheDuyet
                        {
                            BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                            HoSoId = hoSoItem.HoSoId,
                            ThoiGianTao = DateTime.Now,
                            TrangThaiXuLy = 1,
                            ThoiGianXuLy = DateTime.Now,
                            NguoiXuLyId = new Guid(_context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiPhanCongId)).Id),
                        };
                        hoSoItem.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                        // hoSoItem.ThaoTacVuaThucHienId = request.ThaoTacBuocPheDuyetId;

                        _context.QuaTrinhPheDuyet.Add(qt);
                    }
                hoSoItem.NguoiThamTra = _context.Users.FirstOrDefault(x => x.Id == request.NguoiDuocPhanCongId);
                _utilsservice.SendMailKy(new Guid(request.ThaoTacBuocPheDuyetId), hoSoItem.HoSoId);
                _context.SaveChanges();
                transaction.Commit();
                return new ResponsePostViewModel("Phân công người xử lý thành công", 200);
            }
            catch (System.Exception ex)
            {
                transaction.Rollback();
                return new ResponsePostViewModel(ex.Message, 500);
            }
        }

    }
}