using BCXN.ViewModels;
using Epayment.Models;
using Epayment.Repositories;
using Epayment.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using BCXN.Statics;

namespace Epayment.Services
{
    public interface IVTBService
    {
        Response ChuyenKhoanVTB(YeuCauChiTienParams ycct, ChungTu chungtuItem);
    }

    public class VTBResponse
    {
        public status[] records { get; set; }
        public int processedRecords { get; set; }
        public int model { get; set; }
        public string softwareProviderId { get; set; }
        public string requestId { get; set; }
        public string providerId { get; set; }
        public string merchantId { get; set; }
        public string signature { get; set; }
    }

    public class VTBService : IVTBService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IVTBService> _logger;
        private readonly IYeuCauChiTienRepository _repo;
        private readonly IChungTuRepository _ctrepo;
        private readonly IHoSoThanhToanRepository _hsttrepo;
        private readonly IUtilsService _utilsservice;

        public VTBService(IYeuCauChiTienRepository repo, IChungTuRepository ctrepo, IHoSoThanhToanRepository hsttrepo, IConfiguration configuration, ILogger<IVTBService> logger, IUtilsService utilsservice)
        {
            _repo = repo;
            _ctrepo = ctrepo;
            _hsttrepo = hsttrepo;
            _configuration = configuration;
            _logger = logger;
            _utilsservice = utilsservice;
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
                    Response signResp = _utilsservice.GenerateBankSignature(original_data);
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

                        if (bankResponseString == null || bankResponseString == "")
                        {
                            _repo.UpdateYeuCauChiTien(new YeuCauChiTienParams
                            {
                                ChungTuId = ycct.ChungTuId,
                                YeuCauChiTienId = requestID,
                                TrangThaiChi = TrangThaiChiTien.NganHangXuLyLoi,
                                ChuKy = signedString,
                                MaKetQuaChi = ""
                            });
                            return new Response(message: "Không nhận được response chuyển tiền của ngân hàng", data: "", errorcode: "107", success: false);
                        }

                        //dynamic bankResponseJson = JToken.Parse(bankResponseString);
                        VTBResponse bankResponseJson = JsonConvert.DeserializeObject<VTBResponse>(bankResponseString);

                        int bankResponseCode = bankResponseJson.records[0].code;
                        string bankResponseMsg = bankResponseJson.records[0].message;

                        int bankResponseProcessedRecords = bankResponseJson.processedRecords;
                        string bankResponseRequestId = bankResponseJson.requestId;
                        string bankResponseProviderId = bankResponseJson.providerId;
                        string bankResponseMerchantId = bankResponseJson.merchantId;
                        string signData = bankResponseRequestId + bankResponseProviderId + bankResponseMerchantId + bankResponseCode.ToString() + bankResponseProcessedRecords;
                        string signature = bankResponseJson.signature;
                        Response verifyResp = _utilsservice.VerifyBankSignature(signData, signature, chungtuItem.TenNganHangChuyen);
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
    }
}
