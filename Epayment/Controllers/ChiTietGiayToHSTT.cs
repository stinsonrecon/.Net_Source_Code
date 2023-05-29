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
using System.IO;
using BCXN.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Epayment.Controllers
{
    [Route("api/ChitietgiaytoHSTT")]
    [Authorize]

    public class ChiTietGiayToHSTT : Controller
    {
        private readonly IChiTietGiayToHSTTService _service;
        private readonly ApplicationDbContext _context;
        private readonly IUploadToFtpService _IUploadToFtpService;

        public ChiTietGiayToHSTT(IChiTietGiayToHSTTService service, ApplicationDbContext context, IUploadToFtpService IUploadToFtpService)
        {
            _service = service;
            _context = context;
            _IUploadToFtpService = IUploadToFtpService;
        }
        [HttpPost]
        public async Task<ActionResult<ResponsePostHSTTViewModel>> CreateChiTietGiayToHSTT([FromForm] ParmChiTietGiayToHSTTViewModel request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<string> lfile = new List<string>();
                ParmChiTietGiayToHSTTViewModel parm = new ParmChiTietGiayToHSTTViewModel();
                parm = request;
                parm.NguoiCapNhatId = userId.ToString();
                var resPost = _service.CreateChiTietGiayToHSTT(parm, lfile);
                return await resPost;
            }
            catch (Exception ex)
            {
                return new ResponsePostHSTTViewModel(ex.ToString(), 400 , "");
            }
        }
        [HttpPut]
        public async Task<ActionResult<ResponsePostViewModel>> UpdateChiTietGiayToHSTT([FromForm] ParmChiTietGiayToHSTTViewModel request)
        {
            try
            {
                List<string> lfile = new List<string>();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //ParmChiTietGiayToHSTTViewModel parm = new ParmChiTietGiayToHSTTViewModel();
                //parm = request;
                request.NguoiCapNhatId = userId.ToString();
                var resPost = _service.UpdateChiTietGiayToHSTT(request, lfile);
                return await resPost;
            }
            catch (Exception ex)
            {
                return new ResponsePostViewModel(ex.ToString(), 400);
            }
        }
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteChiTietGiayToHSTT(Guid id)
        {
            var resPost = _service.DeleteChiTietGiayToHSTT(id);
            return resPost;
        }
        [HttpGet("paging")]
        public ActionResult<ResponseHoSoViewModel> getHoSoThanhToanPaging([FromQuery] ChiTietHSTTSearchViewModel request)
        {
            var resp = _service.GetChiTietGiayToHSTT(request);
            return Ok(resp);
        }
        [HttpDelete("delete-by-hostt/{HoSoTTId}")]
        public ActionResult<ResponsePostViewModel> DeleteChiTietGiayToByHSTT(Guid HoSoTTId)
        {
            var itemCTGT = _context.ChiTietGiayToHSTT.Where(x => x.HoSoThanhToan.HoSoId == HoSoTTId);
            if (itemCTGT != null)
            {
                foreach (var fileDinhKem in itemCTGT)
                {
                    var itemFile = fileDinhKem.ToString().Split(",");
                    foreach (var f in itemFile )
                    if (System.IO.File.Exists(f))
                    {
                        System.IO.File.Delete(f);
                    }
                }
            }
            var resPost = _service.DeleteChiTietGiayToByIdHSTT(HoSoTTId);
            return resPost;
        }
        [HttpPost("PhieuThamTra")]
        public async Task<ActionResult<Response>> CreatePhieuThamTraHSTT([FromForm] ParmPhieuThamTraHSTTViewModel request)
        {
            try
            {
                var resPost = _service.CreatePhieuThamTraHSTT(request, null);
                return await resPost;
            }
            catch (Exception ex)
            {
                return new Response(message:"Lỗi đẩy file", data:null, errorcode: "02", success: false);
            }
        }

        [HttpPost("updateFile")]
        public async Task<ActionResult<string>> UpdateFileSftp([FromForm] IFormFile request)
        {
            var resPost = await _IUploadToFtpService.UploadFileToSftpServer(request, "Epayment-Test");
            return resPost;
        }
        [HttpGet("DownloadFile")]
        public async Task<FileStream> downloadFileHoSoTT([FromQuery] string request)
        {
            var resPost =  _IUploadToFtpService.DownloadFileToSftpServer(request);
            return resPost;
        }
        [HttpPost("updateFileInFtp")]
        public async Task<ActionResult<string>> UpdateFileFtp([FromForm] IFormFile request)
        {
            var resPost = await _IUploadToFtpService.UploadFileToFtpServer(request, "Epayment-Test");
            return resPost;
        }

        [HttpGet("DownloadFileFtp")]
        public ActionResult<Response> downloadFileFtp([FromQuery] string request)
        {
            var resPost = _IUploadToFtpService.DownloadFileFtpServerConvertBase64(request);
            return resPost;
        }

        [HttpGet("DownloadFileSFtp")]
        public ActionResult<Response> downloadFileSFtp1([FromQuery] string request)
        {
            var resPost = _IUploadToFtpService.DownloadFileToSftpServerConvertBase64(request);
            return resPost;
        }
    }
}