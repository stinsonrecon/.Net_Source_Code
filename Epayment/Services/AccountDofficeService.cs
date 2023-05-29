using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using BCXN.Repositories;

namespace Epayment.Services
{
    public interface IAccountDofficeService
    {
        Response GetTaiLieu(SearchTaiLieuDOffice requet);
        Response DownloadTaiLieu(SearchTaiLieuDOffice requet);
    }
    public class AccountDofficeService : IAccountDofficeService
    {
        private readonly IConfiguration _configuration;
        private readonly IDonViRepository _IDonVi;
        private readonly ILogger<AccountDofficeService> _logger;
        private IMemoryCache _cache;

        public AccountDofficeService(IConfiguration configuration, ILogger<AccountDofficeService> logger, IMemoryCache cache, IDonViRepository IDonVi)
        {
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
            _IDonVi = IDonVi;
        }
        public string GetToken()
        {
            try
            {
                string token = "";
                object objtoken = _cache.Get("CACHE_TOKEN_DOFFICE");
                if (objtoken != null)
                {
                    var resToken = objtoken;
                    var jwthandler = new JwtSecurityTokenHandler();
                    Dictionary<string, object> list = (Dictionary<string, object>)resToken;
                    var jwttoken = jwthandler.ReadToken(list["accessToken"].ToString());
                    var expDate = jwttoken.ValidTo;
                    if (expDate > DateTime.UtcNow.AddMinutes(1))
                    {
                        token = list["accessToken"].ToString();
                        return token;
                    }
                    else
                    {
                        var RefreshTokenResult = RefreshTokenDoffice(list["refreshToken"].ToString());
                        if (RefreshTokenResult.Success == true)
                        {
                            token = RefreshTokenResult.Message.ToString();
                            _cache.Set("CACHE_TOKEN_DOFFICE", RefreshTokenResult.Data);
                            return token;
                        }
                    }
                    // if (expDate > DateTime.UtcNow.AddMinutes(1)){
                    //     token = list["accessToken"].ToString();
                    //     return token;
                    // }
                    // else {
                    //     var RefreshTokenResult = RefreshTokenDoffice(list["refreshToken"].ToString());
                    //     if (RefreshTokenResult.Success == true){
                    //         token = RefreshTokenResult.Message.ToString();
                    //         _cache.Set("CACHE_TOKEN_DOFFICE", RefreshTokenResult.Data);
                    //     }
                    // }
                }
                GetTokenDoffice request = new GetTokenDoffice();
                DeviceInfo deviceInfo = new DeviceInfo();
                request.UserName = _configuration["D-Office:UserName"];
                request.Password = _configuration["D-Office:Password"];
                request.DeviceInfo = deviceInfo;
                deviceInfo.DeviceId = _configuration["D-Office:DeviceInfo:DeviceId"];
                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    _configuration["D-Office:AddressUrl"] + "v2/auth/Auth/DAuth",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    var responseContent = resultResponse.Result.Content.ReadAsStringAsync();
                    resultResponse.Wait();
                    dynamic result = JObject.Parse(responseContent.Result);
                    if (result != null)
                    {
                        if (result.Message == 200 && result.State == true)
                        {
                            token = result.Data.accessToken.Value;
                            Dictionary<string, object> payload = new Dictionary<string, object> {
                            { "accessToken" , result.Data.accessToken.Value },
                            { "refreshToken",result.Data.refreshToken.Value}
                            };
                            _cache.Set("CACHE_TOKEN_DOFFICE", payload);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"[GetTokenD-Office] lỗi: Không lấy được token");
                }
                return token;

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[GetTokenD-Office] lỗi: {ex}");
                return null;
            }

        }
        public Response RefreshTokenDoffice(string RefreshTokenCode)
        {
            try
            {
                Dictionary<string, object> request = new Dictionary<string, object> {
                    { "refreshToken" , RefreshTokenCode },
                    { "deviceId",_configuration["D-Office:DeviceInfo:DeviceId"]}
                    };
                HttpClient client = new HttpClient();
                var resultResponse = client.PostAsync(
                    _configuration["D-Office:AddressUrl"] + "v1/auth/Auth/refresh",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                );
                if (resultResponse.Result.Content != null)
                {
                    var responseContent = resultResponse.Result.Content.ReadAsStringAsync();
                    resultResponse.Wait();
                    dynamic result = JObject.Parse(responseContent.Result);
                    if (result != null)
                    {
                        if (result.Message == 200 && result.State == true)
                        {
                            Dictionary<string, object> payload = new Dictionary<string, object> {
                        { "accessToken" , result.Data.accessToken.Value },
                        { "refreshToken",result.Data.refreshToken.Value}
                        };
                            return new Response(message: payload["accessToken"].ToString(), data: payload, errorcode: "00", success: true);
                        }
                    }
                }
                return new Response(message: "Lỗi Call api", data: "", errorcode: "02", success: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[RefreshTokenD-Office] lỗi: {ex}");
                return new Response(message: "Lỗi Call api", data: ex, errorcode: "03", success: false);
            }
        }
        public Response GetTaiLieu(SearchTaiLieuDOffice request)
        {
            try
            {
                if (request.url == null)
                {
                    return new Response(message: "Đường dẫn trống hoặc, sai đường dẫn", data: "", errorcode: "02", success: false);
                }
                if (!string.IsNullOrEmpty(GetToken()))
                {
                    HttpClient client = new HttpClient();
                    List<string> arrValue = new List<string>();
                    var addressUrl = _configuration["D-Office:AddressUrl"];
                    foreach (var item in request.url)
                    {
                        var idDonvi = _IDonVi.GetDonViById(request.idDonvi);
                        string itemUrl = item;
                        if (idDonvi != null)
                        {
                            itemUrl += "&ID_DV=" + idDonvi.DOfficeDonVi;
                        }
                        using (var GetTaiLieu = new HttpRequestMessage(HttpMethod.Get, addressUrl + itemUrl))
                        {
                            GetTaiLieu.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                            var response = client.SendAsync(GetTaiLieu);
                            var DataContent = response.Result.Content.ReadAsStringAsync();
                            response.Wait();
                            //dynamic resultData = JObject.Parse(DataContent.Result);
                            dynamic convert = JsonConvert.DeserializeObject(DataContent.Result);
                            var value = JsonConvert.SerializeObject(convert);
                            arrValue.Add(value);
                            //resultData.Message =="Success" &&
                            // if (resultData.State == true)
                            // {
                            // }
                        }
                    }
                    return new Response(message: "Lấy dữ liệu thành công", data: arrValue, errorcode: "00", success: true);
                }
                else
                {
                    return new Response(message: "Không kết nối được đến D-Office ", data: "", errorcode: "01", success: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[GetTaiLieu-Office] lỗi: {ex}");
                return new Response(message: "Không kết nối được đến D-Office ", data: ex, errorcode: "03", success: false);
            }
        }
        public Response DownloadTaiLieu(SearchTaiLieuDOffice request)
        {
            try
            {
                if (request.url == null)
                {
                    return new Response(message: "Đường dẫn trống hoặc, sai đường dẫn", data: "", errorcode: "02", success: false);
                }
                if (!string.IsNullOrEmpty(GetToken()))
                {
                    var addressUrl = _configuration["D-Office:AddressUrl"];
                    HttpClient client = new HttpClient();
                    List<string> arrValue = new List<string>();
                    foreach (var item in request.url)
                    {
                        var idDonvi = _IDonVi.GetDonViById(request.idDonvi);
                        string itemUrl = item;
                        if (idDonvi != null)
                        {
                            itemUrl += "&ID_DV=" + idDonvi.DOfficeDonVi;
                        }
                        using (var GetTaiLieu = new HttpRequestMessage(HttpMethod.Get, addressUrl + itemUrl))
                        {
                            GetTaiLieu.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
                            var response = client.SendAsync(GetTaiLieu);
                            byte[] fileBytes = response.Result.Content.ReadAsByteArrayAsync().Result;
                            response.Wait();
                            string t = Convert.ToBase64String(fileBytes);
                            arrValue.Add(t);
                        }
                    }
                    return new Response(message: "Lấy dữ liệu thành công", data: arrValue, errorcode: "00", success: true);
                }
                else
                {
                    return new Response(message: "Không kết nối được đến D-Office ", data: "", errorcode: "01", success: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[DownloadTaiLieu-Office] lỗi: {ex}");
                return new Response(message: "Không kết nối được đến D-Office ", data: ex, errorcode: "03", success: false);
            }
        }
    }
}