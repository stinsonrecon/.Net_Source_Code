using System;
using BCXN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BCXN.Controllers
{
    [Route("api/sms")]
    public class SmsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<HomeController> _logger;
        public SmsController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            this.Configuration = configuration;
            this._logger = logger;
        }
        [HttpPost]
        public ActionResult<object> SendSMS([FromBody] ContentSmsViewModel contentSms)
        {
            ResultSmsViewModel output = new ResultSmsViewModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var strApiSmsUrl = Configuration["SMSSetting:ApiSmsUrl"];
                    var strMa_DviQly = Configuration["SMSSetting:MA_DVIQLY"];
                    var strMaYcauSms = Configuration["SMSSetting:MA_YCAU_SMS"];
                    contentSms.MA_YCAU_SMS = strMaYcauSms;
                    contentSms.MA_DVIQLY = strMa_DviQly;
                    client.BaseAddress = new Uri(strApiSmsUrl);
                    var stringContent = new StringContent(JsonConvert.SerializeObject(contentSms), Encoding.UTF8, "application/json");
                    var resultResponse = client.PostAsync(strApiSmsUrl, stringContent);

                    if (resultResponse.Result.Content != null)
                    {
                        var responseContent = resultResponse.Result.Content.ReadAsStringAsync();
                        resultResponse.Wait();                      
                        output = JsonConvert.DeserializeObject<ResultSmsViewModel>(responseContent.Result);
                        return output;
                    }
                }
            }
            catch(Exception ex)
            {
                output.STATUS = "ERR0R";
                output.MESSAGE = ex.Message;
            }
            return output;
        }

        public ActionResult<object> SendOTP([FromBody] ContentSmsViewModel contentSms)
        {
            ResultSmsViewModel output = new ResultSmsViewModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var strApiSmsUrl = Configuration["SMSSetting:ApiSmsUrl"];
                    var strMa_DviQly = Configuration["SMSSetting:MA_DVIQLY"];
                    var strMaYcauSms = Configuration["SMSSetting:MA_YCAU_SMS"];
                    client.BaseAddress = new Uri(strApiSmsUrl);
                    var stringContent = new StringContent(JsonConvert.SerializeObject(contentSms), Encoding.UTF8, "application/json");
                    var resultResponse = client.PostAsync("SMS", stringContent);

                    if (resultResponse.Result.Content != null)
                    {
                        var responseContent = resultResponse.Result.Content.ReadAsStringAsync();
                        resultResponse.Wait();
                        output = JsonConvert.DeserializeObject<ResultSmsViewModel>(responseContent.Result);
                        return output;
                    }
                }
            }
            catch (Exception ex)
            {
                output.STATUS = "ERR0R";
                output.MESSAGE = ex.Message;
            }
            return output;
        }
        public ActionResult<object> VerifyOTP([FromBody] ContentSmsViewModel contentSms)
        {
            ResultSmsViewModel output = new ResultSmsViewModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var strApiSmsUrl = Configuration["SMSSetting:ApiSmsUrl"];
                    var strMa_DviQly = Configuration["SMSSetting:MA_DVIQLY"];
                    var strMaYcauSms = Configuration["SMSSetting:MA_YCAU_SMS"];
                    client.BaseAddress = new Uri(strApiSmsUrl);
                    var stringContent = new StringContent(JsonConvert.SerializeObject(contentSms), Encoding.UTF8, "application/json");
                    var resultResponse = client.PostAsync("SMS", stringContent);

                    if (resultResponse.Result.Content != null)
                    {
                        var responseContent = resultResponse.Result.Content.ReadAsStringAsync();
                        resultResponse.Wait();
                        output = JsonConvert.DeserializeObject<ResultSmsViewModel>(responseContent.Result);
                        return output;
                    }
                }
            }
            catch (Exception ex)
            {
                output.STATUS = "ERR0R";
                output.MESSAGE = ex.Message;
            }
            return output;
        }
    }
}