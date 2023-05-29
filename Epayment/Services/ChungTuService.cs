using AutoMapper;
using BCXN.Data;
using BCXN.Repositories;
using BCXN.Statics;
using BCXN.ViewModels;
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

namespace Epayment.Services
{
    public interface IChungTuService
    {
        ResponseGetChungTu GetChungTu(ChungTuParams chungtu);
        Response ERPUpdateChungTu(ChungTuParams chungtu);
        Response ERPUpdateChungTuGhiSo(ChungTuParams ctgs);
        Response CreateChungTu(ChungTuParams chungtu);
        Response DayChungTuToiERP(Guid chungTuId, string userId);
        Response CreateChungTuDayErp(ChungTuParams chungtu);
        Response DeleteChungTu(ChungTuParams chungtu);
        Response KyChungTu(KySoChungTuParams request);
        ResponseGetKySoChungTu GetKySoChungTu(KySoChungTuParams kysochungtu);
        Dictionary<string, object> GetDuLieuBanTheHien(Guid chungTuId);
        Response CancelChungTu(string NguoiKyId, KySoChungTuParams request);
        Response GetChungTuByID(Guid id);
        Response UpdateTrangThaiDongCT(Guid ChungTuId, string UserId, Guid ThaoTacBuocPheDuyetId);
        Response GetPCUNC(ChungTuParams chungtu);
        Response GetCTGS(ChungTuParams chungtu);
    }

    public class ChungTuService : IChungTuService
    {
        private readonly IChungTuRepository _repo;
        private readonly IHoSoThanhToanRepository _hsttrepo;
        private readonly IChiTietHachToanRepository _cthtrepo;
        private readonly IDonViRepository _donvirepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IUtilsService _utilsservice;
        private readonly IGeneratePDFService _pcuncservice;
        private readonly IChiTietGiayToHSTTRepository _chiTietGiaoToHSTTRepo;
        private readonly IUploadToFtpService _uploadService;

        public ChungTuService(IChungTuRepository repo, IHoSoThanhToanRepository hsttrepo, IChiTietHachToanRepository cthtrepo,
            IDonViRepository donvirepo, IMapper mapper, IConfiguration configuration, ApplicationDbContext context
            , IUtilsService utilsservice, IGeneratePDFService pcuncservice, IChiTietGiayToHSTTRepository chiTietGiayToHSTTRepository,
            IUploadToFtpService uploadService)
        {
            _repo = repo;
            _hsttrepo = hsttrepo;
            _cthtrepo = cthtrepo;
            _donvirepo = donvirepo;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
            _utilsservice = utilsservice;
            _pcuncservice = pcuncservice;
            _chiTietGiaoToHSTTRepo = chiTietGiayToHSTTRepository;
            _uploadService = uploadService;
        }

        public Response GetChungTuByID(Guid id)
        {
            try
            {
                var data = _repo.GetChungTuByID(id);
                return new Response(message: "", data: data, errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public ResponseGetChungTu GetChungTu(ChungTuParams chungtu)
        {
            var resp = _repo.GetChungTu(chungtu);
            return resp;
        }

        public Response DeleteChungTu(ChungTuParams chungtu)
        {
            var resp = _repo.DeleteChungTu(chungtu);
            return resp;
        }

        public Response GetPCUNC(ChungTuParams chungtu)
        {
            var resp = _repo.GetPCUNC(chungtu);
            return resp;
        }

        public Response ERPUpdateChungTu(ChungTuParams chungtu)
        {
            var resp = _repo.ERPUpdateChungTu(chungtu);
            // if (resp.Success == true)
            // {
            //     if(chungtu.LoaiChungTu == 0) {
            //         string pcuncPath = _pcuncservice.GeneratePCUNC((Guid)resp.Data);
            //         _repo.UpdateTaiLieuChungtu(chungtuId: (Guid)resp.Data, tailieugoc: pcuncPath);
            //     }
            //     else if(chungtu.LoaiChungTu == 1) {
            //         string pcuncPath = _pcuncservice.GenerateCTGS((Guid)resp.Data);
            //         _repo.UpdateTaiLieuChungtu(chungtuId: (Guid)resp.Data, tailieugoc: pcuncPath);
            //     }
            // }
            return resp;
        }

        public Response ERPUpdateChungTuGhiSo(ChungTuParams ctgs)
        {
            var resp = _repo.ERPUpdateChungTuGhiSo(ctgs);
            return resp;
        }

        public Response CreateChungTu(ChungTuParams chungtu)
        {
            var resp = _repo.CreateChungTu(chungtu);
            return resp;
        }

        public Response DayChungTuToiERP(Guid chungTuId, string userId)
        {
            var userName = _context.ApplicationUser.FirstOrDefault(x => (x.Id == userId)).UserName;

            var chungTuItem = _repo.GetChungTuByID(chungTuId);
            if (chungTuItem == null || chungTuItem.DaXoa == true) return new Response(message: "Chứng từ đã bị hủy", data: "", errorcode: "003", success: false);          

            Dictionary<string, object> payload = new Dictionary<string, object> {
                { "userName" , userName },
                { "MaDonViErp", chungTuItem.DonViThanhToan.ERPMaCongTy.ToString() },
                { "MaChiNhanh", chungTuItem.DonViThanhToan.ERPMaChiNhanh.ToString() },
                { "TrangThaiGl", chungTuItem.CTGS_GL == false ? "N" : "Y" },
                { "TrangThaiAp", chungTuItem.CTGS_AP == false ? "N" : "Y" },
                { "TrangThaiCm", chungTuItem.CTGS_CM == false ? "N" : "Y" },
                { "NgayChungTu", chungTuItem.NgayYeuCauLapCT.ToString("dd/MM/yyyy HH:mm:ss")},
                { "NoiDungThanhToan", chungTuItem.NoiDungTT },
                { "MaHSTT", chungTuItem.HoSoThanhToan.MaHoSo },
                { "TenHSTT", chungTuItem.HoSoThanhToan.TenHoSo },
                { "TenNguoiThuHuong", chungTuItem.TenTaiKhoanNhan },
                { "TaiKhoanNguoiThuHuong", chungTuItem.SoTaiKhoanNhan.ToString() },
                { "LoaiTien", chungTuItem.LoaiTienTe },
                { "TyGia", chungTuItem.TyGia },
                { "SoTienNguyenTe", chungTuItem.SoTien  },
                { "SoTienQuyDoi", chungTuItem.SoTien*chungTuItem.TyGia },
                { "MaChiNhanhNhan", chungTuItem.MaChiNhanhNhan },
                { "TrangThaiHSTT", "Đã soát xét"}
            };
            HttpClient client = new HttpClient();
            var resultResponse = client.PostAsync(
                _configuration["ErpSettings:BaseAddress"],
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            );
            if (resultResponse.Result.Content != null)
            {
                resultResponse.Wait();
                var erpResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                dynamic erpResponseJson = JToken.Parse(erpResponseString);
                if (Int32.Parse((string)erpResponseJson.statusCode) == 1)
                {
                    return new Response(message: "Đẩy chứng từ tới erp thành công", data: "", errorcode: "", success: true);
                }
                else
                {
                    return new Response(message: "Lỗi: Chưa đẩy được chứng từ tới erp", data: "", errorcode: "004", success: false);
                }
            }
            else
            {
                return new Response(message: "Lỗi: Không tạo được chứng từ trên erp", data: "", errorcode: "002", success: false);
            }
        }

        public Response CreateChungTuDayErp(ChungTuParams chungtu)
        {
            var resp = _repo.CreateChungTu(chungtu);
            if (resp.Success == true)
            {
                Guid chungtuId = new Guid(resp.Data.ToString());
                var userName = _context.ApplicationUser.FirstOrDefault(x => (x.Id == chungtu.UserId)).UserName;
                _utilsservice.SendMailKy(chungtu.ThaoTacBuocPheDuyetId, chungtu.HoSoThanhToanId);
                var chungtuItem = _repo.GetChungTuByID(chungtuId);
                if (chungtuItem == null || chungtuItem.DaXoa == true) return new Response(message: "Chứng từ đã bị hủy", data: "", errorcode: "003", success: false);

                return DayChungTuToiERP(chungtuId, chungtu.UserId);
            }
            return resp;
        }

        public ResponseGetKySoChungTu GetKySoChungTu(KySoChungTuParams kysochungtu)
        {
            var resp = _repo.GetKySoChungTu(kysochungtu);
            return resp;
        }

        public Response KyChungTu(KySoChungTuParams request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var nguoikyItem = _context.ApplicationUser.FirstOrDefault(x => (x.Id == request.NguoiKyId));
                if (nguoikyItem == null || nguoikyItem.PhoneNumber == null || nguoikyItem.CAProvider == null)
                    return new Response(message: "Lỗi", data: "Chưa cấu hình đủ thông tin để ký chứng từ", errorcode: "009", success: false);
                //if( request.LoaiChungTu == 1 && request.CapKy == 1)
                // if( request.CapKy == 1) {
                //     string pcuncPath = _pcuncservice.GeneratePCUNC((Guid)request.ChungTuId);
                //     _repo.UpdateTaiLieuChungtu(chungtuId: (Guid)request.ChungTuId, tailieugoc: pcuncPath);
                // }
                // if( request.CapKy == 4) {
                //     string PathFileKySo = _pcuncservice.GenerateCTGS((Guid)request.ChungTuId);
                //     _repo.UpdateTaiLieuChungtu(chungtuId: (Guid)request.ChungTuId, tailieugoc: PathFileKySo);
                // }
                var chungTuItem = _repo.GetChungTuByID(request.ChungTuId.Value);
                if (chungTuItem == null) return new Response(message: "Không tìm thấy chứng từ", data: "", errorcode: "005", success: false);
                if (chungTuItem.TrangThaiCT == TrangThaiChungTu.KhoiTao) return new Response(message: "Chứng từ chưa được lập trên erp", data: "", errorcode: "006", success: false);
                if (chungTuItem.TrangThaiCT == TrangThaiChungTu.DaHuy) return new Response(message: "Chứng từ đã bị hủy", data: "", errorcode: "007", success: false);
                var buocPheDuyet = _context.BuocPheDuyet.FirstOrDefault(x => x.BuocPheDuyetId == request.BuocPheDuyetId);

                // tạo chuỗi xml từ nội dung ký, chứng từ id, cấp ký
                StringWriter sw = new StringWriter();
                XmlWriter writer = XmlWriter.Create(sw);
                writer.WriteStartElement("ChungTu");
                writer.WriteElementString("TenHoSo", chungTuItem.HoSoThanhToan.TenHoSo);
                writer.WriteElementString("MaHoSo", chungTuItem.HoSoThanhToan.MaHoSo);
                writer.WriteElementString("HanThanhToan", chungTuItem.HoSoThanhToan.HanThanhToan.ToString());
                writer.WriteElementString("SoTien", chungTuItem.SoTien.ToString());
                writer.WriteElementString("NoiDungTT", chungTuItem.NoiDungTT);
                writer.WriteElementString("TenTaiKhoanNhan", chungTuItem.TenTaiKhoanNhan);
                writer.WriteElementString("TenTaiKhoanChuyen", chungTuItem.TenTaiKhoanChuyen);
                writer.WriteElementString("MaNganHangChuyen", chungTuItem.MaNganHangChuyen);
                writer.WriteElementString("MaNganHangNhan", chungTuItem.MaNganHangNhan);
                writer.WriteElementString("NguoiKyId", request.NguoiKyId);
                writer.WriteElementString("NoiDungKy", request.NoiDungKy);
                writer.WriteElementString("ChungTuId", request.ChungTuId.ToString());
                writer.WriteElementString("CapKy", request.CapKy.ToString());
                writer.WriteEndElement();
                writer.Flush();
                string rawXmlString = sw.ToString();

                // load rsa key từ config
                RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
                rsaKey.ImportRSAPublicKey(Convert.FromBase64String(_configuration["RSAkey:public"]), out _);
                rsaKey.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["RSAkey:private"]), out _);

                RSACryptoServiceProvider rsaPublicKey = new RSACryptoServiceProvider();
                rsaPublicKey.ImportRSAPublicKey(Convert.FromBase64String(_configuration["RSAkey:public"]), out _);

                // kí ra chuỗi xml mới, lưu dữ liệu ký và chữ kí ra db
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(rawXmlString);
                SignedXml signedXml = new SignedXml(xmlDoc);
                signedXml.SigningKey = rsaKey;
                Reference reference = new Reference();
                reference.Uri = "";
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                signedXml.AddReference(reference);
                KeyInfo keyInfo = new KeyInfo();
                RSAKeyValue rkv = new RSAKeyValue(rsaPublicKey);
                keyInfo.AddClause(rkv);
                signedXml.KeyInfo = keyInfo;
                signedXml.ComputeSignature();
                XmlElement xmlDigitalSignature = signedXml.GetXml();
                xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

                // ký thêm evn ca
                string signURL, tai_lieu_ky_moi_path=null;
                Dictionary<string, object> payload = new Dictionary<string, object>()
                {
                    { "TypeSign", "Text" },
                    { "Provider", nguoikyItem.CAProvider },
                    { "CommentSign", request.NoiDungKy }
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
                if (request.CapKy == 1)
                {
                    var getFile = _uploadService.DownloadFileFtpServerConvertBase64(chungTuItem.TaiLieuGoc);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký cho cấp 1", data: "SV002", errorcode: "", success: false);
                    // tlk_bytes = File.ReadAllBytes(chungTuItem.TaiLieuGoc);
                    payload.Add("FileType", Path.GetExtension(chungTuItem.TaiLieuGoc).Replace(".", "").ToLower());
                    payload.Add("FullName", "Người lập");
                    // payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "down");
                    payload.Add("DINH_DANG_FILE", "doc");
                }
                else if (request.CapKy == 2)
                {
                    var getFile = _uploadService.DownloadFileFtpServerConvertBase64(chungTuItem.TaiLieuKy);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký cho cấp 2", data: "SV002", errorcode: "", success: false);
                    // tlk_bytes = File.ReadAllBytes(chungTuItem.TaiLieuKy);
                    payload.Add("FileType", Path.GetExtension(chungTuItem.TaiLieuKy).Replace(".", "").ToLower());
                    payload.Add("FullName", "Kế toán");
                    // payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "down");
                    payload.Add("DINH_DANG_FILE", "doc");
                }
                else if (request.CapKy == 3)
                {
                    var getFile = _uploadService.DownloadFileFtpServerConvertBase64(chungTuItem.TaiLieuKy);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký cho cấp 3", data: "SV002", errorcode: "", success: false);
                    // tlk_bytes = File.ReadAllBytes(chungTuItem.TaiLieuKy);
                    payload.Add("FileType", Path.GetExtension(chungTuItem.TaiLieuKy).Replace(".", "").ToLower());
                    payload.Add("FullName", "Chủ tài khoản");
                    // payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "down");
                    payload.Add("DINH_DANG_FILE", "doc");
                }
                else if (request.CapKy == 4)
                {
                    // tlk_bytes = File.ReadAllBytes(chungTuItem.TaiLieuGocCTGS);
                    var getFile = _uploadService.DownloadFileFtpServerConvertBase64(chungTuItem.TaiLieuGocCTGS);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký chứng từ ghi sổ cho cấp 1", data: "SV002", errorcode: "", success: false);
                    payload.Add("FileType", Path.GetExtension(chungTuItem.TaiLieuGocCTGS).Replace(".", "").ToLower());
                    payload.Add("FullName", "Người lập biểu");
                    // payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "down");
                    payload.Add("DINH_DANG_FILE", "doc");
                }
                else if (request.CapKy == 5)
                {
                    // tlk_bytes = File.ReadAllBytes(chungTuItem.TaiLieuKyCTGS);
                    var getFile = _uploadService.DownloadFileFtpServerConvertBase64(chungTuItem.TaiLieuKyCTGS);
                    if (getFile.Success == true) tlk_base64str = getFile.Data.ToString();                    
                    else return new Response(message: "Lỗi: không lấy được file cần ký chứng từ ghi sổ cho cấp 2", data: "SV002", errorcode: "", success: false);
                    payload.Add("FileType", Path.GetExtension(chungTuItem.TaiLieuKyCTGS).Replace(".", "").ToLower());
                    payload.Add("FullName", "Kế toán trưởng");
                    // payload.Add("FullName", nguoikyItem.FirstName + " " + nguoikyItem.LastName);
                    payload.Add("SignPosition", "down");
                    payload.Add("DINH_DANG_FILE", "doc");
                }
                else  
                {
                    transaction.Rollback();
                    return new Response(message: "Lỗi: Sai cấp ký", data: "011", errorcode: "", success: false); 
                }
                if(!string.IsNullOrEmpty(tlk_base64str)){
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
                    KetQuaKySoModel output = new KetQuaKySoModel();
                    output = JsonConvert.DeserializeObject<KetQuaKySoModel>(CAResponseString);
                    if (output.state == "true")
                    {
                        tai_lieu_ky_moi_path = _uploadService.UploadByteFileFTP(Convert.FromBase64String(output.data), chungTuItem.HoSoThanhToan.MaHoSo, Guid.NewGuid().ToString().Replace("-", ""), "pdf");
                        if (tai_lieu_ky_moi_path == "") return new Response(message: "Lỗi: Không upload được file kí lên ftp server", data: "SV006", errorcode: "", success: false);
                        // string tai_lieu_ky_moi_base64 = output.data;
                        // byte[] tai_lieu_ky_moi_bytes = Convert.FromBase64String(tai_lieu_ky_moi_base64);
                        // tai_lieu_ky_moi_path = @"wwwroot\fileTaiLieuHoSoTT\" + Guid.NewGuid().ToString().Replace("-", "") + ".pdf";
                        // File.WriteAllBytes(tai_lieu_ky_moi_path, tai_lieu_ky_moi_bytes);
                        // _chiTietGiaoToHSTTRepo.CreateChiTietGiayToHSTT()
                    }
                    else
                    {
                        transaction.Rollback();
                        return new Response(message: "Chưa kí", data: "Bạn đã không đồng ý ký chứng từ", errorcode: "012", success: false);
                    }
                }
                else
                {
                    transaction.Rollback();
                    return new Response(message: "Lỗi", data: "Không thể gọi dịch vụ ký", errorcode: "013", success: false);
                }

                var resp = _repo.KyChungTu(
                    new KySoChungTuParams
                    {
                        ChungTuId = request.ChungTuId,
                        NoiDungKy = request.NoiDungKy,
                        NgayKy = DateTime.Now,
                        DuLieuKy = xmlDoc.OuterXml,
                        CapKy = request.CapKy,
                        KySoId = request.KySoId,
                        NguoiKyId = request.NguoiKyId,
                        TaiLieuKy = tai_lieu_ky_moi_path,
                        TaiLieuGoc = chungTuItem.TaiLieuGoc,
                        LoaiChungTu = request.LoaiChungTu,
                        ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId
                    }
                );

                // ====== hot fix: chữ kí cấp 1 ko dùng dịch vụ kí tập trung ============
                if(request.CapKy == 1)
                {
                    var resp_hot_fix = _repo.HotFixKyChungTuCap1(
                        new KySoChungTuParams
                        {
                            ChungTuId = request.ChungTuId,
                            CapKy = request.CapKy,
                            // TaiLieuKy = _pcuncservice.GeneratePCUNC(request.ChungTuId.Value, false)
                            TaiLieuKy = tai_lieu_ky_moi_path
                        }
                    );
                }
                // ======================================================================

                if (resp.Success)
                {
                    if (resp.Data != null){
                        var ItemChungTu = _repo.GetChungTuByID(new Guid(resp.Data.ToString()));
                        if (ItemChungTu != null)
                        {
                            DayChungTuToiERP(ItemChungTu.ChungTuId, ItemChungTu.NguoiYeuCauLap.Id);
                            // đẩy chứng từ ghi sổ vào danh mục tài liệu trong hồ sơ
                            ParmPhieuThamTraHSTTViewModel param = new ParmPhieuThamTraHSTTViewModel();
                            param.HoSoThanhToanId = chungTuItem.HoSoThanhToan.HoSoId.ToString();
                            param.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId;
                            param.NguoiCapNhatId = request.NguoiKyId;
                            _chiTietGiaoToHSTTRepo.CreatePhieuThamTraHSTT(param, chungTuItem.TaiLieuKyCTGS);
                        }
                    }
                    // nếu ký thành công và cấp ký hiện tại là cấp 3 thì sẽ đẩy chứng từ thường vào danh mục tại liệu hồ sơ
                    if(request.CapKy == 3 && request.LoaiChungTu == 0){
                        ParmPhieuThamTraHSTTViewModel param = new ParmPhieuThamTraHSTTViewModel();
                        param.HoSoThanhToanId = chungTuItem.HoSoThanhToan.HoSoId.ToString();
                        param.ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId;
                        param.NguoiCapNhatId = request.NguoiKyId;
                        _chiTietGiaoToHSTTRepo.CreatePhieuThamTraHSTT(param, chungTuItem.TaiLieuKy);
                    }
                    var metadataChungTu = _repo.GetMetaDataByChungTuId(request.ChungTuId.Value);

                    //gửi mail
                    _utilsservice.SendMailKy(request.ThaoTacBuocPheDuyetId, chungTuItem.HoSoThanhToan.HoSoId);
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
                return resp;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "008", success: false);
            }
        }

        public Dictionary<string, object> GetDuLieuBanTheHien(Guid chungTuId)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            KySoChungTuParams p = new KySoChungTuParams();
            p.ChungTuId = chungTuId;
            var respKySo = _repo.GetKySoChungTuChoBanTheHien(p);
            res.Add("DanhSachChuKyCuaChungTu", respKySo.Data);

            var respChungTu = _repo.GetChungTuByID(chungTuId);
            res.Add("ChungTu", new Dictionary<string, object>() {
                { "capKy", respChungTu.CapKy },
                { "trangThaiCT", respChungTu.TrangThaiCT },
                { "hoSoThanhToan", new Dictionary<string, object>()
                    {
                        { "hoSoId", respChungTu.HoSoThanhToan.HoSoId } 
                    }
                }
            });
            return res;
        }

        public Response CancelChungTu(string NguoiKyId, KySoChungTuParams request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var resp = _repo.CancelChungTu(
                    NguoiKyId,
                    new KySoChungTuParams
                    {
                        ChungTuId = request.ChungTuId,
                        NoiDungKy = request.NoiDungKy,
                        NgayKy = DateTime.Now,
                        DuLieuKy = "",
                        CapKy = request.CapKy,
                        KySoId = request.KySoId,
                        HoSoId = request.HoSoId,
                        ThaoTacBuocPheDuyetId = request.ThaoTacBuocPheDuyetId
                    }
                );
                if (resp.Success)
                {
                    _utilsservice.SendMailHuy(request.ThaoTacBuocPheDuyetId, request.HoSoId);
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
                return resp;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response UpdateTrangThaiDongCT(Guid ChungTuId, string UserId, Guid thaoTacBuocPheDuyetId)
        {
            var resp = _repo.UpdateTrangThaiDongCT(ChungTuId, UserId, thaoTacBuocPheDuyetId);
            return resp;
        }

        public Response GetCTGS(ChungTuParams chungtu)
        {
            var resp = _repo.GetCTGS(chungtu);
            return resp;
        }
    }
}
