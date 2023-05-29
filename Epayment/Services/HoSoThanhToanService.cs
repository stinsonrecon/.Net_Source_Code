using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.Repositories;
using Epayment.ViewModels;
using System.Linq;
using System.IO;
using Epayment.Services;
using BCXN.Statics;
using BCXN.Data;

namespace Epayment.Services
{
    public class HoSoThanhToanService : IHoSoThanhToanService
    {
        private readonly IHoSoThanhToanRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILichSuHoSoTTRepository _ILichSuHoSoTT;
        private readonly IKySoTaiLieuRepository _IKySoTaiLieu;
        private readonly IKySoTaiLieuService _IKySoTaiLieuService;
        private readonly IChiTietGiayToHSTTRepository _IChiTietGiayToRepo;
        private readonly QuyTrinhPheDuyetService _quyTrinhPheDuyetService;
        private readonly ThaoTacBuocPheDuyetRepository _thaoTacBuocPheDuyetRepo;
        private readonly IUploadToFtpService _IUploadToFtpService;
        private readonly ApplicationDbContext _context;
        //private readonly IGeneratePDFService _GeneratePDFService;
        public HoSoThanhToanService(IHoSoThanhToanRepository repo, IMapper mapper, ILichSuHoSoTTRepository LichSuHoSoTT, IKySoTaiLieuRepository KySoTaiLieu, IGeneratePDFService generatePDFService, 
        IKySoTaiLieuService IKySoTaiLieuService, IChiTietGiayToHSTTRepository IChiTietGiayToRepo, IUploadToFtpService IUploadToFtpService, 
        QuyTrinhPheDuyetService QuyTrinhPheDuyetService,ThaoTacBuocPheDuyetRepository thaoTacBuocPheDuyetRepo, ApplicationDbContext context)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
            _ILichSuHoSoTT = LichSuHoSoTT;
            _IKySoTaiLieu = KySoTaiLieu;
            // _GeneratePDFService = generatePDFService;
            _IKySoTaiLieuService = IKySoTaiLieuService;
            _IChiTietGiayToRepo = IChiTietGiayToRepo;
            _quyTrinhPheDuyetService = QuyTrinhPheDuyetService;
            _thaoTacBuocPheDuyetRepo = thaoTacBuocPheDuyetRepo;
            _IUploadToFtpService = IUploadToFtpService;
        }

        public async Task<ResponsePostHSTTViewModel> CreateHoSoTT(ParmHoSoThanhToanViewModel request)
        {

            var ret = _repo.CreateHoSoTT(request);
            if (ret.idHoSoTT != null)
            {
                ParmHoSoThanhToanViewModel tam = new ParmHoSoThanhToanViewModel();
                tam = request;
                tam.HoSoId = new Guid(ret.idHoSoTT);
                ParmChiTietGiayToHSTTViewModel parmCTGT = new ParmChiTietGiayToHSTTViewModel();
                parmCTGT.HoSoThanhToanId = ret.idHoSoTT;
                parmCTGT.NguoiCapNhatId = request.IdLogin;
                parmCTGT.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId;
                _IChiTietGiayToRepo.CreateToTrinhChiTietGiay(parmCTGT);
                await _ILichSuHoSoTT.CreateLichSuHoSoTT(tam);
                CreateQuaTrinhPheDuyet qt = new CreateQuaTrinhPheDuyet {
                    BuocPheDuyetId = request.QuaTrinhPheDuyetId,
                    HoSoId = new Guid(ret.idHoSoTT),
                    ThoiGianTao = DateTime.Now,
                    TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                    ThoiGianXuLy = DateTime.Now,
                    NguoiXuLyId = new Guid(request.IdLogin),
                };
                this._quyTrinhPheDuyetService.CreateQuaTrinhPheDuyet(qt);
            }
            return ret;
        }

        public ResponsePostViewModel DeleteHoSoTT(Guid id, string userID)
        {
            var ret = _repo.DeleteHoSoTT(id, userID);
            return ret;
        }

        public List<HoSoThanhToanViewModel> GetHoSoTT()
        {
            var ret = _repo.GetHoSoTT();
            return ret;
        }

        public ResponseHoSoViewModel GetHoSoTTById(string id)
        {
            var ret = _repo.GetHoSoTTById(id);
            return ret;
        }

        public async Task<ResponsePostViewModel> UpdateHoSoTT(ParmHoSoThanhToanViewModel request)
        {
            var ret = _repo.UpdateHoSoTT(request);
            if (ret.StatusCode == 200)
            {
                await _ILichSuHoSoTT.CreateLichSuHoSoTT(request);
            }
            return ret;
        }

        public ResponseHoSoViewModel GetHoSoPaging(HoSoSearchViewModel request)
        {
            var hoSo = _repo.GetHoSoPaging(request);
            return hoSo;
        }

        public ResponsePostViewModel ApproveHoSo(ApproveHoSoTT request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (request.TrangThaiPheDuyet == TrangThaiPheDuyetHoSo.TuChoi || request.TrangThaiPheDuyet == TrangThaiPheDuyetHoSo.TuChoiPhanCongNguoiPheDuyet)
                {
                    var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                        .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);
                    if (thaoTacBuocPheDuyet == null)
                    {
                        throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {request.ThaoTacBuocPheDuyetId}");
                    }

                    var buocPheDuyet = _context.BuocPheDuyet.FirstOrDefault(b => b.BuocPheDuyetId == thaoTacBuocPheDuyet.BuocPheDuyetId);
                    if (buocPheDuyet == null)
                    {
                        throw new Exception($"Không tìm thấy bước phê duyệt Id = {thaoTacBuocPheDuyet.BuocPheDuyetId}");
                    }

                    //quay về bước 1
                    var buocPheDuyet1 = _context.BuocPheDuyet.FirstOrDefault(b => b.QuyTrinhPheDuyetId == buocPheDuyet.QuyTrinhPheDuyetId && b.ThuTu == 1 && !b.DaXoa);
                    if (buocPheDuyet1 == null)
                    {
                        throw new Exception($"Không tìm thấy bước phê duyệt đầu tiên của quy trình phê duyệt Id = {buocPheDuyet.QuyTrinhPheDuyetId}");
                    }

                    //danh sách các bước
                    var buocPheDuyetIds = _context.BuocPheDuyet
                        .Where(b => b.QuyTrinhPheDuyetId == buocPheDuyet.QuyTrinhPheDuyetId && !b.DaXoa)
                        .Select(b => b.BuocPheDuyetId)
                        .ToList();

                    //Xóa quá trình phê duyệt
                    var quaTrinhPheDuyetRemoves = _context.QuaTrinhPheDuyet.Where(q => buocPheDuyetIds.Contains(q.BuocPheDuyetId)).ToList();
                    foreach (var quaTrinhPheDuyetRemove in quaTrinhPheDuyetRemoves)
                    {
                        quaTrinhPheDuyetRemove.DaXoa = true;
                    }

                    //quay về bước hồ sơ thanh toán 1
                    var hoSoThanhToan = _context.HoSoThanhToan.FirstOrDefault(h => h.HoSoId == new Guid(request.HoSoId));
                    if (hoSoThanhToan == null)
                    {
                        throw new Exception($"Không tìm thấy hồ sơ thanh toán Id = {request.HoSoId}");
                    }
                    hoSoThanhToan.QuaTrinhPheDuyetId = buocPheDuyet1.BuocPheDuyetId;

                    //xóa ký số tài liệu
                    var kySoTaiLieuRemove = from tt in _context.KySoTaiLieu
                                            where tt.HoSoThanhToan.HoSoId == new Guid(request.HoSoId)
                                            select tt;
                    foreach (var item in kySoTaiLieuRemove.ToList())
                    {
                        item.DaXoa = true;
                    }

                    _context.SaveChanges();
                }

                var ret = _repo.ApproveHoSo(request);

                transaction.Commit();
                return ret;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResponsePostViewModel(e.ToString(), 500);
            }
        }
        public ResponsePheDuyetHoSoTTViewModel GetPheDuyetHoSoTTById(string id)
        {
            var ret = _repo.GetPheDuyetHoSoTTById(id);
            return ret;
        }

        public async Task<ResponsePostViewModel> UpdateFileHoSoTT(CreateFileHoSoTT request)
        {
            try
            {
                var fileDinhKem = _repo.GetHoSoTTById(request.HoSoId);
                if (fileDinhKem.Data == null ){
                    return new ResponsePostViewModel(message: "Lỗi không tìm thấy hồ sơ", statusCode: 400);
                }
                if (fileDinhKem.Data[0].trangThaiHoSo != BCXN.Statics.TrangThaiHoSo.KhoiTao)
                {
                    return new ResponsePostViewModel(message: "Lỗi hồ sơ không phải trạng thái khỏi tạo", statusCode: 400);
                }
                if (request.FileTaiLieu != null)
                {
                    string fileExtension = System.IO.Path.GetExtension(request.FileTaiLieu.FileName);
                    //if ( fileExtension == ".doc" || fileExtension == ".docx" ){
                        var pathFile = await _IUploadToFtpService.UploadFileToFtpServer(request.FileTaiLieu, fileDinhKem.Data[0].maHoSo);
                        request.UrlFile = pathFile.ToString();
                    // }else {
                    //     return new ResponsePostViewModel(message: "file upload không hợp lệ", statusCode: 400);
                    // }
                }
                else
                {
                    return new ResponsePostViewModel(message: "file upload không hợp lệ", statusCode: 400);
                }
                request.CapKy = 1;
                // request.TrangThai = BCXN.Statics.TrangThaiHoSo.ChuaTiepNhan;
                //tìm thao tác bước phê duyệt
                var thaoTacBuocPheDuyet = _thaoTacBuocPheDuyetRepo.GetDbQueryTable()
                    .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == request.ThaoTacBuocPheDuyetId);

                if (thaoTacBuocPheDuyet == null)
                {
                    throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {request.ThaoTacBuocPheDuyetId}");
                }
                //Convert.ToInt32(fileDinhKem.Data[0].BuocPheDuyet.TrangThaiHoSo);
                request.TrangThai = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);
                var ret = _repo.UpdateFileHoSoTT(request);
                KySoTaiLieuParams kstl = new KySoTaiLieuParams();
                kstl.HoSoThanhToanId = new Guid(request.HoSoId);

                var itemKySoTT = _IKySoTaiLieu.GetKySoTaiLieuByHoSoId(request.HoSoId);
                if (Convert.ToInt32(itemKySoTT.Data["TotalRecord"]) <= 0)
                {
                    var kySo = _IKySoTaiLieu.CreateKySoTaiLieu(kstl);
                    if (kySo.Success == false)
                    {
                        return new ResponsePostViewModel(message: kySo.Message, statusCode: 400);
                    }
                }
                // tự ký cấp 1 tờ trình
                kstl.CapKy = 1;
                kstl.TaiLieuGoc = request.UrlFile;
                kstl.NoiDungKy = "Đồng ý";
                kstl.NguoiKyId = fileDinhKem.Data[0].NguoiTaoId;
                kstl.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId;
                kstl.BuocPheDuyetId = request.BuocPheDuyetId;
                kstl.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId;
                var kc1 = _IKySoTaiLieuService.KyToTrinh(kstl);
                //var kc1 = _IKySoTaiLieuService.KyEVNCA(kstl);
                if (kc1.Success == false)
                {
                    request.CapKy = 0;
                    request.TrangThai = BCXN.Statics.TrangThaiHoSo.KhoiTao;
                    //request.UrlFile = null;
                    var upDateCapKy = _repo.UpdateFileHoSoTT(request);
                    // var CancelkySo = _IKySoTaiLieu.CancelToTrinh(kstl, fileDinhKem.Data[0].NguoiTaoId);
                    // if (CancelkySo.Success == false)
                    // {
                    //     return new ResponsePostViewModel(message: CancelkySo.Message, statusCode: 400);
                    // }
                    return new ResponsePostViewModel(message: kc1.Data.ToString(), statusCode: 400);
                    //return new ResponsePostViewModel(message: "đã lưu hồ sơ chưa, lỗi khi ký", statusCode: 201);
                }
                return ret;
            }
            catch (System.Exception ex)
            {
                return new ResponsePostViewModel(message: ex.Message, statusCode: 400);
            }

        }

        public ResponsePaging GetAllHoSoThamChieu(HoSoThamChieuSearchViewModel request)
        {
            var ret = _repo.GetAllHoSoThamChieu(request);
            return ret;
        }

        public ResponsePostViewModel PhanCongXuLy(ModelRequest.PhanCongXuLy request)
        {
            var ret = _repo.PhanCongXuLy(request);
            return ret;
        }
    }
}