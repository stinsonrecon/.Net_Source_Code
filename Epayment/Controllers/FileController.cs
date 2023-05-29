using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Services;
using BCXN.Statics;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using Newtonsoft.Json;

namespace BCXN.Controllers
{
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly ITableauService _service;
        //public FileController(IVatTuService service)
        private readonly IConfiguration Configuration;
        private readonly ILogger<HomeController> _logger;
        public FileController(IConfiguration configuration, ILogger<HomeController> logger, ITableauService service)
        {
            _service = service;
            this.Configuration = configuration;
            this._logger = logger;
        }
        // [Authorize(Roles = Policies.TCT + "," + Policies.NCC+ "," + Policies.DVTD + "," + Policies.ADMIN)]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                _logger.LogError("INFO: File", file);
                var folderName = Path.Combine("wwwroot", "fileTableAu");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var name = "";
                    for (int i = 0; i < fileName.Split('.').Count(); i++)
                    {
                        if (i < fileName.Split('.').Count() - 1)
                        {
                            name += fileName.Split('.')[i];
                        }
                    }
                    string renameFile = name + "@@@" + Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
                    var fullPath = Path.Combine(pathToSave, renameFile);
                    var dbPath = Path.Combine(folderName, renameFile);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    string urlImageConverted = _service.ConvertPPTXToImage(renameFile);
                    _logger.LogError("INFO: File path", dbPath);
                    return Ok(new { dbPath = urlImageConverted });
                }
                else
                {
                    _logger.LogError("INFO: Khong co file");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR: ", ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //[HttpGet("{siteId}/{viewname}")]
        //public ResponseFile GetViewTableau(string siteId, string viewname)
        //{
        //    var TABLEAU_SERVER = Configuration["TABLEAUSetting:TABLEAU_SERVER"];
        //    var TABLEAU_USER = Configuration["TABLEAUSetting:TABLEAU_USER"];
        //    var TABLEAU_PASS = Configuration["TABLEAUSetting:TABLEAU_PASS"];
        //    // this.SetConnectionDB();
        //    string tokenUser = "";//"LSVJ4eJPQq-vPxUgSozVGA|2xYOx4hCfpYgxJvrsQdXx0yS4xrQao63";

        //    InputDataSignIn inputPostData = new InputDataSignIn();
        //    Site site = new Site();
        //    site.contentUrl = "GIAOBAN_EVN";
        //    Credentials credentials = new Credentials();
        //    credentials.name = TABLEAU_USER;
        //    credentials.password = TABLEAU_PASS;
        //    credentials.site = site;
        //    inputPostData.credentials = credentials;
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(TABLEAU_SERVER + "api/3.7/auth/signin");

        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var stringPostDataJson = JsonConvert.SerializeObject(inputPostData);
        //        var httpContent = new StringContent(stringPostDataJson, Encoding.UTF8, "application/json");
        //        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //        var httpResponse = client.PostAsync(TABLEAU_SERVER + "api/3.7/auth/signin", httpContent);
        //        httpResponse.Wait();
        //        if (httpResponse.Result.Content != null)
        //        {
        //            var responseContent = httpResponse.Result.Content.ReadAsStringAsync();
        //            httpResponse.Wait();

        //            OutputDataSignIn output = new OutputDataSignIn();
        //            output = JsonConvert.DeserializeObject<OutputDataSignIn>(responseContent.Result);
        //            if (output != null && output.credentials != null && output.credentials.token != null)
        //            {
        //                tokenUser = output.credentials.token;

        //            }

        //        }
        //    }
        //    string viewId = "";
        //    using (var clientDownload = new HttpClient())
        //    {

        //        clientDownload.DefaultRequestHeaders.Add("X-Tableau-Auth", tokenUser);
        //        //clientDownload.DefaultRequestHeaders.Add("X-Tableau-Auth", "ITwprjWsR-GBXTKS2Y_26g|0jhEeo16Uf4Rt2bHUPFOBow7eg2Sf5JJ");
        //        //https://bcqt.evn.com.vn/api/3.7/sites/6836caad-f827-4246-8356-db6763390eb9/views?filter=viewUrlName:eq:2_2_KhuphtinA0-GitrnvSMP

        //        string apiGetViewUrl = TABLEAU_SERVER + "api/3.7/sites/" + siteId + "/views?filter=viewUrlName:eq:" + viewname;

        //        HttpResponseMessage response = clientDownload.GetAsync(apiGetViewUrl).Result;


        //        if (response.IsSuccessStatusCode)
        //        {
        //            string path = @"wwwroot\fileTableAu";
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }

        //            System.IO.DirectoryInfo di = new DirectoryInfo(path);

        //            foreach (FileInfo file in di.GetFiles())
        //            {
        //                file.Delete();
        //            }

        //            string fileNameXML = path + "\\" + DateTime.Now.ToString("YYMMDDhhMMss") + "_temp.xml";
        //            byte[] fileBytes = response.Content.ReadAsByteArrayAsync().Result;

        //            System.IO.File.WriteAllBytes(fileNameXML, fileBytes);
        //            var allXMLFiles = di.GetFiles("*.xml", SearchOption.AllDirectories).ToList();
        //            if (allXMLFiles != null && allXMLFiles.Count > 0)
        //            {
        //                var xmlFile = allXMLFiles[0];
        //                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
        //                xmlDocument.Load(xmlFile.FullName);
        //                System.Xml.XmlNodeList empNodes = xmlDocument.GetElementsByTagName("view");
        //                foreach (System.Xml.XmlNode chldNode in empNodes)
        //                {
        //                    viewId = chldNode.Attributes["id"].Value;
        //                    using (var clientDownload2 = new HttpClient())
        //                    {
        //                        string apiGetFile = TABLEAU_SERVER + "api/3.7/sites/" + siteId + "/views/" + viewId + "/pdf";
        //                        clientDownload2.DefaultRequestHeaders.Add("X-Tableau-Auth", tokenUser);
        //                        HttpResponseMessage responseGet = clientDownload2.GetAsync(apiGetFile).Result;
        //                        if (responseGet.IsSuccessStatusCode)
        //                        {
        //                            //string path1 = Path.Combine("wwwroot", "fileTableAu");
        //                            string path1 = @"wwwroot\fileTableAu";
        //                            if (!Directory.Exists(path1))
        //                            {
        //                                Directory.CreateDirectory(path1);
        //                            }
        //                            string fileNamePdf;
        //                            fileNamePdf = viewId + "_" + DateTime.Now.ToString("YYMMDDhhMMss") + ".pdf";

        //                            if (System.IO.File.Exists(path1 + fileNamePdf))
        //                            {
        //                                try
        //                                {
        //                                    System.IO.File.Delete(path1 + fileNamePdf);
        //                                }
        //                                catch (System.IO.IOException e)
        //                                {
        //                                    Console.WriteLine(e.Message);

        //                                }
        //                            }
        //                            string pathFile;
        //                            pathFile = path1 + "\\" + fileNamePdf;
        //                            byte[] fileBytes2 = responseGet.Content.ReadAsByteArrayAsync().Result;
        //                            System.IO.File.WriteAllBytes(pathFile, fileBytes2);
        //                            //}
        //                            //else
        //                            //{
        //                            //}
        //                            //return new ResponseFile(pathFile);
        //                            return new ResponseFile(responseGet.Content.ReadAsByteArrayAsync().Result);

        //                        }

        //                    }
        //                }
        //            }

        //            //string value = string.Empty;
        //            //value = JsonConvert.SerializeObject(reportSelected, Formatting.Indented, new JsonSerializerSettings
        //            //{
        //            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //            //});
        //        }
        //        return new ResponseFile(response.Content.ReadAsByteArrayAsync().Result);

        //    }
        //}
    }
}

public class ResponseViewId
{
    public string viewId { get; set; }
    public ResponseViewId( string id)
    {
        viewId = id;
    }
}