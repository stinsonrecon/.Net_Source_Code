using AutoMapper;
using BCXN.Data;
using BCXN.Repositories;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Models;
using Epayment.Repositories;
using Epayment.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BCXN.Statics;

namespace Epayment.Services
{
    public interface IKySoTaiLieuService
    {
        Response KyToTrinh(KySoTaiLieuParams kstl);
        ResponseGetKySoTaiLieu GetKySoTaiLieuPaging(SearchKySoTaiLieu request);
        ResponseGetKySoTaiLieu GetKySoTaiLieuById(Guid KySoTaiLieuId);
        //Response UpdateSoTaiLieu(KySoTaiLieuParams kstl);
        Response CancelToTrinh(KySoTaiLieuParams request, string nguoihuy);
        Response KySoPhieuThamTraHoSo(KySoPhieuThamTraPram ksptt);
        Response GetPhieuThamTraByIdHSTT (Guid HoSoTTId, Guid GiayToId);
        Response KySoTiepNhanHoSo (ApproveHoSoTT itemApproveHSTT);
    }

    public class KySoTaiLieuService : IKySoTaiLieuService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IKySoTaiLieuRepository _repo;
        private readonly IChiTietGiayToHSTTRepository _ctgtrepo;
        private readonly IHoSoThanhToanRepository _HoSoThanhToanRepository;
        private readonly IUtilsService _utilsservice;
        private readonly QuyTrinhPheDuyetService _quyTrinhPheDuyetService;
        private readonly ThaoTacBuocPheDuyetRepository _thaoTacBuocPheDuyetRepo;
        private readonly IUploadToFtpService _ftpService;

        public KySoTaiLieuService(IKySoTaiLieuRepository repo, IChiTietGiayToHSTTRepository ctgtrepo, IMapper mapper, 
            IConfiguration configuration, ApplicationDbContext context, IHoSoThanhToanRepository HoSoThanhToanRepository,
            IUtilsService utilsservice, QuyTrinhPheDuyetService quyTrinhPheDuyetService, 
            ThaoTacBuocPheDuyetRepository thaoTacBuocPheDuyetRepo, IUploadToFtpService ftpService)
        {
            _repo = repo;
            _ctgtrepo = ctgtrepo;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
            _HoSoThanhToanRepository = HoSoThanhToanRepository;
            _utilsservice = utilsservice;
            _quyTrinhPheDuyetService = quyTrinhPheDuyetService;
            _thaoTacBuocPheDuyetRepo = thaoTacBuocPheDuyetRepo;
            _ftpService = ftpService;
        }

        public ResponseGetKySoTaiLieu GetKySoTaiLieuPaging(SearchKySoTaiLieu request)
        {
            var ret = _repo.GetKySoTaiLieuPaging(request);
            return ret;
        }

        public Response KyToTrinh(KySoTaiLieuParams kstl)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var nguoikyItem = _context.ApplicationUser.FirstOrDefault(x => (x.Id == kstl.NguoiKyId));
                if (nguoikyItem == null || nguoikyItem.PhoneNumber == null || nguoikyItem.CAProvider == null)
                    return new Response(message: "Lỗi", data: "Chưa cấu hình đủ thông tin để ký tờ trình", errorcode: "SV001", success: false);

                var hosottItem = (from hs in _context.HoSoThanhToan
                                  join dv in _context.DonVi on hs.BoPhanCauId equals dv
                                  join lhs in _context.LoaiHoSo on hs.LoaiHoSo equals lhs
                                  where hs.HoSoId == kstl.HoSoThanhToanId
                                  select new
                                  {
                                      HoSoThanhToan = hs,
                                      BoPhanYeuCau = dv,
                                      LoaiHoSo = lhs
                                  }).FirstOrDefault();

                string signURL;
                var buocPheDuyet = _context.BuocPheDuyet.FirstOrDefault(x => x.BuocPheDuyetId == kstl.BuocPheDuyetId);

                Dictionary<string, object> payload = new Dictionary<string, object>()
                {
                    { "TypeSign", "BigImage" },
                    { "Provider", nguoikyItem.CAProvider },
                    { "DINH_DANG_FILE", "doc"},
                    { "CommentSign", kstl.NoiDungKy }
                };

                if (nguoikyItem.CAProvider == "EVN_CA")
                {
                    payload.Add("DinhDanhKy", nguoikyItem.DinhDanhKy);
                    payload.Add("KEY_SIGN", "EVN_CA");
                    signURL = _configuration["CASettings:BaseAddress"] + "/SignServerV2/api/KySo/signPDFvs2/";
                }
                else
                {
                    payload.Add("DinhDanhKy", nguoikyItem.PhoneNumber);
                    signURL = _configuration["CASettings:BaseAddress"] + "/SignServerV2/api/KySo/signPDFvs2/";
                }

                string tlk_base64str = "";
                if (kstl.CapKy == 1)
                {
                    //tlk_bytes = File.ReadAllBytes(hosottItem.HoSoThanhToan.TaiLieuGoc);
                    var getFile = _ftpService.DownloadFileFtpServerConvertBase64(hosottItem.HoSoThanhToan.TaiLieuGoc);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký cho cấp 1", data: "SV002", errorcode: "", success: false);
                    payload.Add("FileType", Path.GetExtension(hosottItem.HoSoThanhToan.TaiLieuGoc).Replace(".", "").ToLower());
                    // payload.Add("FullName", "Người đề nghị");
                    payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "up");
                }
                else if (kstl.CapKy == 2)
                {
                    var getFile = _ftpService.DownloadFileFtpServerConvertBase64(hosottItem.HoSoThanhToan.TaiLieuKy);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký cho cấp 2", data: "SV003", errorcode: "", success: false);
                    payload.Add("FileType", Path.GetExtension(hosottItem.HoSoThanhToan.TaiLieuKy).Replace(".", "").ToLower());
                    // payload.Add("FullName", "Chánh VP/Trưởng Ban");
                    // payload.Add("FullName", "Kế toán trưởng");
                    payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "up");
                }
                else if (kstl.CapKy == 3)
                {
                    var getFile = _ftpService.DownloadFileFtpServerConvertBase64(hosottItem.HoSoThanhToan.TaiLieuKy);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();
                    else return new Response(message: "Lỗi: không lấy được file cần ký cho cấp 3", data: "SV004", errorcode: "", success: false);
                    payload.Add("FileType", Path.GetExtension(hosottItem.HoSoThanhToan.TaiLieuKy).Replace(".", "").ToLower());
                    // payload.Add("FullName", "Lãnh đạo EVN");
                    // payload.Add("FullName", "Tổng giám đốc");
                    payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "up");
                }
                else return new Response(message: "Lỗi: Sai cấp ký", data: "SV005", errorcode: "", success: false);
                if (!string.IsNullOrEmpty(tlk_base64str))
                {
                    payload.Add("FileDataSign", Convert.FromBase64String(tlk_base64str));
                }
                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    signURL,
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    resultResponse.Wait();
                    var CAResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                    // dynamic bankResponseJson = JToken.Parse(bankResponseString);
                    KetQuaKySoModel output = new KetQuaKySoModel();
                    output = JsonConvert.DeserializeObject<KetQuaKySoModel>(CAResponseString);
                    //string status = (string)bankResponseJson.State;

                    //thêm thông tin vào bảng PheDuyetHoSoTT
                    ApproveHoSoTT itemApproveHSTT = new ApproveHoSoTT();
                    itemApproveHSTT.HoSoId = hosottItem.HoSoThanhToan.HoSoId.ToString();
                    itemApproveHSTT.BuocThucHien = "cấp Ký" + kstl.CapKy;
                    itemApproveHSTT.NguoiThucHienId = nguoikyItem.Id;

                    var thaoTacBuocPheDuyet = _thaoTacBuocPheDuyetRepo.GetDbQueryTable()
                        .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == kstl.ThaoTacBuocPheDuyetId);
                    if (thaoTacBuocPheDuyet == null)
                    {
                        throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {kstl.ThaoTacBuocPheDuyetId}");
                    }
                    itemApproveHSTT.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);

                    if (output.state == "true")
                    {
                        string tai_lieu_ky_moi_path = _ftpService.UploadByteFileFTP(Convert.FromBase64String(output.data), hosottItem.HoSoThanhToan.MaHoSo, Guid.NewGuid().ToString().Replace("-", ""), "pdf");
                        if (tai_lieu_ky_moi_path == "") return new Response(message: "Lỗi: Không upload được file kí lên ftp server", data: "SV006", errorcode: "", success: false);
                        kstl.TaiLieuKy = tai_lieu_ky_moi_path;

                        // update "file dinh kem" la tai lieu ky vao chi tiet ho so
                        _ctgtrepo.UpdateTaiLieuToTrinh(hosottItem.HoSoThanhToan, tai_lieu_ky_moi_path, kstl.ThaoTacBuocPheDuyetId, kstl.NguoiKyId);   
                        itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.PheDuyetToTrinh;
                        itemApproveHSTT.NoiDung = kstl.NoiDungKy;
                        itemApproveHSTT.ThaoTacBuocPheDuyetId = kstl.ThaoTacBuocPheDuyetId;
                        _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);
                        var listBuocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hosottItem.HoSoThanhToan.QuyTrinhPheDuyetId).ToList();

                        var resp = _repo.KyToTrinh(kstl);
                        if (resp.Success)
                        {
                            // gửi mail thông báo đến những user thực hiện bước sau
                            _utilsservice.SendMailKy(kstl.ThaoTacBuocPheDuyetId, kstl.HoSoThanhToanId);
                        }
                        else
                        {
                            throw new Exception(resp.Message);
                        }
                        var buocPD = _context.BuocPheDuyet.Select(x => x).Where(i => i.QuyTrinhPheDuyetId == hosottItem.HoSoThanhToan.QuyTrinhPheDuyetId && i.BuocPheDuyetId == hosottItem.HoSoThanhToan.QuaTrinhPheDuyetId).ToList();
                        var thuTu = 0;
                        BuocPheDuyet buocHienTai = new BuocPheDuyet();
                        if (listBuocPD.Count != 0 && buocPD.Count != 0) 
                        {
                            thuTu = buocPD[0].ThuTu;
                            foreach (var item in listBuocPD)
                            {
                                if (item.ThuTu == buocPD[0].ThuTu + 1)
                                {
                                    buocHienTai = item;
                                }
                            }
                        }
                        CreateQuaTrinhPheDuyet qt = new CreateQuaTrinhPheDuyet {
                            BuocPheDuyetId = buocHienTai.BuocPheDuyetId,
                            HoSoId = hosottItem.HoSoThanhToan.HoSoId,
                            ThoiGianTao = DateTime.Now,
                            TrangThaiXuLy = 1, // trạng thái 1 -> đã xử lý xong
                            ThoiGianXuLy = DateTime.Now,
                            NguoiXuLyId = new Guid(nguoikyItem.Id),
                        };
                        _quyTrinhPheDuyetService.CreateQuaTrinhPheDuyet(qt);
                        var hoSo = _context.HoSoThanhToan.FirstOrDefault(x => x.HoSoId == hosottItem.HoSoThanhToan.HoSoId);
                        hoSo.QuaTrinhPheDuyetId = buocHienTai.BuocPheDuyetId;
                        _context.SaveChanges();

                        transaction.Commit();
                        return resp;
                    }
                    else
                    {
                        itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.TuChoiPheDuyetToTrinh;
                        itemApproveHSTT.NoiDung = "Từ chối";
                        _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);
                        _utilsservice.SendMailHuy(kstl.ThaoTacBuocPheDuyetId, kstl.HoSoThanhToanId);
                        transaction.Commit();
                        return new Response(message: "Chưa kí", data: "Bạn đã không đồng ý ký tờ trình", errorcode: "004", success: false);
                    }
                }
                else
                {
                    transaction.Rollback();
                    return new Response(message: "Lỗi", data: "Không thể gọi dịch vụ ký", errorcode: "003", success: false);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public ResponseGetKySoTaiLieu GetKySoTaiLieuById(Guid KySoTaiLieuId)
        {
            var ret = _repo.GetKySoTaiLieuById(KySoTaiLieuId);
            return ret;
        }

        public Response CancelToTrinh(KySoTaiLieuParams request, string nguoiHuy)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var ret = _repo.CancelToTrinh(request, nguoiHuy);

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

                var hoSoThanhToan = _context.HoSoThanhToan.FirstOrDefault(h => h.HoSoId == request.HoSoThanhToanId);
                hoSoThanhToan.QuaTrinhPheDuyetId = buocPheDuyet1.BuocPheDuyetId;

                // gửi mail thông báo cho người thực hiện bước 1 biết có hồ sơ được yêu cầu thay đổi
                _utilsservice.SendMailHuy(request.ThaoTacBuocPheDuyetId, request.HoSoThanhToanId);

                _context.SaveChanges();

                ApproveHoSoTT itemApproveHSTT = new ApproveHoSoTT();
                itemApproveHSTT.HoSoId = request.HoSoThanhToanId.ToString();
                itemApproveHSTT.BuocThucHien = "cấp Ký" + request.CapKy;
                //itemApproveHSTT.TrangThaiHoSo = BCXN.Statics.TrangThaiHoSo.YeuCauThaiDoi; // chuyển trạng thái hồ sơ về trạng thái yêu cầu thay đổi
                itemApproveHSTT.TrangThaiHoSo = Convert.ToInt32(thaoTacBuocPheDuyet.TrangThaiHoSo);

                itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.TuChoiPheDuyetToTrinh; // chuyển trạng thái phê duyệt về trạng thái từ chối tờ trình
                itemApproveHSTT.NguoiThucHienId = nguoiHuy;
                itemApproveHSTT.NoiDung = request.NoiDungKy;
                itemApproveHSTT.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId;
                _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);

                transaction.Commit();
                return ret;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new Response(message: "Lỗi", data: e.Message, errorcode: "001", success: false);
            }
        }

        public Response KySoPhieuThamTraHoSo (KySoPhieuThamTraPram ksptt)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var nguoikyItem = _context.ApplicationUser.FirstOrDefault(x => (x.Id == ksptt.NguoiKyId));
                if (nguoikyItem == null || nguoikyItem.PhoneNumber == null || nguoikyItem.CAProvider == null)
                    return new Response(message: "Lỗi", data: "Chưa cấu hình đủ thông tin để ký phiếu thẩm tra", errorcode: "005", success: false);

                var hosottItem = (from hs in _context.HoSoThanhToan
                                  where hs.HoSoId == ksptt.HoSoThanhToanId
                                  select new
                                  {
                                      HoSoThanhToan = hs,
                                  }).FirstOrDefault();
                //if (hosottItem.HoSoThanhToan.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.ThamTraHoSo)
                //    return new Response(message: "Hồ sơ không thuộc hồ sơ thẩm tra", data: null, errorcode: "007", success: false);
                var PhieuThamTraItem = (from ctgt in _context.ChiTietGiayToHSTT 
                                        where ctgt.HoSoThanhToan.HoSoId == ksptt.HoSoThanhToanId && ctgt.GiayTo.GiayToId == ksptt.GiayToId
                                        select new {
                                            HoSoId = ctgt.HoSoThanhToan.HoSoId,
                                            GiayToId = ctgt.GiayTo.GiayToId,
                                            TaiLieu = ctgt.FileDinhKem
                                        }).FirstOrDefault();
                string signURL;
                if (ksptt.CapKy == 0) //cancel
                {
                    var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                        .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == ksptt.ThaoTacBuocPheDuyetId);
                    if (thaoTacBuocPheDuyet == null)
                    {
                        throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {ksptt.ThaoTacBuocPheDuyetId}");
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
                    var hoSoThanhToan = _context.HoSoThanhToan.FirstOrDefault(h => h.HoSoId == ksptt.HoSoThanhToanId);
                    if (hoSoThanhToan == null)
                    {
                        throw new Exception($"Không tìm thấy hồ sơ thanh toán Id = {ksptt.HoSoThanhToanId}");
                    }
                    hoSoThanhToan.QuaTrinhPheDuyetId = buocPheDuyet1.BuocPheDuyetId;

                    //xóa ký số tài liệu
                    var kySoTaiLieuRemove = from tt in _context.KySoTaiLieu
                                            where tt.HoSoThanhToan.HoSoId == ksptt.HoSoThanhToanId
                                            select tt;
                    foreach (var item in kySoTaiLieuRemove.ToList())
                    {
                        item.DaXoa = true;
                    }

                    _context.SaveChanges();

                    ApproveHoSoTT itemApproveHSTT = new ApproveHoSoTT();
                    itemApproveHSTT.HoSoId = hosottItem.HoSoThanhToan.HoSoId.ToString();
                    itemApproveHSTT.NguoiThucHienId = nguoikyItem.Id;
                    itemApproveHSTT.TrangThaiHoSo = hosottItem.HoSoThanhToan.TrangThaiHoSo;
                    itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.TuChoiThamTraHoSo;
                    itemApproveHSTT.NoiDung = ksptt.NoiDungKy;
                    itemApproveHSTT.ThaoTacBuocPheDuyetId = ksptt.ThaoTacBuocPheDuyetId; //thao tác bước phê duyệt

                    _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);
                    //_HoSoThanhToanRepository.UpdateTrangThaiHsttById(hosottItem.HoSoThanhToan.HoSoId, BCXN.Statics.TrangThaiHoSo.YeuCauThaiDoi);
                    transaction.Commit();
                    return new Response(message: "Từ chối ký phiếu thẩm tra thành công", data: "Bạn đã từ chối ký phiếu thẩm tra" , errorcode: "007", success: true);
                }
                Dictionary<string, object> payload = new Dictionary<string, object>()
                {
                    { "TypeSign", "BigImage" },
                    { "Provider", nguoikyItem.CAProvider },
                    { "DINH_DANG_FILE", "doc"},
                    { "CommentSign", ksptt.NoiDungKy }
                };

                if (nguoikyItem.CAProvider == "EVN_CA")
                {
                    payload.Add("DinhDanhKy", nguoikyItem.DinhDanhKy);
                    payload.Add("KEY_SIGN", "EVN_CA");
                    signURL = _configuration["CASettings:BaseAddress"] + "/SignServerV2/api/KySo/signPDFvs2/";
                }
                else
                {
                    payload.Add("DinhDanhKy", nguoikyItem.PhoneNumber);
                    signURL = _configuration["CASettings:BaseAddress"] + "/SignServerV2/api/KySo/signPDFvs2/";
                }

                Byte[] tlk_bytes;
                string tlk_base64str = "";
                if (ksptt.CapKy == TrangThaiChiTietGiayToThamTra.TrinhKyCap1)
                {
                    // tlk_bytes = File.ReadAllBytes(PhieuThamTraItem.TaiLieu);
                    var getFile = _ftpService.DownloadFileFtpServerConvertBase64(PhieuThamTraItem.TaiLieu);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();
                    payload.Add("FileType", Path.GetExtension(PhieuThamTraItem.TaiLieu).Replace(".", "").ToLower());
                    payload.Add("FullName", "Người thẩm tra");
                    payload.Add("SignPosition", "down");
                }
                else if (ksptt.CapKy == TrangThaiChiTietGiayToThamTra.TrinhKyCap2)
                {
                    // tlk_bytes = File.ReadAllBytes(PhieuThamTraItem.TaiLieu);
                    var getFile = _ftpService.DownloadFileFtpServerConvertBase64(PhieuThamTraItem.TaiLieu);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();
                    payload.Add("FileType", Path.GetExtension(PhieuThamTraItem.TaiLieu).Replace(".", "").ToLower());
                    payload.Add("FullName", "Trưởng phòng KHKT");
                    payload.Add("SignPosition", "down");
                }
                else
                {
                    transaction.Rollback();
                    return new Response(message: "Lỗi: Sai cấp ký", data: "002", errorcode: "", success: false);
                }
                payload.Add("FileDataSign", tlk_base64str);

                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    signURL,
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    resultResponse.Wait();
                    var CAResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                    // dynamic bankResponseJson = JToken.Parse(bankResponseString);
                    KetQuaKySoModel output = new KetQuaKySoModel();
                    output = JsonConvert.DeserializeObject<KetQuaKySoModel>(CAResponseString);
                    //string status = (string)bankResponseJson.State;

                    //thêm thông tin vào bảng PheDuyetHoSoTT
                    ApproveHoSoTT itemApproveHSTT = new ApproveHoSoTT();
                    itemApproveHSTT.HoSoId = hosottItem.HoSoThanhToan.HoSoId.ToString();
                    itemApproveHSTT.BuocThucHien = "Thẩm tra lần" + ksptt.CapKy;
                    itemApproveHSTT.NguoiThucHienId = nguoikyItem.Id;
                    itemApproveHSTT.TrangThaiHoSo = hosottItem.HoSoThanhToan.TrangThaiHoSo;
                    itemApproveHSTT.ThaoTacBuocPheDuyetId = ksptt.ThaoTacBuocPheDuyetId; //thao tác bước phê duyệt

                    if (output.state == "true")
                    {
                        // string tai_lieu_ky_moi_base64 = output.data;
                        // byte[] tai_lieu_ky_moi_bytes = Convert.FromBase64String(tai_lieu_ky_moi_base64);
                        // string tai_lieu_ky_moi_path = @"wwwroot\fileTaiLieuHoSoTT\" + Guid.NewGuid().ToString().Replace("-", "") + ".pdf";
                        // File.WriteAllBytes(tai_lieu_ky_moi_path, tai_lieu_ky_moi_bytes);
                        string tai_lieu_ky_moi_path = _ftpService.UploadByteFileFTP(Convert.FromBase64String(output.data), hosottItem.HoSoThanhToan.MaHoSo, Guid.NewGuid().ToString().Replace("-", ""), "pdf");
                        if (tai_lieu_ky_moi_path == "") return new Response(message: "Lỗi: Không upload được file kí lên ftp server", data: "SV006", errorcode: "", success: false);

                        ksptt.TaiLieuKy = tai_lieu_ky_moi_path;
                        ksptt.NguoiKyId = nguoikyItem.Id;
                        itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.ThamTraHoSo;
                        itemApproveHSTT.NoiDung = ksptt.NoiDungKy;
                        _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);

                        var resp = _repo.KySoPhieuThamTraHoSo(ksptt);
                        if (resp.Success == true)
                        {
                            // gửi mail thông báo đến những user thực hiện bước sau
                            _utilsservice.SendMailKy(ksptt.ThaoTacBuocPheDuyetId, ksptt.HoSoThanhToanId);
                        }
                        transaction.Commit();
                        return resp;
                    }
                    else
                    {
                        itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.TuChoiThamTraHoSo;
                        itemApproveHSTT.NoiDung = "Từ chối";
                        _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);
                        transaction.Commit();
                        return new Response(message: "Chưa kí", data: "Bạn đã không đồng ý ký phiếu thẩm tra", errorcode: "004", success: false);
                    }
                }
                else
                {
                    transaction.Rollback();
                    return new Response(message: "Lỗi", data: "Không thể gọi dịch vụ ký", errorcode: "003", success: false);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response GetPhieuThamTraByIdHSTT(Guid HoSoTTId, Guid GiayToId)
        {
            var ret = _repo.GetPhieuThamTraByIdHSTT(HoSoTTId, GiayToId);
            return ret;
        }

        public Response KySoTiepNhanHoSo (ApproveHoSoTT itemApproveHSTT)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var nguoikyItem = _context.ApplicationUser.FirstOrDefault(x => (x.Id == itemApproveHSTT.NguoiThucHienId));
                if (nguoikyItem == null || nguoikyItem.PhoneNumber == null || nguoikyItem.CAProvider == null)
                    return new Response(message: "Lỗi", data: "Chưa cấu hình đủ thông tin để ký phiếu thẩm tra", errorcode: "005", success: false);

                var hosottItem = (from hs in _context.HoSoThanhToan
                                  where hs.HoSoId.ToString() == itemApproveHSTT.HoSoId
                                  select new
                                  {
                                    HoSoThanhToan = hs,
                                  }).FirstOrDefault();
                if (hosottItem.HoSoThanhToan.TrangThaiHoSo != BCXN.Statics.TrangThaiHoSo.ChuaTiepNhan)
                    return new Response(message: "Hồ sơ không thuộc hồ sơ tiếp nhận", data: null, errorcode: "007", success: false);
                string signURL;
                if (itemApproveHSTT.TrangThaiHoSo == BCXN.Statics.TrangThaiHoSo.YeuCauThaiDoi) {
                    //quay lại bước 1, xóa 3 bản ghi kí số tài liệu
                    var buocPheDuyet = _context.BuocPheDuyet.FirstOrDefault(b => b.BuocPheDuyetId == itemApproveHSTT.BuocPheDuyetId);
                    if (buocPheDuyet == null)
                    {
                        throw new Exception($"Không tìm thấy bước phê duyệt Id = {itemApproveHSTT.BuocPheDuyetId}");
                    }

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

                    //quay về bước 1
                    hosottItem.HoSoThanhToan.QuaTrinhPheDuyetId = buocPheDuyet1.BuocPheDuyetId;
                    itemApproveHSTT.TrangThaiPheDuyet = TrangThaiPheDuyetHoSo.TuChoiTiepNhanHoSo;

                    var kySoTaiLieuRemove = from tt in _context.KySoTaiLieu
                                        where tt.HoSoThanhToan.HoSoId.ToString() == itemApproveHSTT.HoSoId
                                        select tt;
                    foreach (var item in kySoTaiLieuRemove.ToList())
                    {
                        item.DaXoa = true;
                    }

                    _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);
                    transaction.Commit();
                    return new Response(message: "Từ chối ký phiếu thẩm tra thành công", data: "Bạn đã từ chối ký phiếu thẩm tra" , errorcode: "007", success: true);
                }
                Dictionary<string, object> payload = new Dictionary<string, object>()
                {
                    { "TypeSign", "MiniImage" },
                    { "Provider", nguoikyItem.CAProvider },
                    { "DINH_DANG_FILE", "doc"},
                    { "CommentSign", itemApproveHSTT.NoiDung }
                };

                if (nguoikyItem.CAProvider == "EVN_CA")
                {
                    payload.Add("DinhDanhKy", nguoikyItem.DinhDanhKy);
                    payload.Add("KEY_SIGN", "EVN_CA");
                    signURL = _configuration["CASettings:BaseAddress"] + "/SignServerV2/api/KySo/signPDFvs2/";
                }
                else
                {
                    payload.Add("DinhDanhKy", nguoikyItem.PhoneNumber);
                    signURL = _configuration["CASettings:BaseAddress"] + "/SignServerV2/api/KySo/signPDFvs2/";
                }

                 string tlk_base64str = "";
                 var getFile = _ftpService.DownloadFileFtpServerConvertBase64(hosottItem.HoSoThanhToan.TaiLieuKy);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();
                // tlk_bytes = File.ReadAllBytes(hosottItem.HoSoThanhToan.TaiLieuKy);
                payload.Add("FileType", Path.GetExtension(hosottItem.HoSoThanhToan.TaiLieuKy).Replace(".", "").ToLower());
                payload.Add("FullName", "./.");
                payload.Add("SignPosition", "down");
                payload.Add("FileDataSign", tlk_base64str);

                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    signURL,
                    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    resultResponse.Wait();
                    var CAResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                    // dynamic bankResponseJson = JToken.Parse(bankResponseString);
                    KetQuaKySoModel output = new KetQuaKySoModel();
                    output = JsonConvert.DeserializeObject<KetQuaKySoModel>(CAResponseString);
                    //string status = (string)bankResponseJson.State;
                   
                    //if (true)
                    if (output.state == "true")
                    {
                        //string tai_lieu_ky_moi_base64 = "";
                        // string tai_lieu_ky_moi_base64 = output.data;
                        // byte[] tai_lieu_ky_moi_bytes = Convert.FromBase64String(tai_lieu_ky_moi_base64);
                        // string tai_lieu_ky_moi_path = @"wwwroot\fileTaiLieuHoSoTT\" + Guid.NewGuid().ToString().Replace("-", "") + ".pdf";
                        // File.WriteAllBytes(tai_lieu_ky_moi_path, tai_lieu_ky_moi_bytes);
                        string tai_lieu_ky_moi_path = _ftpService.UploadByteFileFTP(Convert.FromBase64String(output.data), hosottItem.HoSoThanhToan.MaHoSo, Guid.NewGuid().ToString().Replace("-", ""), "pdf");
                        if (tai_lieu_ky_moi_path == "") return new Response(message: "Lỗi: Không upload được file kí lên ftp server", data: "SV006", errorcode: "", success: false);
                        hosottItem.HoSoThanhToan.TaiLieuKy = tai_lieu_ky_moi_path;

                        var resp = _HoSoThanhToanRepository.ApproveHoSo(itemApproveHSTT);
                        
                        // update "file dinh kem" la tai lieu ky vao chi tiet ho so
                        _ctgtrepo.UpdateTaiLieuToTrinh(hosottItem.HoSoThanhToan, tai_lieu_ky_moi_path, itemApproveHSTT.ThaoTacBuocPheDuyetId, itemApproveHSTT.NguoiThucHienId);
                        if (resp.StatusCode == 201)
                        {
                            // gửi mail thông báo đến những user thực hiện bước sau
                            _utilsservice.SendMailKy(itemApproveHSTT.ThaoTacBuocPheDuyetId, new Guid(itemApproveHSTT.HoSoId));
                            transaction.Commit();
                            return new Response(message: "Tiếp nhận hồ sơ thành công", data: "", errorcode: "00", success: true);
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Response(message: "Lỗi tiếp nhân hồ sơ", data: "Lỗi lưu trạng thái tiếp nhận hồ sơ", errorcode: "002", success: false);
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        return new Response(message: "Chưa kí", data: "Bạn đã không đồng ý ký nháy", errorcode: "004", success: false);
                    }
                }
                else
                {
                    transaction.Rollback();
                    return new Response(message: "Lỗi", data: "Không thể gọi dịch vụ ký", errorcode: "003", success: false);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }
    }
}
