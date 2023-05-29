using Microsoft.Extensions.Configuration;
using BCXN.ViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using BCXN.Data;

namespace Epayment.Services
{
    public class UserReceiveEmail
    {
        public string Email { get; set; }
        public string FullName { get; set; }
    }

    public class status
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public interface IUtilsService
    {
        Response VerifyBankSignature(string original_data, string signature, string bankcode);
        Response GenerateBankSignature(string original_data);
        Response SendMail(string[] toEmail, string[] cc, string subject, string body);
        /// <summary>
        /// Gửi mail cho người ký ở bước tiếp theo, nếu thao tác được cấu hình không gửi mail thì sẽ không gửi mail
        /// </summary>
        /// <param name="thaoTacBuocPheDuyetId">thao tác thực hiện</param>
        /// <param name="hoSoThanhToanId">hồ sơ thanh toán</param>
        /// <returns></returns>
        void SendMailKy(Guid thaoTacBuocPheDuyetId, Guid hoSoThanhToanId);
        /// <summary>
        /// Gửi mail thông báo hủy cho người ở bước 1 và cc người ở các bước trước
        /// </summary>
        /// <param name="thaoTacBuocPheDuyetId">thao tác thực hiện</param>
        /// <param name="hoSoThanhToanId">hồ sơ thanh toán</param>
        /// <returns></returns>
        void SendMailHuy(Guid thaoTacBuocPheDuyetId, Guid hoSoThanhToanId);
    }

    public class UtilsService : IUtilsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UtilsService> _logger;
        private readonly ApplicationDbContext _context;

        public UtilsService(IConfiguration configuration, ILogger<UtilsService> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public Response SendMail(string[] toEmail, string[] cc, string subject, string body)
        {
            try
            {
                _logger.LogWarning($"[SendMail] to:{string.Join(",", toEmail)}, cc:{string.Join(",", cc)}, subject:{subject}, body: {body}");
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                service.Url = new Uri(_configuration["EmailSetting:serviceUri"]);
                service.UseDefaultCredentials = false;
                service.Credentials = new WebCredentials(_configuration["EmailSetting:fromAddress"], _configuration["EmailSetting:password"], _configuration["EmailSetting:domain"]);
                EmailMessage message = new EmailMessage(service);
                message.Subject = subject;
                message.Body = body;
                if(toEmail != null && toEmail.Length > 0){
                    foreach (string item in toEmail)
                    {
                        message.ToRecipients.Add(item.Trim());
                    }
                }
                if (cc != null && cc.Length > 0)
                    foreach (string item in cc) message.CcRecipients.Add(item.Trim());
                message.Send();
                return new Response(message: "Gửi email thành công", data: "", errorcode: "00", success: true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[SendMail] Lỗi khi gửi mail: {toEmail} {subject} {ex.Message} {ex.StackTrace}");
                return new Response(message: "Lỗi: Chưa xác định cho hàm gửi mail", data: ex.Message, errorcode: "01", success: false);
            }
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

        public void SendMailKy(Guid thaoTacBuocPheDuyetId, Guid hoSoThanhToanId)
        {
            var hoSoThanhToan = _context.HoSoThanhToan.FirstOrDefault(t => t.HoSoId == hoSoThanhToanId);
            var loaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoThanhToan.LoaiHoSoId);
            var donVi = _context.DonVi.FirstOrDefault(x => x.Id == hoSoThanhToan.DonViYeuCauId);
            if (hoSoThanhToan == null)
            {
                throw new Exception($"Không tìm thấy hồ sơ thanh toán có Id = {hoSoThanhToanId}");
            }

            var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet.FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == thaoTacBuocPheDuyetId);
            if (thaoTacBuocPheDuyet == null)
            {
                throw new Exception($"Không tìm thấy thao tác bước phê duyệt có Id = {thaoTacBuocPheDuyetId}");
            }

            //không gửi mail
            if (!thaoTacBuocPheDuyet.IsSendMail)
            {
                return;
            }

            var buocPheDuyet = _context.BuocPheDuyet.FirstOrDefault(b => b.BuocPheDuyetId == thaoTacBuocPheDuyet.BuocPheDuyetId && !b.DaXoa);
            if (buocPheDuyet == null)
            {
                throw new Exception($"Không tìm thấy bước phê duyệt có Id = {thaoTacBuocPheDuyet.BuocPheDuyetId}");
            }

            var buocPheDuyetTiepTheo = _context.BuocPheDuyet.FirstOrDefault(b => b.QuyTrinhPheDuyetId == buocPheDuyet.QuyTrinhPheDuyetId 
                && b.ThuTu == buocPheDuyet.ThuTu + 1 && !b.DaXoa);
            if (buocPheDuyetTiepTheo == null)
            {
                throw new Exception($"Không tìm thấy bước phê duyệt tiếp theo của bước phê duyệt có Id = {buocPheDuyet.BuocPheDuyetId}");
            }

            var dsNguoiThucHien = new List<UserReceiveEmail>();
            var dsNguoiThucHienId = buocPheDuyetTiepTheo.NguoiThucHien.Split(",");
            foreach (var userId in dsNguoiThucHienId)
            {
                var user = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    throw new Exception($"Không tìm thấy user có Id = {userId}");
                }

                dsNguoiThucHien.Add(new UserReceiveEmail
                {
                    Email = user.Email,
                    FullName = user.FirstName + " " + user.LastName,
                });
            }

            string subject = "EPAYMENT: Thông báo có Hồ sơ cần được xử lý";

            string body = @"Kính gửi: Anh/Chị " + string.Join(",", dsNguoiThucHien.Select(n => n.FullName)) + @"<br/>Hệ thống EPAYMENT có một hồ sơ thanh toán đang trình ký như sau:";
            body += @"<ul><li>Nội dung trình duyệt: " + hoSoThanhToan.NoiDungTT + @"</li><li>Bộ phận trình duyệt: "
                + donVi.TenDonVi + @"</li><li>Tên hồ sơ: " 
                + hoSoThanhToan.TenHoSo + @"</li><li>Loại hồ sơ: " 
                + loaiHoSo.TenLoaiHoSo + @"</li><li>Mã hồ sơ: " 
                + hoSoThanhToan.MaHoSo + @"</li></ul>Để xem chi tiết Hồ sơ thanh toán đang trình, đề nghị Ông/Bà vui lòng click <a href='http://uat-epayment.evn.com.vn/'>vào đây</a>.</br>Trân trọng.";

            string[] cc = _configuration.GetSection("Notification:DefaultCC").Get<string[]>();
            var resSendEmail = SendMail(toEmail: dsNguoiThucHien.Select(n => n.Email).ToArray(), cc: cc, subject: subject, body: body);
            // if (!resSendEmail.Success)
            // {
            //     throw new Exception("Lỗi send email: " + resSendEmail.Message);
            // }
        }

        public void SendMailHuy(Guid thaoTacBuocPheDuyetId, Guid hoSoThanhToanId)
        {
            var hoSoThanhToan = _context.HoSoThanhToan.FirstOrDefault(t => t.HoSoId == hoSoThanhToanId);
            var loaiHoSo = _context.LoaiHoSo.FirstOrDefault(x => x.LoaiHoSoId == hoSoThanhToan.LoaiHoSoId);
            var donVi = _context.DonVi.FirstOrDefault(x => x.Id == hoSoThanhToan.DonViYeuCauId);
            if (hoSoThanhToan == null)
            {
                throw new Exception($"Không tìm thấy hồ sơ thanh toán có Id = {hoSoThanhToanId}");
            }

            var thaoTacBuocPheDuyet = _context.ThaoTacBuocPheDuyet
                        .FirstOrDefault(t => t.ThaoTacBuocPheDuyetId == thaoTacBuocPheDuyetId);
            if (thaoTacBuocPheDuyet == null)
            {
                throw new Exception($"Không tìm thấy thao tác bước phê duyệt Id = {thaoTacBuocPheDuyetId}");
            }

            var buocPheDuyetHienTai = _context.BuocPheDuyet.FirstOrDefault(b => b.BuocPheDuyetId == thaoTacBuocPheDuyet.BuocPheDuyetId);
            if (buocPheDuyetHienTai == null)
            {
                throw new Exception($"Không tìm thấy bước phê duyệt Id = {thaoTacBuocPheDuyet.BuocPheDuyetId}");
            }

            //quay về bước 1
            var buocPheDuyet1 = _context.BuocPheDuyet.FirstOrDefault(b => b.QuyTrinhPheDuyetId == buocPheDuyetHienTai.QuyTrinhPheDuyetId && b.ThuTu == 1 && !b.DaXoa);
            if (buocPheDuyet1 == null)
            {
                throw new Exception($"Không tìm thấy bước phê duyệt đầu tiên của quy trình phê duyệt Id = {buocPheDuyetHienTai.QuyTrinhPheDuyetId}");
            }

            // gửi mail thông báo cho người thực hiện bước 1 biết có hồ sơ được yêu cầu thay đổi
            var dsNguoiThucHien = new List<UserReceiveEmail>();
            var dsNguoiThucHienId = buocPheDuyet1.NguoiThucHien.Split(",");
            foreach (var userId in dsNguoiThucHienId)
            {
                var user = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    throw new Exception($"Không tìm thấy user có Id = {userId}");
                }

                dsNguoiThucHien.Add(new UserReceiveEmail
                {
                    Email = user.Email,
                    FullName = user.FirstName + " " + user.LastName,
                });
            }
            string subject = "EPAYMENT: Thông báo có Hồ sơ thanh toán được yêu cầu thay đổi";

            string body = @"Kính gửi: Anh/Chị " + string.Join(",", dsNguoiThucHien.Select(n => n.FullName)) + @"<br/>Hệ thống EPAYMENT có một hồ sơ thanh toán được yêu cầu thay đổi:"
                + @"<ul><li>Nội dung trình duyệt: " + hoSoThanhToan.NoiDungTT + @"</li><li>Bộ phận trình duyệt: " + donVi.TenDonVi
                + @"</li><li>Tên hồ sơ: " + hoSoThanhToan.TenHoSo + @"</li><li>Loại hồ sơ: " 
                + loaiHoSo.TenLoaiHoSo + @"</li><li>Mã hồ sơ: " + hoSoThanhToan.MaHoSo 
                + @"</li></ul>Để xem chi tiết Hồ sơ thanh toán đang trình, đề nghị Ông/Bà vui lòng click <a href='http://uat-epayment.evn.com.vn/'>vào đây</a>.</br>Trân trọng.";

            //list email cc
            var dsCacBuocTruoc = _context.BuocPheDuyet.Where(b => b.QuyTrinhPheDuyetId == buocPheDuyetHienTai.QuyTrinhPheDuyetId
                && b.ThuTu < buocPheDuyetHienTai.ThuTu && b.ThuTu > 1 && !b.DaXoa).ToList();

            var listEmailCC = new List<string>();
            foreach (var buoc in dsCacBuocTruoc)
            {
                var listNguoiThucHien = buoc.NguoiThucHien.Split(",");
                foreach (var item in listNguoiThucHien)
                {
                    var userEmail = _context.ApplicationUser.FirstOrDefault(x => (x.Id == item)).Email;
                    listEmailCC.Add(userEmail);
                }
            }

            var resSendEmail = SendMail(toEmail: dsNguoiThucHien.Select(n => n.Email).ToArray(), cc: listEmailCC.ToArray(), subject: subject, body: body);
            // if (!resSendEmail.Success)
            // {
            //     throw new Exception("Lỗi send email: " + resSendEmail.Message);
            // }
        }
    }
}
