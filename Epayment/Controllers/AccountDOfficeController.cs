

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Epayment.Controllers
{
    public class AccountDOfficeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountDofficeService _Service;
        private IMemoryCache _cache;
        public AccountDOfficeController(IConfiguration configuration, IAccountDofficeService AccountDofficeService, IMemoryCache cache)
        {
            _configuration = configuration;
            _Service = AccountDofficeService;
            _cache = cache;
        }

        [HttpGet("api/GetTokenDoffice")]
        public ActionResult<Response> GetTokenDoffice()
        {
            object objtoken = _cache.Get("CACHE_TOKEN_DOFFICE");
            if (objtoken != null)
            {
                Response resToken = (Response)objtoken;             
                var jwthandler = new JwtSecurityTokenHandler();
                Dictionary<string, object> list = (Dictionary<string, object>)resToken.Data;                
                var jwttoken = jwthandler.ReadToken(list["accessToken"].ToString());
                var expDate = jwttoken.ValidTo;
                if (expDate > DateTime.UtcNow.AddMinutes(1))
                    return (Response)objtoken;
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
                        Dictionary<string, object> payload = new Dictionary<string, object> {
                        { "accessToken" , result.Data.accessToken.Value },
                        { "refreshToken",result.Data.refreshToken.Value}
                        };
                        Response resToken = new Response(message: "", data: payload, errorcode: "00", success: true);
                        _cache.Set("CACHE_TOKEN_DOFFICE", resToken);
                        return resToken;
                    }
                    else
                    {
                        return new Response(message: result.Message.Value, data: "", errorcode: "003", success: true);
                    }
                }
            }
            else
            {
                return new Response(message: "Không lấy được token D-Office", data: "", errorcode: "002", success: false);
            }
            return new Response(message: "", data: "", errorcode: "", success: false);
        }
        [HttpGet("api/RefreshTokenDoffice/{RefreshTokenCode}")]
        public ActionResult<Response> RefreshTokenDoffice(string RefreshTokenCode)
        {
            Dictionary<string, object> request = new Dictionary<string, object> {
                    { "refreshToken" , RefreshTokenCode },
                    { "deviceId",_configuration["D-Office:DeviceInfo:DeviceId"]}
            };

            HttpClient client = new HttpClient();
            var resultResponse = client.PostAsync(
                _configuration["D-Office:AddressToken"] + "/auth/Auth/refresh",
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
                        return new Response(message: "", data: payload, errorcode: "", success: true);
                    }
                }
                else
                {
                    return new Response(message: result.Error.M.Value, data: result.Message.Value, errorcode: "003", success: false);
                }
            }
            else
            {
                return new Response(message: "Không lấy được token D-Office", data: "", errorcode: "002", success: false);
            }
            return new Response(message: "", data: "", errorcode: "", success: false);
        }
        [HttpPost("api/GetTokenDoffice")]
        public ActionResult<Response> GetTaiLieu([FromBody] SearchTaiLieuDOffice requet)
        {
            var rep = _Service.GetTaiLieu(requet);
            return rep;
        }
        [HttpPost("api/DownloadTaiLieunDoffice")]
        public ActionResult<Response> DownloadTaiLieu([FromBody] SearchTaiLieuDOffice requet)
        {
            var rep = _Service.DownloadTaiLieu(requet);
            return rep;
        }
    }
}