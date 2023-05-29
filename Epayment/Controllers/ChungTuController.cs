using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;

namespace Epayment.Controllers
{
    [Authorize]
    public class ChungTuController : Controller
    {
        private readonly IChungTuService _service;
        private readonly IGeneratePDFService _generatepdfservice;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUploadToFtpService _uploadService;
        private readonly IChungTuRepository _chungTuRepo;

        public ChungTuController(IChungTuService service, IGeneratePDFService generatepdfservice, UserManager<ApplicationUser> userManager, IUploadToFtpService uploadService, IChungTuRepository chungTuRepository)
        {
            _service = service;
            _generatepdfservice = generatepdfservice;
            _userManager = userManager;
            _uploadService = uploadService;
            _chungTuRepo = chungTuRepository;
        }

        [HttpPost("api/ChungTu/Create")]
        public ActionResult<Response> CreateChungTu([FromBody] ChungTuParams request)
        {
            var resPost = _service.CreateChungTu(request);
            if (resPost.Success == true) return StatusCode(201, resPost);
            else return StatusCode(500, resPost);
        }

        [HttpPut("api/ChungTu/DayChungTuToiERP")]
        public ActionResult<Response> DayChungTuToiERP([FromBody] ChungTuParams request)
        {
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resPost = _service.DayChungTuToiERP(request.ChungTuId, request.UserId );
            if (resPost.Success == true) return StatusCode(200, resPost);
            else return StatusCode(500, resPost);
        }

        [HttpPost("api/ChungTu/CreateChungTuDayErp")]
        public ActionResult<Response> CreateChungTuDayErp([FromBody]ChungTuParams chungtu)
        {
            chungtu.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resPost = _service.CreateChungTuDayErp(chungtu);
            if (resPost.Success == true) return StatusCode(201, resPost);
            else return StatusCode(500, resPost);
        }

        [HttpPost("api/ChungTu/Delete")]
        public ActionResult<Response> DeleteChungTu([FromBody] ChungTuParams request)
        {
            var resPost = _service.DeleteChungTu(request);
            if (resPost.Success == true) return StatusCode(200, resPost);
            else return StatusCode(500, resPost);
        }
        [AllowAnonymous]
        [HttpPost("api/ChungTu/Update")]
        public ActionResult<Response> UpdateChungTu([FromBody] ChungTuParams request)
        {
            //tạm trả tất cả status code 200 cho bên erp
            var resPost = _service.ERPUpdateChungTu(request);
            if (resPost.Success == true) return StatusCode(200, resPost);
            else {
                if (resPost.ErrorCode == "001") return StatusCode(200, resPost);
                else return StatusCode(200, resPost);
            }
        }
        [AllowAnonymous]
        [HttpPost("api/ChungTuGhiSo/Update")]
        public ActionResult<Response> UpdateChungTuGhiSo([FromBody] ChungTuParams request)
        {
            //tạm trả tất cả status code 200 cho bên erp
            var resPost = _service.ERPUpdateChungTuGhiSo(request);
            if (resPost.Success == true) return StatusCode(200, resPost);
            else
            {
                if (resPost.ErrorCode == "001") return StatusCode(200, resPost);
                else return StatusCode(200, resPost);
            }
        }

        [HttpGet("api/ChungTu/GetPCUNC")]
        public object GetPCUNC([FromQuery] ChungTuParams request)
        {
            var resp = _service.GetPCUNC(request);
            if (resp.Success == true)
            {
                // byte[] bytes = System.IO.File.ReadAllBytes((string)resp.Data);
                // string data = Convert.ToBase64String(bytes);
                var result = _uploadService.DownloadFileFtpServerConvertBase64((string)resp.Data);
                if (result.Success == true) {
                    return new Dictionary<string, string>() { { "Data", result.Data.ToString()} };
                }
                else {
                    return new Dictionary<string, string>() { { "Data", ""} };
                }
            }
            else
            {
                var res = _generatepdfservice.MergePCUNC(request.ChungTuId);
                byte[] bytes = System.IO.File.ReadAllBytes(res);
                string pathFile = _uploadService.UploadByteFileFTP(bytes, request.MaHoSo, "PCNHUNC", "pdf");
                _chungTuRepo.UpdateTaiLieuChungtu(chungtuId: request.ChungTuId, tailieugoc: pathFile);
                var result = _uploadService.DownloadFileFtpServerConvertBase64(pathFile);
                if (result.Success == true) {
                    if (System.IO.File.Exists(res))
                    {
                        System.IO.File.Delete(res);
                    }
                    return new Dictionary<string, string>() { { "Data", result.Data.ToString()} };
                }
                else {
                    return new Dictionary<string, string>() { { "Data", ""} };
                }
            }
        }

        [HttpGet("api/ChungTu/GetCTGS")]
        public object GetCTGS([FromQuery] ChungTuParams request)
        {
            var resp = _service.GetCTGS(request);
            if (resp.Success == true)
            {
                // byte[] bytes = System.IO.File.ReadAllBytes((string)resp.Data);
                // string data = Convert.ToBase64String(bytes);
                var result = _uploadService.DownloadFileFtpServerConvertBase64((string)resp.Data);
                if (result.Success == true) {
                    return new Dictionary<string, string>() { { "Data", result.Data.ToString()} };
                }
                else {
                    return new Dictionary<string, string>() { { "Data", ""} };
                } 
            }
            else{
                var res = _generatepdfservice.GenerateCTGS(request.ChungTuId);
                byte[] bytes = System.IO.File.ReadAllBytes(res);
                string pathFile = _uploadService.UploadByteFileFTP(bytes, request.MaHoSo, "chungTuGhiSo", "pdf");
                _chungTuRepo.UpdateTaiLieuChungtu(chungtuId: request.ChungTuId, tailieugoc: pathFile);
                var result = _uploadService.DownloadFileFtpServerConvertBase64(pathFile);
                if (result.Success == true) {
                    if (System.IO.File.Exists(res))
                    {
                        System.IO.File.Delete(res);
                    }
                    return new Dictionary<string, string>() { { "Data", result.Data.ToString()} };
                }
                else {
                    return new Dictionary<string, string>() { { "Data", ""} };
                }
            }
            
        }

        [HttpGet("api/ChungTu/Get")]
        public ActionResult<ResponseGetChungTu> GetChungTu([FromQuery] ChungTuParams request)
        {
            var resp = _service.GetChungTu(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpGet("api/ChungTu/GetByID")]
        public ActionResult<Response> GetChungTuById(Guid chungTuId)
        {
            var resp = _service.GetChungTuByID(chungTuId);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpPost("api/ChungTu/Sign")]
        public ActionResult<Response> KyChungTu([FromBody] KySoChungTuParams request)
        {
            request.NguoiKyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resp = _service.KyChungTu(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpGet("api/KySoChungTu/Get")]
        public ActionResult<ResponseGetKySoChungTu> GetKySoChungTu([FromQuery] KySoChungTuParams request)
        {           
            var resp = _service.GetKySoChungTu(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpGet("api/BanTheHien/Get")]
        public ActionResult<object> GetBanTheHien(Guid chungTuId)
        {
            Dictionary<string, object> banTheHienData = _service.GetDuLieuBanTheHien(chungTuId);
            return StatusCode(200, banTheHienData);
        }

        [HttpPost("api/ChungTu/Cancel")]
        public ActionResult<ResponseGetChungTu> CancelChungTu([FromBody] KySoChungTuParams request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resp = _service.CancelChungTu(userId, request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }
        [HttpPost("api/ChungTu/UpdateTT/{ChungTuId}/{thaoTacBuocPheDuyetId}")]
        public ActionResult<ResponseGetChungTu> UpdateTrangThaiDongCT( Guid ChungTuId, Guid thaoTacBuocPheDuyetId)
        { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resp = _service.UpdateTrangThaiDongCT(ChungTuId,userId, thaoTacBuocPheDuyetId);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }
    }
}
