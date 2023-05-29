using BCXN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Services;
using BCXN.Statics;
using BCXN.Models;
using Microsoft.AspNetCore.Authorization;
using Epayment.Services;
using Epayment.Models;
using Epayment.ViewModels;
using Epayment.ModelRequest;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Spire;
using Spire.Doc;
using Microsoft.AspNetCore.StaticFiles;

namespace Epayment.Controllers
{
    [Route("api/HoSoTT")]
    [Authorize]
    public class HoSoThanhToanController : Controller
    {
        private readonly IHoSoThanhToanService _service;

        public HoSoThanhToanController(IHoSoThanhToanService service)
        {
            _service = service;
        }
        [HttpPost("paging")]
        public ActionResult<ResponseHoSoViewModel> getHoSoThanhToanPaging([FromBody] HoSoSearchViewModel request)
        {
            var resp = _service.GetHoSoPaging(request);
            return Ok(resp);
        }
        [HttpGet("{id}")]
        public ActionResult<ResponseHoSoViewModel> getHoSoThanhToanById(string id)
        {
            var resp = _service.GetHoSoTTById(id);
            return Ok(resp);
        }
        [HttpPost]
        public async Task<ActionResult<ResponsePostHSTTViewModel>> CreateGiayTo([FromBody] ParmHoSoThanhToanViewModel request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ParmHoSoThanhToanViewModel parm = new ParmHoSoThanhToanViewModel();
            parm = request;
            parm.IdLogin = userId;
            var resPost = await _service.CreateHoSoTT(request);
            return resPost;
        }
        [HttpPut]
        public async Task<ActionResult<ResponsePostViewModel>> UpdateHoSoTT([FromBody] ParmHoSoThanhToanViewModel request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ParmHoSoThanhToanViewModel parm = new ParmHoSoThanhToanViewModel();
            parm = request;
            parm.IdLogin = userId;
            var resPost = await _service.UpdateHoSoTT(request);
            return resPost;
        }
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteHoSoTT(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resPost = _service.DeleteHoSoTT(id, userId);
            return resPost;
        }
        [HttpPut("chuyen-trang-thai")]
        public ActionResult<ResponsePostViewModel> ApproveHoSoTT([FromBody] ApproveHoSoTT request)
        {
            var resPost = _service.ApproveHoSo(request);
            return resPost;
        }
        [HttpGet("Pd-Ho-So-TT/{id}")]
        public ActionResult<ResponsePheDuyetHoSoTTViewModel> getPDHoSoThanhToanById(string id)
        {
            var resp = _service.GetPheDuyetHoSoTTById(id);
            return Ok(resp);
        }
        [HttpPut("updateFileHoSoTT")]
        public async Task<ActionResult<ResponsePostViewModel>> UpdateFileHoSoTT([FromForm] CreateFileHoSoTT request)
        {
            var resPost = await _service.UpdateFileHoSoTT(request);
            return resPost;
        }

        [HttpGet("convertToBase64")]
        public ActionResult<Response> ConvertToBase64(string filepath)
        {
            if (System.IO.File.Exists(filepath) == false) return StatusCode(400, new Response(message: "Không tìm thấy file", data: "", errorcode: "001", success: false));
            else return StatusCode(200, new Response(message: "", data: Convert.ToBase64String(System.IO.File.ReadAllBytes(filepath)), errorcode: "", success: true));
        }

        [HttpPost("convertWordToPDF")]
        public async Task<ActionResult<Response>> ConvertWordToPDF([FromForm] IFormFile fileWord)
        {
            string fileExtension = System.IO.Path.GetExtension(fileWord.FileName);
            if (fileExtension == ".doc" || fileExtension == ".docx")
            {
                var fileName = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + DateTime.Now.Millisecond + fileWord.FileName.ToString().Replace(" ", "-");
                var folderName = Path.Combine("wwwroot", "fileConvertPdf");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName.Replace(",", "-"));
                var stream = new FileStream(pathToSave, FileMode.Create);
                await fileWord.CopyToAsync(stream);
                stream.Dispose();
                var pathFile = pathToSave.ToString().Split("wwwroot");
                var UrlFile = "wwwroot" + pathFile[pathFile.Length - 1];
                Document doc = new Document();
                doc.LoadFromFile(pathToSave);
                doc.SaveToFile(UrlFile + ".PDF", FileFormat.PDF);
                byte[] bytes = System.IO.File.ReadAllBytes(UrlFile + ".PDF");
                string file = Convert.ToBase64String(bytes);
                string[] fileList = Directory.GetFiles(@"wwwroot\fileConvertPdf");
                foreach (var item in fileList)
                {
                    if (System.IO.File.Exists(item))
                    {
                        System.IO.File.Delete(item);
                    }
                }
                return new Response(message: "", data: file, errorcode: "", success: true);
            }
            else
            {
                return new Response(message: "lỗi: file không phải định dạng .doc và .docx!", data: "", errorcode: "", success: false);
            }

        }
        [HttpGet("convertWordToPDFEpayment")]
        public async Task<ActionResult<Response>> ConvertWordToPDFEpayment([FromQuery] string pathFileWord)
        {
            string fileExtension = System.IO.Path.GetExtension(pathFileWord);
            if (fileExtension == ".doc" || fileExtension == ".docx")
            {
                Document doc = new Document();
                doc.LoadFromFile(pathFileWord);
                doc.SaveToFile(@"wwwroot\fileConvertPdf\fileconvert.PDF", FileFormat.PDF);
                byte[] bytes = System.IO.File.ReadAllBytes(@"wwwroot\fileConvertPdf\fileconvert.PDF");
                string file = Convert.ToBase64String(bytes);
                string[] fileList = Directory.GetFiles(@"wwwroot\fileConvertPdf");
                foreach (var item in fileList)
                {
                    if (System.IO.File.Exists(item))
                    {
                        System.IO.File.Delete(item);
                    }
                }
                return new Response(message: "", data: file, errorcode: "", success: true);
            }
            else
            {
                return new Response(message: "lỗi: file không phải định dạng .doc và .docx!", data: "", errorcode: "", success: false);
            }

        }
        [HttpGet("DownLoadFilesEpayment")]
        public async Task<ActionResult> DownloadFile([FromQuery]string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }
        [HttpPost("HoSoThamChieuPaging")]
        public ActionResult<ResponsePaging> GetAllHoSoThamChieu([FromBody] HoSoThamChieuSearchViewModel request)
        {
            var resp = _service.GetAllHoSoThamChieu(request);
            return resp;
        }

        [HttpPost("PhanCongXuLy")]
        public ActionResult<ResponsePostViewModel> PhanCongXuLy([FromBody] ModelRequest.PhanCongXuLy request)
        {
            var resp = _service.PhanCongXuLy(request);
            return resp;
        }
    }
}