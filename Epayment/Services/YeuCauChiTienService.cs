using AutoMapper;
using Microsoft.Extensions.Configuration;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Epayment.ModelRequest;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using BCXN.Statics;
using BCXN;
using Epayment.Models;

namespace Epayment.Services
{
    public interface IYeuCauChiTienService
    {
        ResponseGetYeuCauChiTien GetYeuCauChiTien(SearchYeuCauChiTien ycct);
        Response CreateYeuCauChiTienChuyenKhoan(YeuCauChiTienParams ycct);
        Response UpdateKetQuaHachToanNganHang(YeuCauChiTienParams ycct);
        Response CreateYeuCauChiTienMat(YeuCauChiTienParams ycct);
    }

    public class YeuCauChiTienService : IYeuCauChiTienService
    {
        private readonly IYeuCauChiTienRepository _repo;
        private readonly IChungTuRepository _ctrepo;
        private readonly IHoSoThanhToanRepository _hsttrepo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<YeuCauChiTienService> _logger;
        private readonly IMaHoaPGDService _service;
        private readonly IVCBService _IVCBService;
        private readonly IBIDVService _bidvservice;
        private readonly IVTBService _vtbservice;

        public YeuCauChiTienService(IYeuCauChiTienRepository repo, IChungTuRepository ctrepo, IHoSoThanhToanRepository hsttrepo, IConfiguration configuration, ILogger<YeuCauChiTienService> logger, IMaHoaPGDService service, IVCBService IVCBService, IBIDVService bidvservice, IVTBService vtbservice)
        {
            _repo = repo;
            _ctrepo = ctrepo;
            _hsttrepo = hsttrepo;
            _configuration = configuration;
            _logger = logger;
            _service = service;
            _IVCBService = IVCBService;
            _bidvservice = bidvservice;
            _vtbservice = vtbservice;
        }

        public Response VerifyBankSignature(string original_data, string signature, string bankcode)
        {
            try
            {
                HttpClient signclient = new HttpClient();
                Dictionary<string, object> signpayload = new Dictionary<string, object>() {
                    { "hash", original_data },
                    { "signature", signature },
                    { "bankcode", bankcode }
                };
                var verifyResponse = signclient.PostAsync(
                    _configuration["RSAkeySign:VTB:BaseAddress"] + "/CryptoSSL/VerifyData",
                    new StringContent(JsonConvert.SerializeObject(signpayload), Encoding.UTF8, "application/json")
                );
                if (verifyResponse.Result.Content != null)
                {
                    verifyResponse.Wait();
                    var verifyResponseString = verifyResponse.Result.Content.ReadAsStringAsync().Result;
                    dynamic verifyResponseJson = JToken.Parse(verifyResponseString);
                    if (verifyResponseJson.Success == true) return new Response(message: "", data: "ok", errorcode: "", success: true);
                    else
                    {
                        _logger.LogWarning($"[VerifyBankSignature] Chữ kí không hợp lệ: {original_data}");
                        return new Response(message: "Lỗi: chữ kí không hợp lệ", data: "", errorcode: "VD001", success: false);
                    }
                }
                else
                {
                    _logger.LogWarning($"[GenerateBankSignature] Không gọi được api xác minh chữ ký: {original_data}");
                    return new Response(message: "Lỗi: không gọi được api xác minh chữ ký", data: "", errorcode: "VD002", success: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[VerifyBankSignature] Lỗi khi gọi api xác minh chữ kí: {original_data} {ex.Message} {ex.StackTrace}");
                return new Response(message: "Lỗi: Chưa xác định cho hàm xác minh chữ kí", data: ex.Message, errorcode: "VD003", success: false);
            }
        }

        public Response GenerateBankSignature(string original_data)
        {
            try
            {
                _logger.LogWarning($"[GenerateBankSignature] Dữ liệu ký: {original_data}");
                HttpClient signclient = new HttpClient();
                Dictionary<string, object> signpayload = new Dictionary<string, object>() {
                    { "data", original_data}
                };
                var signResponse = signclient.PostAsync(
                    _configuration["RSAkeySign:VTB:BaseAddress"] + "/CryptoSSL/SignData",
                    new StringContent(JsonConvert.SerializeObject(signpayload), Encoding.UTF8, "application/json")
                );
                if (signResponse.Result.Content != null)
                {
                    signResponse.Wait();
                    var signResponseString = signResponse.Result.Content.ReadAsStringAsync().Result;
                    dynamic signResponseJson = JToken.Parse(signResponseString);
                    if (signResponseJson.Success == true) return new Response(message: "", data: signResponseJson.Data, errorcode: "", success: true);
                    else
                    {
                        _logger.LogWarning($"[GenerateBankSignature] Không lấy được chữ ký: {original_data}");
                        return new Response(message: "Lỗi: không lấy được chữ ký", data: "", errorcode: "SD001", success: false);
                    }
                }
                else
                {
                    _logger.LogWarning($"[GenerateBankSignature] Không gọi được api ký: {original_data}");
                    return new Response(message: "Lỗi: không gọi được api ký", data: "", errorcode: "SD002", success: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[GenerateBankSignature] Lỗi khi gọi api ký: {original_data} {ex.Message} {ex.StackTrace}");
                return new Response(message: "Lỗi: Chưa xác định cho hàm lấy chữ kí", data: ex.Message, errorcode: "SD003", success: false);
            }
        }

        public ResponseGetYeuCauChiTien GetYeuCauChiTien(SearchYeuCauChiTien ycct)
        {
            var resp = _repo.GetYeuCauChiTien(ycct);
            return resp;
        }

        public Response UpdateKetQuaHachToanNganHang(YeuCauChiTienParams ycct)
        {
            try
            {
                // cập nhật số liệu trên epayment
                var chungtuItem = _ctrepo.GetChungTuByYeuCauChi(ycct.YeuCauChiTienId);
                if (chungtuItem == null) return new Response(message: "Yêu cầu chi tiền này không thuộc epayment", data: "", errorcode: "002", success: false);

                YeuCauChiTienParams p = new YeuCauChiTienParams();
                p.ChungTuId = chungtuItem.ChungTuId;
                p.MaKetQuaChi = ycct.MaKetQuaChi;
                p.ThoiGianChi = ycct.ThoiGianChi;
                p.YeuCauChiTienId = ycct.YeuCauChiTienId;
                _repo.ERPUpdateYeuCauChiTien(p);

                if (ycct.MaKetQuaChi.ToLower() == "true")
                {
                    // update trạng thái hồ sơ
                    var hosottItem = _hsttrepo.GetHoSoTTByCTId(chungtuItem.ChungTuId).Data[0];
                    _hsttrepo.UpDateTTHoSoTT(hosottItem.maHoSo, BCXN.Statics.TrangThaiHoSo.DaThanhToan, ycct.ThoiGianChi);

                    // cập nhật số liệu sang erp
                    var resp = _ctrepo.GetKySoChungTuByERPChungTuId(chungtuItem.ChungTuERPId);
                    Dictionary<string, object> payload = new Dictionary<string, object> {
                        { "MaChungTuERP", Convert.ToInt32(resp.Data["ChungTuERPId"]) },
                        { "MaHSTT", resp.Data["MaHoSo"] },
                        { "MaDonViERP", resp.Data["MaDonViERP"] },
                        { "NgayChuyenNganHang", ((DateTime)resp.Data["NgayGuiLenhChi"]).ToString("dd/MM/yyyy HH:mm:ss") },
                        { "NgayNganHangThanhToan", ycct.ThoiGianChi.ToString("dd/MM/yyyy HH:mm:ss")},
                        { "ChuKy1", resp.Data["TenNguoiKy1"] },
                        { "ChuKy2", resp.Data["TenNguoiKy2"] },
                        { "ChuKy3", resp.Data["TenNguoiKy3"] },
                        { "ChuKy4", resp.Data["TenNguoiKy4"] },
                        { "TrangThaiHSTT", hosottItem.trangThaiHoSo.ToString() }
                    };
                    HttpClient client = new HttpClient();
                    var resultResponse = client.PutAsync(
                        _configuration["ErpSettings:UpdateTrangThaiHachToanAddress"],
                        new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                    );
                    if (resultResponse.Result.Content != null)
                    {
                        resultResponse.Wait();
                        var erpResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(JsonConvert.SerializeObject(payload));
                        Console.WriteLine(erpResponseString);

                        dynamic erpResponseJson = JToken.Parse(erpResponseString);
                        if (Int32.Parse((string)erpResponseJson.statusCode) == 1)
                        {
                            p.TrangThaiChi = TrangThaiChiTien.DaCapNhatKetQuaERP;
                            _repo.UpdateYeuCauChiTien(p);
                        }
                    }
                }

                return new Response(message: "Đã nhận kết quả của gateway", data: "", errorcode: "", success: true);
            }
            catch (Exception ex)
            {
                return new Response(message: "Lỗi", data: ex.Message, errorcode: "001", success: false);
            }
        }

        public Response ChuyenKhoanVTB(YeuCauChiTienParams ycct, ChungTu chungtuItem)
        {
            // tạo bản ghi chi tiền trong db
            Response resp = _repo.CreateYeuCauChiTien(
                new YeuCauChiTienParams
                {
                    ChungTuId = ycct.ChungTuId,
                    NguoiYeuCauChiId = ycct.NguoiYeuCauChiId,
                    NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe
                }
            );

            try
            {
                if (resp.Success == false)
                {
                    _logger.LogWarning($"[CreateYeuCauChiTien]Không cập nhật được dữ liệu chi tiền vào db cho chứng từ: {ycct.ChungTuId}");
                    return new Response(message: "Không cập nhật được dữ liệu chi tiền vào db", data: "", errorcode: "101", success: false);
                }
                else
                {
                    Guid requestID = (Guid)resp.Data;
                    string feeAccount = chungtuItem.SoTaiKhoanChuyen;
                    string feeType = chungtuItem.LoaiPhi;
                    string transtimeStr = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string signedString = "";

                    string merchantId = _configuration["BankSettings:VTB:merchantId"];
                    string providerId = _configuration["BankSettings:VTB:providerId"];
                    string approver = _configuration["BankSettings:VTB:approver"];
                    string senderAddr = _configuration["BankSettings:VTB:senderAddr"];
                    string priority = _configuration["BankSettings:VTB:priority"];
                    string softwareProviderId = _configuration["BankSettings:VTB:softwareProviderId"];
                    string appointedApprover = _configuration["BankSettings:VTB:appointedApprover"];
                    string channel = _configuration["BankSettings:VTB:channel"];
                    string version = _configuration["BankSettings:VTB:version"];
                    string clientIP = _configuration["BankSettings:VTB:clientIP"];
                    string language = _configuration["BankSettings:VTB:language"];
                    string model = _configuration["BankSettings:VTB:model"];
                    // tạo tham số chi tiền
                    string noidungTT = "";
                    if (chungtuItem.NoiDungTT != null)
                    {
                        noidungTT = StringUtils.RemoveSign4VietnameseString(chungtuItem.NoiDungTT);
                        noidungTT = StringUtils.RemoveStringSpecial(noidungTT);
                    }
                    string strloaigiaodich = "";
                    if (chungtuItem.LoaiGiaoDich != null)
                    {
                        strloaigiaodich = chungtuItem.LoaiGiaoDich.ToLower();
                    }
                    string amount = chungtuItem.SoTien.ToString().Replace(".00", "").Replace(",00", "");
                    List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
                    records.Add(
                        new Dictionary<string, object>()
                        {
                        { "transId", requestID.ToString() },
                        { "approver", approver},
                        { "transType", strloaigiaodich},
                        { "amount", amount},
                        { "recvAcctId", chungtuItem.SoTaiKhoanNhan },
                        { "recvBankId", chungtuItem.MaNganHangNhan },
                        { "recvBranchId", chungtuItem.MaChiNhanhNhan },
                        { "recvBankName", chungtuItem.TenNganHangNhan},
                        { "recvAcctName", chungtuItem.TenTaiKhoanNhan},
                        { "recvAddr", ""},
                        { "recvPhoneNo", ""},
                        { "currencyCode", chungtuItem.LoaiTienTe },
                        { "remark", noidungTT },
                        { "senderBankId", chungtuItem.MaNganHangChuyen },
                        { "senderBranchId", chungtuItem.MaChiNhanhChuyen },
                        { "senderAddr", senderAddr },
                        { "senderAcctName", chungtuItem.TenTaiKhoanChuyen},
                        { "senderAcctId", chungtuItem.SoTaiKhoanChuyen }
                        }
                    );
                    string loop = requestID.ToString() + chungtuItem.SoTaiKhoanChuyen + chungtuItem.SoTaiKhoanNhan + amount;

                    // tạo tham số gen chữ kí số
                    string scheduleDateString = "";
                    if (chungtuItem.NgayGiaoDichThucTe != null && chungtuItem.NgayGiaoDichThucTe.Date > DateTime.Now.Date)
                        scheduleDateString = chungtuItem.NgayGiaoDichThucTe.ToString("dd/MM/yyyy");
                    string sigmerchantId = "";
                    if (merchantId != null)
                        sigmerchantId = merchantId;

                    string original_data = requestID + providerId + model + priority
                        + softwareProviderId + appointedApprover + feeAccount + feeType + scheduleDateString
                        + approver + loop + transtimeStr + channel + version + clientIP + language;

                    // gọi api gen chữ kí số
                    Response signResp = GenerateBankSignature(original_data);
                    if (signResp.Success == true) signedString = signResp.Data.ToString();
                    else return signResp;

                    // tiếp tục tạo tham số chuyển ngân hàng
                    Dictionary<string, object> payload = new Dictionary<string, object>();
                    payload.Add("model", model);
                    payload.Add("requestId", requestID.ToString());
                    payload.Add("providerId", providerId);
                    //payload.Add("merchantId", merchantId);
                    payload.Add("merchantId", null);
                    payload.Add("priority", priority);
                    payload.Add("version", version);
                    payload.Add("softwareProviderId", softwareProviderId);
                    payload.Add("language", language);
                    if (chungtuItem.NgayGiaoDichThucTe != null && chungtuItem.NgayGiaoDichThucTe.Date > DateTime.Now.Date)
                        payload.Add("scheduledDate", chungtuItem.NgayGiaoDichThucTe.ToString("dd/MM/yyyy"));
                    else
                        payload.Add("scheduledDate", null);
                    payload.Add("appointedApprover", appointedApprover);
                    payload.Add("approver", approver);
                    payload.Add("transTime", transtimeStr);
                    payload.Add("channel", channel);
                    payload.Add("clientIP", clientIP);
                    payload.Add("feeAccount", feeAccount);
                    payload.Add("feeType", feeType);
                    payload.Add("records", records);
                    payload.Add("signature", signedString);

                    // call api chi tiền 
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("sBankCode", chungtuItem.TenNganHangChuyen);
                    client.DefaultRequestHeaders.Add("sFunction", "transfer");
                    client.DefaultRequestHeaders.Add("sRequestID", requestID.ToString());
                    client.DefaultRequestHeaders.Add("sZone", "EVN");
                    client.DefaultRequestHeaders.Add("sDonvi", chungtuItem.DonViThanhToan.ERPMaCongTy);

                    _logger.LogWarning($"[CreateYeuCauChiTien] Tham số truyền lên: {JsonConvert.SerializeObject(payload)}");

                    Console.WriteLine(JsonConvert.SerializeObject(payload));

                    var resultResponse = client.PostAsync(
                        _configuration["BankSettings:VTB:TransferBaseAddress"],
                        new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                    );
                    if (resultResponse.Result.Content != null)
                    {
                        resultResponse.Wait();
                        var bankResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                        _logger.LogWarning($"[CreateYeuCauChiTien] Bank response: {bankResponseString}");
                        dynamic bankResponseJson = JToken.Parse(bankResponseString);
                        int bankResponseCode = bankResponseJson.records[0].code;
                        string bankResponseMsg = bankResponseJson.records[0].message;

                        int bankResponseProcessedRecords = bankResponseJson.processedRecords;
                        string bankResponseRequestId = bankResponseJson.requestId;
                        string bankResponseProviderId = bankResponseJson.providerId;
                        string bankResponseMerchantId = bankResponseJson.merchantId;
                        string signData = bankResponseRequestId + bankResponseProviderId + bankResponseMerchantId + bankResponseCode.ToString() + bankResponseProcessedRecords;
                        string signature = bankResponseJson.signature;
                        Response verifyResp = VerifyBankSignature(signData, signature, chungtuItem.TenNganHangChuyen);
                        if (verifyResp.Success == false) return verifyResp;

                        _logger.LogWarning($"[CreateYeuCauChiTien] Đã gửi lệnh chi tiền cho chứng từ: {ycct.ChungTuId}, với response: {bankResponseMsg}");
                        if (bankResponseCode == 1)
                        {
                            _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                            {
                                ChungTuId = ycct.ChungTuId,
                                YeuCauChiTienId = requestID,
                                TrangThaiChi = TrangThaiChiTien.GuiLenhChiTienThanhCong,
                                ChuKy = signedString,
                                MaKetQuaChi = bankResponseMsg
                            });
                            return new Response(message: "Gửi lệnh chi tiền thành công", data: "", errorcode: "", success: true);
                        }
                        else
                        {
                            _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                            {
                                ChungTuId = ycct.ChungTuId,
                                YeuCauChiTienId = requestID,
                                TrangThaiChi = TrangThaiChiTien.LenhChiTienLoiThamSo,
                                ChuKy = signedString,
                                MaKetQuaChi = bankResponseMsg
                            });
                            return new Response(message: bankResponseMsg, data: "", errorcode: "104", success: false);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"[CreateYeuCauChiTien] Không gửi được lệnh chi tiền tới ngân hàng cho chứng từ {ycct.ChungTuId}");
                        _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                        {
                            ChungTuId = ycct.ChungTuId,
                            YeuCauChiTienId = requestID,
                            ChuKy = signedString,
                            TrangThaiChi = TrangThaiChiTien.KhongGuiDuocLenhChi,
                            MaKetQuaChi = "Không gửi được lệnh chi tiền tới ngân hàng"
                        });
                        return new Response(message: "Không gửi được lệnh chi tiền tới ngân hàng", data: "", errorcode: "105", success: false);
                    }
                }
            }
            catch (Exception e)
            {
                if (resp != null)
                {
                    _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                    {
                        ChungTuId = ycct.ChungTuId,
                        YeuCauChiTienId = (Guid)resp.Data,
                        TrangThaiChi = TrangThaiChiTien.KhongGuiDuocLenhChi,
                        MaKetQuaChi = "Lỗi nội bộ hệ thống EPayment"
                    });
                }
                _logger.LogWarning($"[CreateYeuCauChiTien] Không gửi được lệnh chi tiền tới ngân hàng cho chứng từ {ycct.ChungTuId} {e.Message} {e.StackTrace}");
                return new Response(message: "Lỗi nội bộ hệ thống EPayment", data: "", errorcode: "106", success: false);
            }
        }

        public Response ChuyenKhoanVCB(YeuCauChiTienParams ycct, ChungTu chungtuItem)
        {
            try
            {
                // tạo bản ghi chi tiền trong db
                var resp = _repo.CreateYeuCauChiTien(
                new YeuCauChiTienParams
                {
                    ChungTuId = ycct.ChungTuId,
                    NguoiYeuCauChiId = ycct.NguoiYeuCauChiId,
                    NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe
                }
                );
                if (resp.Success == false)
                {
                    _logger.LogWarning($"[CreateYeuCauChiTien]Không cập nhật được dữ liệu chi tiền vào db cho chứng từ: {ycct.ChungTuId}");
                    return new Response(message: "Không cập nhật được dữ liệu chi tiền vào db", data: "", errorcode: "201", success: false);
                }
                else
                {
                    Guid requestID = (Guid)resp.Data;
                    ycct.YeuCauChiTienId = requestID;
                    var vcbResp = _service.CreateYeuCauChuyenKhoanVCB(ycct);
                    // tạo chữ kí số
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["RSAkey:private"]), out _);
                    ASCIIEncoding ByteConverter = new ASCIIEncoding();
                    string original_data = ycct.YeuCauChiTienId.ToString();
                    byte[] signedByte = rsa.SignData(ByteConverter.GetBytes(original_data), SHA256.Create());
                    string signedString = Convert.ToBase64String(signedByte);
                    if (vcbResp.Success == true)
                    {
                        _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                        {
                            ChungTuId = ycct.ChungTuId,
                            TrangThaiChi = TrangThaiChiTien.GuiLenhChiTienThanhCong,
                            YeuCauChiTienId = requestID,
                            MaKetQuaChi = "Đã gửi lệnh chi",
                            ChuKy = signedString
                        });
                        return new Response(message: "Gửi lệnh chi tiền thành công", data: "", errorcode: "", success: true);
                    }
                    else
                    {
                        // _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                        // {
                        //     ChungTuId = ycct.ChungTuId,
                        //     YeuCauChiTienId = requestID,
                        //     TrangThaiChi = TrangThaiChiTien.LenhChiTienLoiThamSo,
                        //     ChuKy = signedString,
                        //     MaKetQuaChi = vcbResp.Message
                        // });
                        return new Response(message: "Có lỗi trong quá trình đẩy lệnh chuyển tiền: " + vcbResp.Message, data: "", errorcode: "202", success: false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[ChuyenKhoanVCB]Lỗi: {ex}");
                return new Response(message: ex.ToString(), data: "", errorcode: "203", success: false);
            }
        }

        public Response ChuyenKhoanBIDV(YeuCauChiTienParams ycct, ChungTu chungtuItem)
        {
            // tạo bản ghi chi tiền trong db
            Response resp = _repo.CreateYeuCauChiTien(
                new YeuCauChiTienParams
                {
                    ChungTuId = ycct.ChungTuId,
                    NguoiYeuCauChiId = ycct.NguoiYeuCauChiId,
                    NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe
                }
            );
            try
            {
                if (resp.Success == false)
                {
                    _logger.LogWarning($"[CreateYeuCauChiTien]Không cập nhật được dữ liệu chi tiền vào db cho chứng từ: {ycct.ChungTuId}");
                    return new Response(message: "Không cập nhật được dữ liệu chi tiền vào db", data: "", errorcode: "301", success: false);
                }
                else
                {
                    Guid requestID = (Guid)resp.Data;
                    string feeAccount = chungtuItem.SoTaiKhoanChuyen;
                    string feeType = chungtuItem.LoaiPhi;
                    string transtimeStr = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string signedString = "";

                    string merchantId = _configuration["BankSettings:BIDV:merchantId"];
                    string providerId = _configuration["BankSettings:BIDV:providerId"];
                    string approver = _configuration["BankSettings:BIDV:approver"];
                    string senderAddr = _configuration["BankSettings:BIDV:senderAddr"];
                    string priority = _configuration["BankSettings:BIDV:priority"];
                    string softwareProviderId = _configuration["BankSettings:BIDV:softwareProviderId"];
                    string appointedApprover = _configuration["BankSettings:BIDV:appointedApprover"];
                    string channel = _configuration["BankSettings:BIDV:channel"];
                    string version = _configuration["BankSettings:BIDV:version"];
                    string clientIP = _configuration["BankSettings:BIDV:clientIP"];
                    string language = _configuration["BankSettings:BIDV:language"];
                    string model = _configuration["BankSettings:BIDV:model"];
                    // tạo tham số chi tiền
                    string noidungTT = "";
                    if (chungtuItem.NoiDungTT != null)
                    {
                        noidungTT = StringUtils.RemoveSign4VietnameseString(chungtuItem.NoiDungTT);
                        noidungTT = StringUtils.RemoveStringSpecial(noidungTT);
                    }
                    string strloaigiaodich = "";
                    if (chungtuItem.LoaiGiaoDich != null)
                    {
                        strloaigiaodich = chungtuItem.LoaiGiaoDich.ToLower();
                    }
                    string amount = chungtuItem.SoTien.ToString().Replace(".00", "").Replace(",00", "");
                    List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
                    records.Add(
                        new Dictionary<string, object>()
                        {
                        { "transId", requestID.ToString() },
                        { "approver", approver},
                        { "transType", strloaigiaodich },
                        { "amount", amount},
                        { "recvAcctId", chungtuItem.SoTaiKhoanNhan },
                        { "recvBankId", chungtuItem.MaNganHangNhan },
                        { "recvBranchId", chungtuItem.MaChiNhanhNhan },
                        { "recvBankName", chungtuItem.TenNganHangNhan},
                        { "recvAcctName", chungtuItem.TenTaiKhoanNhan},
                        { "recvAddr", ""},
                        { "recvPhoneNo", ""},
                        { "currencyCode", chungtuItem.LoaiTienTe },
                        { "remark", noidungTT },
                        { "senderBankId", chungtuItem.MaNganHangChuyen },
                        { "senderBranchId", chungtuItem.MaChiNhanhChuyen },
                        { "senderAddr", senderAddr },
                        { "senderAcctName", chungtuItem.TenTaiKhoanChuyen},
                        { "senderAcctId", chungtuItem.SoTaiKhoanChuyen }
                        }
                    );
                    string loop = requestID.ToString() + chungtuItem.SoTaiKhoanChuyen + chungtuItem.SoTaiKhoanNhan + amount;

                    // tạo chữ kí số
                    string scheduleDateString = "";
                    if (chungtuItem.NgayGiaoDichThucTe != null && chungtuItem.NgayGiaoDichThucTe.Date > DateTime.Now.Date)
                        scheduleDateString = chungtuItem.NgayGiaoDichThucTe.ToString("dd/MM/yyyy");
                    string sigmerchantId = "";
                    if (merchantId != null)
                        sigmerchantId = merchantId;

                    string original_data = requestID + providerId + sigmerchantId + model + priority
                        + softwareProviderId + appointedApprover + feeAccount + feeType + scheduleDateString
                        + approver + loop + transtimeStr + channel + version + clientIP + language;

                    // gọi api gen chữ kí số
                    Response signResp = GenerateBankSignature(original_data);
                    if (signResp.Success == true) signedString = signResp.Data.ToString();
                    else return signResp;

                    // tiếp tục tạo tham số chuyển tiền
                    Dictionary<string, object> payload = new Dictionary<string, object>();
                    payload.Add("model", model);
                    payload.Add("requestId", requestID.ToString());
                    payload.Add("providerId", providerId);
                    payload.Add("merchantId", merchantId);
                    //payload.Add("merchantId", "");
                    payload.Add("priority", priority);
                    payload.Add("version", version);
                    payload.Add("softwareProviderId", softwareProviderId);
                    payload.Add("language", language);
                    if (chungtuItem.NgayGiaoDichThucTe != null && chungtuItem.NgayGiaoDichThucTe.Date > DateTime.Now.Date)
                        payload.Add("scheduledDate", chungtuItem.NgayGiaoDichThucTe.ToString("dd/MM/yyyy"));
                    else
                        payload.Add("scheduledDate", "");
                    payload.Add("appointedApprover", appointedApprover);
                    payload.Add("approver", approver);
                    payload.Add("transTime", transtimeStr);
                    payload.Add("channel", channel);
                    payload.Add("clientIP", clientIP);
                    payload.Add("feeAccount", feeAccount);
                    payload.Add("feeType", feeType);
                    payload.Add("records", records);
                    payload.Add("signature", signedString);

                    // call api chi tiền 
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("sBankCode", chungtuItem.TenNganHangChuyen);
                    client.DefaultRequestHeaders.Add("sFunction", "transfer");
                    client.DefaultRequestHeaders.Add("sRequestID", requestID.ToString());
                    client.DefaultRequestHeaders.Add("sZone", "EVN");
                    client.DefaultRequestHeaders.Add("sDonvi", chungtuItem.DonViThanhToan.ERPMaCongTy);

                    _logger.LogWarning($"[CreateYeuCauChiTien] Tham số truyền lên: {JsonConvert.SerializeObject(payload)}");

                    var resultResponse = client.PostAsync(
                        _configuration["BankSettings:BIDV:TransferURL"],
                        new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                    );

                    Console.WriteLine(JsonConvert.SerializeObject(payload));

                    if (resultResponse.Result.Content != null)
                    {
                        resultResponse.Wait();
                        var bankResponseString = resultResponse.Result.Content.ReadAsStringAsync().Result;
                        _logger.LogWarning($"[CreateYeuCauChiTien] Bank response: {bankResponseString}");
                        dynamic bankResponseJson = JToken.Parse(bankResponseString);
                        int bankResponseCode = bankResponseJson.status.code;
                        string bankResponseMsg = bankResponseJson.status.message;

                        int bankResponseProcessedRecords = bankResponseJson.processedRecords;
                        int bankResponseModel = bankResponseJson.model;
                        string bankResponseSoftwareProviderId = bankResponseJson.softwareProviderId;
                        string bankResponseRequestId = bankResponseJson.requestId;
                        string bankResponseProviderId = bankResponseJson.providerId;
                        string bankResponseMerchantId = bankResponseJson.merchantId;
                        string signature = bankResponseJson.signature;
                        string signData = bankResponseRequestId + bankResponseProviderId + bankResponseMerchantId + bankResponseModel.ToString() + bankResponseSoftwareProviderId + bankResponseProcessedRecords.ToString() + bankResponseCode.ToString();

                        Response verifyResp = VerifyBankSignature(signData, signature, chungtuItem.TenNganHangChuyen);
                        if (verifyResp.Success == false) return verifyResp;

                        _logger.LogWarning($"[CreateYeuCauChiTien] Đã gửi lệnh chi tiền cho chứng từ: {ycct.ChungTuId}, với response: {bankResponseMsg}");
                        if (bankResponseCode == 1)
                        {
                            _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                            {
                                ChungTuId = ycct.ChungTuId,
                                YeuCauChiTienId = requestID,
                                TrangThaiChi = TrangThaiChiTien.GuiLenhChiTienThanhCong,
                                ChuKy = signedString,
                                MaKetQuaChi = bankResponseMsg
                            });
                            return new Response(message: "Gửi lệnh chi tiền thành công", data: "", errorcode: "", success: true);
                        }
                        else
                        {
                            _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                            {
                                ChungTuId = ycct.ChungTuId,
                                YeuCauChiTienId = requestID,
                                TrangThaiChi = TrangThaiChiTien.LenhChiTienLoiThamSo,
                                ChuKy = signedString,
                                MaKetQuaChi = bankResponseMsg
                            });
                            return new Response(message: bankResponseMsg, data: "", errorcode: "304", success: false);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"[CreateYeuCauChiTien] Không gửi được lệnh chi tiền tới ngân hàng cho chứng từ {ycct.ChungTuId}");
                        _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                        {
                            ChungTuId = ycct.ChungTuId,
                            YeuCauChiTienId = requestID,
                            ChuKy = signedString,
                            TrangThaiChi = TrangThaiChiTien.KhongGuiDuocLenhChi,
                            MaKetQuaChi = "Không gửi được lệnh chi tiền tới ngân hàng"
                        });
                        return new Response(message: "Không gửi được lệnh chi tiền tới ngân hàng", data: "", errorcode: "305", success: false);
                    }
                }
            }
            catch (Exception e)
            {
                if (resp != null)
                {
                    _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                    {
                        ChungTuId = ycct.ChungTuId,
                        YeuCauChiTienId = (Guid)resp.Data,
                        TrangThaiChi = TrangThaiChiTien.KhongGuiDuocLenhChi,
                        MaKetQuaChi = "Lỗi nội bộ hệ thống EPayment"
                    });
                }
                _logger.LogWarning($"[CreateYeuCauChiTien] Không gửi được lệnh chi tiền tới ngân hàng cho chứng từ {ycct.ChungTuId} {e.Message} {e.StackTrace}");
                return new Response(message: "Lỗi nội bộ hệ thống EPayment", data: "", errorcode: "306", success: false);
            }
        }

        public Response CreateYeuCauChiTienChuyenKhoan(YeuCauChiTienParams ycct)
        {
            try
            {
                var chungtuItem = _ctrepo.GetChungTuByID(ycct.ChungTuId);
                if (_ctrepo.DaChiTien(chungtuItem) == true) return new Response(message: "Đã tạo lệnh chi tiền trước đó", data: "", errorcode: "001", success: false);

                if (String.IsNullOrEmpty(chungtuItem.TenNganHangChuyen))
                {
                    return new Response(message: "Thiếu thông tin ngân hàng chuyển", data: "", errorcode: "002", success: false);
                }

                //if (chungtuItem.TenNganHangChuyen == _configuration["BankSettings:VTB:bankCode"]) return ChuyenKhoanVTB(ycct, chungtuItem);
                //else if (chungtuItem.TenNganHangChuyen == _configuration["BankSettings:VCB:bankCode"]) return ChuyenKhoanVCB(ycct, chungtuItem);
                //else if (chungtuItem.TenNganHangChuyen == _configuration["BankSettings:BIDV:bankCode"]) return ChuyenKhoanBIDV(ycct, chungtuItem);
                if (chungtuItem.TenNganHangChuyen == _configuration["BankSettings:VTB:bankCode"]) return _vtbservice.ChuyenKhoanVTB(ycct, chungtuItem);
                else if (chungtuItem.TenNganHangChuyen == _configuration["BankSettings:VCB:bankCode"]) return _IVCBService.ChuyenKhoanVCB(ycct, chungtuItem);
                else if (chungtuItem.TenNganHangChuyen == _configuration["BankSettings:BIDV:bankCode"]) return _bidvservice.ChuyenKhoanBIDV(ycct, chungtuItem);
                else
                {
                    _logger.LogWarning($"[CreateYeuCauChiTien] Không tìm thấy ngân hàng chuyển: {chungtuItem.TenNganHangChuyen}");
                    return new Response(message: "Không tìm thấy ngân hàng chuyển", data: "", errorcode: "003", success: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[CreateYeuCauChiTien] Lỗi chưa xác định cho chứng từ {ycct.ChungTuId} {ex.Data} {ex.StackTrace}");
                return new Response(message: "Lỗi chưa xác định của hệ thống EPayment", data: ex.StackTrace, errorcode: "004", success: false);
            }
        }

        public Response CreateYeuCauChiTienMat(YeuCauChiTienParams ycct)
        {
            var chungtuItem = _ctrepo.GetChungTuByID(ycct.ChungTuId);
            string requestId = Guid.NewGuid().ToString().Replace("-", "");

            // tạo chữ kí số
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["RSAkey:private"]), out _);
            ASCIIEncoding ByteConverter = new ASCIIEncoding();
            string original_data = requestId;
            byte[] signedByte = rsa.SignData(ByteConverter.GetBytes(original_data), SHA256.Create());
            string signedString = Convert.ToBase64String(signedByte);

            var resp = _repo.CreateYeuCauChiTienMat(
                new YeuCauChiTienParams
                {
                    ChungTuId = ycct.ChungTuId,
                    SoTienTT = ycct.SoTienTT,
                    NguoiThuHuong = ycct.NguoiThuHuong,
                    ChuKy = signedString,
                    NguoiYeuCauChiId = ycct.NguoiYeuCauChiId,
                    HoSoId = ycct.HoSoId,
                    NgayGiaoDichThucTe = ycct.NgayGiaoDichThucTe
                }
            );
            return resp;
        }
    }
}
