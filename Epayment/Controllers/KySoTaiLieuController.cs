using BCXN.Models;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Epayment.Controllers
{
    [Authorize]
    public class KySoTaiLieuController : Controller
    {
        private readonly IKySoTaiLieuService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public KySoTaiLieuController(IKySoTaiLieuService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        [HttpPost("api/ToTrinh/Sign")]
        public ActionResult<Response> KyToTrinh([FromBody] KySoTaiLieuParams request)
        {
            request.NguoiKyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resPost = _service.KyToTrinh(request);
            if (resPost.Success == true) return StatusCode(201, resPost);
            else if (resPost.Success == false && resPost.ErrorCode == "004") return StatusCode(200, resPost);
            else return StatusCode(500, resPost);
        }

        [HttpPost("api/ToTrinh/GetPaging")]
        public ActionResult<ResponseGetKySoTaiLieu> GetPagingKySoTT([FromBody] SearchKySoTaiLieu request)
        {
            var resPost = _service.GetKySoTaiLieuPaging(request);
           return resPost;
        }

        [HttpGet("api/ToTrinh/GetById/{KySoTaiLieuId}")]
        public ActionResult<ResponseGetKySoTaiLieu> GetKySoTTById(Guid KySoTaiLieuId)
        {
            var resPost = _service.GetKySoTaiLieuById(KySoTaiLieuId);
           return resPost;
        }
        // [HttpPut("api/ToTrinh")]
        // public ActionResult<Response> UpdateKySoTaiLieu([FromBody] KySoTaiLieuParams request)
        // {
        //     var resPost = _service.UpdateSoTaiLieu(request);
        //    return resPost;
        // }

        [HttpPost("api/ToTrinh/Cancel")]
        public ActionResult<Response> CancelToTrinh([FromBody] KySoTaiLieuParams request)
        {
            var nguoiHuy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resp = _service.CancelToTrinh(request, nguoiHuy);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpPost("api/PhieuThamTra/Sign")]
        public ActionResult<Response> KyPhieuThamTra ([FromBody] KySoPhieuThamTraPram request)
        {
            request.NguoiKyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var resPost = _service.KySoPhieuThamTraHoSo(request);
            return resPost;
        }
        [HttpGet("api/PhieuThamTra")]
        public ActionResult<Response> GetPhieuThamTraHoSoTT([FromQuery] Guid HoSoId, Guid GiayToId)
        {
            var resPost = _service.GetPhieuThamTraByIdHSTT(HoSoId, GiayToId);
           return resPost;
        }
        [HttpPost("api/TiepNhanHoSo/KyNhay")]
        public ActionResult<Response> KySoTiepNhanHoSo ([FromBody] ApproveHoSoTT request)
        {
            var resPost = _service.KySoTiepNhanHoSo(request);
            return resPost;
        }
    }
}
