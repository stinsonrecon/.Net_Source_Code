using BCXN.Data;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.Repositories;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epayment.Services
{
    /// <summary>
    /// Xử lý quy trình phê duyệt động
    /// </summary>
    public class QuyTrinhPheDuyetService
    {
        private readonly QuyTrinhPheDuyetRepository _quyTrinhPheDuyetRepo;
        private readonly BuocPheDuyetRepository _buocPheDuyetRepo;
        private readonly QuaTrinhPheDuyetRepository _quaTrinhPheDuyetRepo;
        private readonly ThaoTacBuocPheDuyetRepository _thaoTacBuocPheDuyetRepo;
        private readonly IHoSoThanhToanRepository _hoSoThanhToanRepo;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public QuyTrinhPheDuyetService(
            ILogger<QuyTrinhPheDuyetService> logger,
            QuyTrinhPheDuyetRepository quyTrinhPheDuyetRepo,
            BuocPheDuyetRepository buocPheDuyetRepo,
            QuaTrinhPheDuyetRepository quaTrinhPheDuyetRepo,
            ThaoTacBuocPheDuyetRepository thaoTacBuocPheDuyetRepo,
            IHoSoThanhToanRepository hoSoThanhToanRepo,
            ApplicationDbContext context
            )
        {
            _logger = logger;
            _quyTrinhPheDuyetRepo = quyTrinhPheDuyetRepo;
            _buocPheDuyetRepo = buocPheDuyetRepo;
            _quaTrinhPheDuyetRepo = quaTrinhPheDuyetRepo;
            _thaoTacBuocPheDuyetRepo = thaoTacBuocPheDuyetRepo;
            _hoSoThanhToanRepo = hoSoThanhToanRepo;
            _context = context;
        }

        /// <summary>
        /// Thêm mới quy trình
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponsePostViewModel CreateQuyTrinh(CreateQuyTrinhPheDuyetViewModel input)
        {
            try
            {
                _quyTrinhPheDuyetRepo.GetDbQueryTable().Add(new QuyTrinhPheDuyet
                {
                    LoaiHoSoId = input.LoaiHoSoId,
                    TenQuyTrinh = input.TenQuyTrinh,
                    NgayHieuLuc = input.NgayHieuLuc,
                    MoTa = input.MoTa,
                    TrangThai = true
                });
                _quyTrinhPheDuyetRepo.GetDbContext().SaveChanges();
                return new ResponsePostViewModel("Thêm quy trình phê duyệt mới thành công", 200);
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        /// <summary>
        /// Xóa quy trình
        /// </summary>
        /// <param name="quyTrinhId"></param>
        /// <returns></returns>
        public ResponsePostViewModel DeleteQuyTrinh(Guid quyTrinhId)
        {
            try
            {
                var quyTrinh = _quyTrinhPheDuyetRepo.GetDbQueryTable()
                    .FirstOrDefault(q => q.QuyTrinhPheDuyetId == quyTrinhId);

                if (quyTrinh == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy quy trình phê duyệt", 400);
                }

                quyTrinh.DaXoa = true;
                _quyTrinhPheDuyetRepo.GetDbContext().SaveChanges();
                return new ResponsePostViewModel("Xóa thành công", 200);
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        /// <summary>
        /// Lấy danh sách chia trang quy trình
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseQuyTrinhPheDuyetViewModel GetPagingQuyTrinh(QuyTrinhPheDuyetSearchViewModel request)
        {
            try
            {
                var quyTrinhPheDuyetQuery = _quyTrinhPheDuyetRepo.GetDbQueryTable().Where(q => q.DaXoa == false);
                var loaiHoSoQuery = _quyTrinhPheDuyetRepo.GetDbContext().LoaiHoSo;

                var quyTrinhPheDuyets = from quyTrinh in quyTrinhPheDuyetQuery
                                        join loaiHoSo in loaiHoSoQuery on quyTrinh.LoaiHoSoId equals loaiHoSo.LoaiHoSoId
                                        select new QuyTrinhPheDuyetViewModel
                                        {
                                            QuyTrinhPheDuyetId = quyTrinh.QuyTrinhPheDuyetId,
                                            TenQuyTrinh = quyTrinh.TenQuyTrinh,
                                            LoaiHoSoId = quyTrinh.LoaiHoSoId,
                                            TenLoaiHoSo = loaiHoSo.TenLoaiHoSo,
                                            NgayHieuLuc = quyTrinh.NgayHieuLuc,
                                            MoTa = quyTrinh.MoTa,
                                            TrangThai = quyTrinh.TrangThai,
                                        };
                if (quyTrinhPheDuyets.Any())
                {
                    if (!string.IsNullOrEmpty(request.TuKhoa))
                    {
                        quyTrinhPheDuyets = quyTrinhPheDuyets.Where(x => x.TenQuyTrinh.Contains(request.TuKhoa));
                    }

                    quyTrinhPheDuyets = quyTrinhPheDuyets.OrderByDescending(x => x.NgayHieuLuc);
                    var totalRecord = quyTrinhPheDuyets.Count();
                    if (request.PageIndex > 0)
                    {
                        quyTrinhPheDuyets = quyTrinhPheDuyets.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                    }
                    var loaiHoSoPaging = quyTrinhPheDuyets.ToList();
                    if (totalRecord == 0)
                    {
                        return new ResponseQuyTrinhPheDuyetViewModel(null, 204, totalRecord);
                    }
                    return new ResponseQuyTrinhPheDuyetViewModel(loaiHoSoPaging, 200, totalRecord);
                }
                else
                {
                    return new ResponseQuyTrinhPheDuyetViewModel(null, 204, 0);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Lấy chi tiết quy trình
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public QuyTrinhPheDuyetViewModel GetQuyTrinh(Guid quyTrinhId)
        {
            try
            {
                var quyTrinh = _quyTrinhPheDuyetRepo.GetDbQueryTable()
                    .FirstOrDefault(q => q.QuyTrinhPheDuyetId == quyTrinhId);

                if (quyTrinh == null)
                {
                    return null;
                }

                var result = new QuyTrinhPheDuyetViewModel
                {
                    QuyTrinhPheDuyetId = quyTrinh.QuyTrinhPheDuyetId,
                    TenQuyTrinh = quyTrinh.TenQuyTrinh,
                    LoaiHoSoId = quyTrinh.LoaiHoSoId,
                    NgayHieuLuc = quyTrinh.NgayHieuLuc,
                    MoTa = quyTrinh.MoTa,
                    TrangThai = quyTrinh.TrangThai,
                };

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cập nhật quy trình
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponsePostViewModel UpdateQuyTrinh(UpdateQuyTrinhPheDuyetViewModel input)
        {
            try
            {
                var quyTrinh = _quyTrinhPheDuyetRepo.GetDbQueryTable()
                    .FirstOrDefault(q => q.QuyTrinhPheDuyetId == input.QuyTrinhPheDuyetId);

                if (quyTrinh == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy quy trình phê duyệt", 400);
                }

                quyTrinh.LoaiHoSoId = input.LoaiHoSoId;
                quyTrinh.TenQuyTrinh = input.TenQuyTrinh;
                quyTrinh.NgayHieuLuc = input.NgayHieuLuc;
                quyTrinh.MoTa = input.MoTa;
                quyTrinh.TrangThai = input.TrangThai;

                _quyTrinhPheDuyetRepo.GetDbContext().SaveChanges();
                return new ResponsePostViewModel("Cập nhật quy trình phê duyệt thành công", 200);
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        /// <summary>
        /// Tạo cấu hình bước phê duyệt, gồm tạo bước phê duyệt và danh sách thao tác trong bước phê duyệt
        /// </summary>
        /// <returns></returns>
        public ResponsePostViewModel CreateCauHinhBuocPheDuyet(CreateBuocPheDuyetViewModel input)
        {
            using var transaction = _quyTrinhPheDuyetRepo.GetDbContext().Database.BeginTransaction();
            try
            {
                // if (input.BuocPheDuyetTruocId == null && input.BuocPheDuyetSauId == null)
                // {
                //     return new ResponsePostViewModel("Phải có bước phê duyệt trước hoặc bước phê duyệt sau", 400);
                // }

                var buocPheDuyetInsert = _buocPheDuyetRepo.GetDbQueryTable().Add(new BuocPheDuyet
                {
                    QuyTrinhPheDuyetId = input.QuyTrinhPheDuyetId,
                    BuocPheDuyetTruocId = input.BuocPheDuyetTruocId,
                    BuocPheDuyetSauId = input.BuocPheDuyetSauId,
                    TenBuoc = input.TenBuoc,
                    TrangThaiHoSo = input.TrangThaiHoSo,
                    TrangThaiChungTu = input.TrangThaiChungTu,
                    NguoiThucHien = string.Join(",", input.DsNguoiThucHien.Select(o => o.ToString())),
                    ThoiGianXuLy = input.ThoiGianXuLy,
                    ThuTu = input.ThuTu,
                    DinhDangKy = input.DinhDangKy,
                    ViTriKy = input.ViTriKy
                });

                _buocPheDuyetRepo.GetDbContext().SaveChanges();

                //thêm ds các bước phê duyệt
                foreach (var item in input.DsThaoTacBuocPheDuyet)
                {
                    _thaoTacBuocPheDuyetRepo.GetDbQueryTable().Add(new ThaoTacBuocPheDuyet
                    {
                        BuocPheDuyetId = buocPheDuyetInsert.Entity.BuocPheDuyetId,
                        GiayToId = item.GiayToId,
                        HanhDong = item.HanhDong,
                        LoaiKy = item.LoaiKy,
                        KySo = item.KySo,
                        TrangThaiHoSo = item.TrangThaiHoSo,
                        TrangThaiChungTu = item.TrangThaiChungTu,
                        IsSendMail = item.IsSendMail,
                        DiDenBuocPheDuyetId = item.DiDenBuocPheDuyetId
                    });
                }

                _buocPheDuyetRepo.GetDbContext().SaveChanges();

                transaction.Commit();
                return new ResponsePostViewModel("Cấu hình bước thành công", 200);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        /// <summary>
        /// Lấy chi tiết cấu hình bước phê duyệt
        /// </summary>
        /// <param name="buocPheDuyetId"></param>
        /// <returns></returns>
        public BuocPheDuyetChiTietViewModel GetCauHinhBuocPheDuyet(Guid buocPheDuyetId)
        {
            var buocPheDuyet = _buocPheDuyetRepo.GetDbQueryTable().FirstOrDefault(b => b.BuocPheDuyetId == buocPheDuyetId);
            if (buocPheDuyet == null)
            {
                return null;
            }

            var result = new BuocPheDuyetChiTietViewModel()
            {
                BuocPheDuyetId = buocPheDuyet.BuocPheDuyetId,
                QuyTrinhPheDuyetId = buocPheDuyet.QuyTrinhPheDuyetId,
                BuocPheDuyetTruocId = buocPheDuyet.BuocPheDuyetTruocId,
                BuocPheDuyetSauId = buocPheDuyet.BuocPheDuyetSauId,
                TenBuoc = buocPheDuyet.TenBuoc,
                TrangThaiChungTu = buocPheDuyet.TrangThaiChungTu,
                TrangThaiHoSo = buocPheDuyet.TrangThaiHoSo,
                ThoiGianXuLy = buocPheDuyet.ThoiGianXuLy,
                ThuTu = buocPheDuyet.ThuTu,
                DsNguoiThucHien = buocPheDuyet.NguoiThucHien.Split(",").Select(str => new Guid(str)).ToList(),
                DinhDangKy = buocPheDuyet.DinhDangKy,
                ViTriKy = buocPheDuyet.ViTriKy,
                DsThaoTacBuocPheDuyet = new List<ThaoTacBuocPheDuyetViewModel>()
            };

            var thaoTacBuocPheDuyets = _thaoTacBuocPheDuyetRepo.GetDbQueryTable().Where(t => t.BuocPheDuyetId == buocPheDuyetId).ToList();
            foreach (var thaoTac in thaoTacBuocPheDuyets)
            {
                result.DsThaoTacBuocPheDuyet.Add(new ThaoTacBuocPheDuyetViewModel
                {
                    ThaoTacBuocPheDuyetId = thaoTac.ThaoTacBuocPheDuyetId,
                    BuocPheDuyetId = thaoTac.BuocPheDuyetId,
                    GiayToId = thaoTac.GiayToId,
                    HanhDong = thaoTac.HanhDong,
                    KySo = thaoTac.KySo,
                    LoaiKy = thaoTac.LoaiKy,
                    TrangThaiHoSo = thaoTac.TrangThaiHoSo,
                    TrangThaiChungTu = thaoTac.TrangThaiChungTu,
                    IsSendMail = thaoTac.IsSendMail,
                    DiDenBuocPheDuyetId = thaoTac.DiDenBuocPheDuyetId
                });
            }

            return result;
        }

        /// <summary>
        /// Cập nhật cấu hình bước phê duyệt, gồm cập nhật bước phê duyệt và danh sách thao tác trong bước phê duyệt
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponsePostViewModel UpdateCauHinhBuocPheDuyet(UpdateBuocPheDuyetViewModel input)
        {
            using var transaction = _quyTrinhPheDuyetRepo.GetDbContext().Database.BeginTransaction();
            try
            {
                var buocPheDuyet = _buocPheDuyetRepo.GetDbQueryTable().FirstOrDefault(b => b.BuocPheDuyetId == input.BuocPheDuyetId);
                if (buocPheDuyet == null)
                {
                    return new ResponsePostViewModel("Không tìm thấy bước phê duyệt", 400);
                }

                // if (input.BuocPheDuyetTruocId == null && input.BuocPheDuyetSauId == null)
                // {
                //     return new ResponsePostViewModel("Phải có bước phê duyệt trước hoặc bước phê duyệt sau", 400);
                // }

                buocPheDuyet.QuyTrinhPheDuyetId = input.QuyTrinhPheDuyetId;
                buocPheDuyet.BuocPheDuyetTruocId = input.BuocPheDuyetTruocId;
                buocPheDuyet.BuocPheDuyetSauId = input.BuocPheDuyetSauId;
                buocPheDuyet.TenBuoc = input.TenBuoc;
                buocPheDuyet.TrangThaiHoSo = input.TrangThaiHoSo;
                buocPheDuyet.TrangThaiChungTu = input.TrangThaiChungTu;
                buocPheDuyet.NguoiThucHien = string.Join(",", input.DsNguoiThucHien.Select(o => o.ToString()));
                buocPheDuyet.ThoiGianXuLy = input.ThoiGianXuLy;
                buocPheDuyet.ThuTu = input.ThuTu;
                buocPheDuyet.DinhDangKy = input.DinhDangKy;
                buocPheDuyet.ViTriKy = input.ViTriKy;

                var thaoTacIds = input.DsThaoTacBuocPheDuyet.Select(o => o.ThaoTacBuocPheDuyetId).ToList();

                //xóa thao tác không còn trong danh sách
                var removeList = _thaoTacBuocPheDuyetRepo.GetDbQueryTable()
                    .Where(o => !thaoTacIds.Contains(o.ThaoTacBuocPheDuyetId) && o.BuocPheDuyetId == buocPheDuyet.BuocPheDuyetId)
                    .ToList();

                foreach (var item in removeList)
                {
                    _thaoTacBuocPheDuyetRepo.GetDbQueryTable().Remove(item);
                }

                //ds các thao tác
                foreach (var item in input.DsThaoTacBuocPheDuyet)
                {
                    if (item.ThaoTacBuocPheDuyetId != null) //update
                    {
                        var thaoTacBuocPheDuyet = _thaoTacBuocPheDuyetRepo.GetDbQueryTable()
                            .FirstOrDefault(o => o.ThaoTacBuocPheDuyetId == item.ThaoTacBuocPheDuyetId 
                                && o.BuocPheDuyetId == buocPheDuyet.BuocPheDuyetId);
                        if (thaoTacBuocPheDuyet != null)
                        {
                            thaoTacBuocPheDuyet.GiayToId = item.GiayToId;
                            thaoTacBuocPheDuyet.HanhDong = item.HanhDong;
                            thaoTacBuocPheDuyet.LoaiKy = item.LoaiKy;
                            thaoTacBuocPheDuyet.KySo = item.KySo;
                            thaoTacBuocPheDuyet.IsSendMail = item.IsSendMail;
                            thaoTacBuocPheDuyet.TrangThaiChungTu = item.TrangThaiChungTu;
                            thaoTacBuocPheDuyet.TrangThaiHoSo = item.TrangThaiHoSo;
                            thaoTacBuocPheDuyet.DiDenBuocPheDuyetId = item.DiDenBuocPheDuyetId;
                        }
                    }
                    else //create
                    {
                        _thaoTacBuocPheDuyetRepo.GetDbQueryTable().Add(new ThaoTacBuocPheDuyet
                        {
                            BuocPheDuyetId = buocPheDuyet.BuocPheDuyetId,
                            GiayToId = item.GiayToId,
                            HanhDong = item.HanhDong,
                            LoaiKy = item.LoaiKy,
                            KySo = item.KySo,
                            TrangThaiHoSo = item.TrangThaiHoSo,
                            TrangThaiChungTu = item.TrangThaiChungTu,
                            IsSendMail = item.IsSendMail
                        });
                    }
                }

                _buocPheDuyetRepo.GetDbContext().SaveChanges();

                transaction.Commit();
                return new ResponsePostViewModel("Cấu hình bước thành công", 200);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }
    
        /// <summary>
        /// Lấy danh sách bước phê duyệt theo quy trình Id
        /// </summary>
        /// <param name="quyTrinhPheDuyetId"></param>
        /// <returns></returns>
        public List<BuocPheDuyetViewModel> GetDsBuocPheDuyet(Guid quyTrinhPheDuyetId)
        {
            var listItem = new List<BuocPheDuyetViewModel>();
            var list = _buocPheDuyetRepo.GetDbQueryTable()
                .Where(b => b.QuyTrinhPheDuyetId == quyTrinhPheDuyetId)
                .OrderBy(x => x.ThuTu)
                .ToList()
                .Select(buocPheDuyet => new BuocPheDuyetViewModel
                {
                    BuocPheDuyetId = buocPheDuyet.BuocPheDuyetId,
                    QuyTrinhPheDuyetId = buocPheDuyet.QuyTrinhPheDuyetId,
                    BuocPheDuyetTruocId = buocPheDuyet.BuocPheDuyetTruocId,
                    TenBuocPheDuyetTruoc = _buocPheDuyetRepo.GetDbQueryTable().Where(x => x.BuocPheDuyetId == buocPheDuyet.BuocPheDuyetTruocId).ToList().Select(x => x.TenBuoc),
                    BuocPheDuyetSauId = buocPheDuyet.BuocPheDuyetSauId,
                    TenBuocPheDuyetSau = _buocPheDuyetRepo.GetDbQueryTable().Where(x => x.BuocPheDuyetId == buocPheDuyet.BuocPheDuyetSauId).ToList().Select(x => x.TenBuoc),
                    TenBuoc = buocPheDuyet.TenBuoc,
                    TrangThaiChungTu = buocPheDuyet.TrangThaiChungTu,
                    TrangThaiHoSo = buocPheDuyet.TrangThaiHoSo,
                    ThoiGianXuLy = buocPheDuyet.ThoiGianXuLy,
                    ThuTu = buocPheDuyet.ThuTu,
                    DsNguoiThucHien = buocPheDuyet.NguoiThucHien.Split(",").Select(str => new Guid(str)).ToList(),
                    // TenNguoiThucHien = (from ksct in _context.ApplicationUser
                    //                     where DsNguoiThucHien.Contains(ksct.Id)
                    //                     select(ksct.FirstName + " " + ksct.LastName)).ToList()
                    //select((ksct.FirstName + " " + ksct.LastName).ToString()).ToList()
                    // TenNguoiThucHien = _context.ApplicationUser.
                }).ToList(); 
                foreach (var item in list ) {
                    var listTenNguoiThucHien = new List<string>();
                    var listTaiKhoan = new List<string>();
                    if (item.DsNguoiThucHien != null){
                        foreach (var ds in item.DsNguoiThucHien)
                        {
                            if (ds != null){
                                listTenNguoiThucHien.Add((from ksct in _context.ApplicationUser
                                        where ds.ToString().ToLower() == ksct.Id.ToLower()
                                        select(ksct.FirstName + " " + ksct.LastName )).FirstOrDefault().ToString() + " ");
                                listTaiKhoan.Add((from ksct in _context.ApplicationUser
                                        where ds.ToString().ToLower() == ksct.Id.ToLower()
                                        select(ksct.UserName )).FirstOrDefault().ToString() + " ");
                            }
                        }
                        item.TenNguoiThucHien = listTenNguoiThucHien;
                        item.TaiKhoan = listTaiKhoan;
                    }
                    listItem.Add(item);
                }
            return listItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponsePostViewModel CreateQuaTrinhPheDuyet(CreateQuaTrinhPheDuyet input)
        {
            try
            {
                _quaTrinhPheDuyetRepo.GetDbQueryTable().Add(new QuaTrinhPheDuyet
                {
                    BuocPheDuyetId = input.BuocPheDuyetId,
                    HoSoId = input.HoSoId,
                    ThoiGianTao = input.ThoiGianTao,
                    TrangThaiXuLy = input.TrangThaiXuLy,
                    ThoiGianXuLy = input.ThoiGianXuLy,
                    NguoiXuLyId = input.NguoiXuLyId,
                    DaXoa = false
                });
                _quaTrinhPheDuyetRepo.GetDbContext().SaveChanges();
                return new ResponsePostViewModel("Lưu quá trình phê duyệt thành công", 200);
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public List<QuyTrinhBuocHanhDongViewModel> GetQuyTrinhByLoaiHoSoId(Guid loaiHoSoId)
        {
            try
            {
                var quyTrinh = _quyTrinhPheDuyetRepo.GetDbQueryTable()
                .Where(x => x.LoaiHoSoId == loaiHoSoId).ToList()
                .Select(qt => new QuyTrinhBuocHanhDongViewModel
                {
                    QuyTrinhPheDuyetId = qt.QuyTrinhPheDuyetId,
                    TenQuyTrinh = qt.TenQuyTrinh,
                    LoaiHoSoId = qt.LoaiHoSoId,
                    NgayHieuLuc = qt.NgayHieuLuc,
                    MoTa = qt.MoTa,
                    TrangThai = qt.TrangThai,
                    ListBuocThuHien = _buocPheDuyetRepo.GetDbQueryTable()
                    .Where(x => x.QuyTrinhPheDuyetId == qt.QuyTrinhPheDuyetId).ToList()
                    .OrderBy(x => x.ThuTu)
                    .Select(buoc => new BuocViewModel
                    {
                        BuocPheDuyetId = buoc.BuocPheDuyetId,
                        QuyTrinhPheDuyetId = buoc.QuyTrinhPheDuyetId,
                        BuocPheDuyetTruocId = buoc.BuocPheDuyetTruocId,
                        BuocPheDuyetSauId = buoc.BuocPheDuyetSauId,
                        TrangThaiHoSo = buoc.TrangThaiHoSo,
                        TrangThaiChungTu = buoc.TrangThaiChungTu,
                        TenBuoc = buoc.TenBuoc,
                        ThuTu = buoc.ThuTu,
                        DsNguoiThucHien = buoc.NguoiThucHien.Split(",").Select(str => new Guid(str)).ToList(),
                        ThoiGianXuLy = buoc.ThoiGianXuLy,
                        ListThaoTac = _thaoTacBuocPheDuyetRepo.GetDbQueryTable()
                        .Where(x => x.BuocPheDuyetId == buoc.BuocPheDuyetId).ToList()
                        .Select(tt => new ThaoTacViewModel {
                            ThaoTacBuocPheDuyetId = tt.ThaoTacBuocPheDuyetId,
                            BuocPheDuyetId = tt.BuocPheDuyetId,
                            HanhDong = tt.HanhDong,
                            KySo = tt.KySo,
                            LoaiKy = tt.LoaiKy,
                            GiayToId = tt.GiayToId,
                            IsSendMail = tt.IsSendMail
                        }).ToList()
                    }).ToList()
                }).ToList();
                
                return quyTrinh;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
