using BCXN.Models;
using BCXN.Services;
using BCXN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace BCXN.Controllers
{
    [Route("api/Tableau")]
    public class TableauController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private ITableauService _service;
        public TableauController(ILogger<HomeController> logger, IConfiguration configuration, ITableauService service)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetToken")]
        public ActionResult<OutputDataSignIn> GetTokenTableau(SiteInfo siteInfo)
        {
            return this._service.GetTokenTableau(siteInfo.sitename);

        }
        [HttpGet("Download/{siteId}/{viewId}/{token}")]
        public ActionResult<object> DownloadTableau(string siteId, string viewId, string token)
        {
            string filename = this._service.DownloadTableau(siteId, viewId, token);
            return new { filename };

        }
        [HttpGet("GetView/{siteId}/{viewname}/{token}")]
        public ActionResult<object> GetViewTableau(string siteId, string viewname, string token)
        {
            return this._service.GetView(siteId, viewname, token);
        }
        [HttpGet("GetTicket")]
        public ActionResult<object> GetTicketTableau(SiteInfo siteInfo)
        {
            return this._service.GetTicketTableau(siteInfo.sitename);
        }

        [HttpGet("ConvertToPPTX")]
        public ActionResult<object> ConvertImageToPPTX(string fileImg, string templatePPTX)
        {
            return this._service.ConvertImageToPPTX(fileImg, templatePPTX);
        }

        [HttpPost("MergeSlideByID")]
        public ActionResult<object> MergeSlideByID([FromBody] SourcePresentationsByID sourcePresentationsByID)
        {
            string[] normalizedSlide = this._service.NormalizeSlidesByID(sourcePresentationsByID.datafile, sourcePresentationsByID.ycbcId, sourcePresentationsByID.DataID);
            var resp = this._service.MergeSlide(normalizedSlide, sourcePresentationsByID.ycbcId);
            foreach (string item in normalizedSlide)
            {
                if (System.IO.File.Exists(Path.Combine(@"wwwroot\fileTableAu", item)) && item != "template.pptx")
                    System.IO.File.Delete(Path.Combine(@"wwwroot\fileTableAu", item));
            }
            return resp;
        }

        [HttpGet("ConvertToImage")]
        public ActionResult<object> ConvertPPTXToImage(string filenamePPTX)
        {
            return this._service.ConvertPPTXToImage(filenamePPTX);
        }
        [HttpGet("DownloadAutoBaoCaoTableau")]
        public ActionResult<object> DownloadAutoBaoCaoTableau()
        {
            this._service.DownloadAutoBaoCaoTableau();
            return true;
        }

    }

    public class SiteInfo
    {
        public string sitename { get; set; }
    }

    public class SourcePresentations
    {
        public string[] Data { get; set; }

        public int ycbcId { get; set; }
    }

    public class SourcePresentationsByID
    {
        public int[] DataID { get; set; }
        public int ycbcId { get; set; }
        public string[] datafile { get; set; }
    }
}