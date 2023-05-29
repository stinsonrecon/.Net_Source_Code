using BCXN.Controllers;
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
    public class YeuCauChiTienController : Controller
    {
        private readonly IYeuCauChiTienService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IUserService _userService;

        public YeuCauChiTienController(IYeuCauChiTienService service, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService)
        {
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpPost("api/YeuCauChiTien/Get")]
        public ActionResult<ResponseGetYeuCauChiTien> GetYeuCauChiTien([FromBody] SearchYeuCauChiTien request)
        {
            var resp = _service.GetYeuCauChiTien(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpPost("api/YeuCauChiTien/Confirm")]
        public async Task<bool> XacNhanYeuCauChiTien([FromBody] XacNhanChuyenTienViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            return result.Succeeded;
        }

        [HttpPost("api/YeuCauChiTien/Create")]
        public ActionResult<Response> CreateYeuCauChiTienChuyenKhoan([FromBody] YeuCauChiTienParams request)
        {
            var resp = _service.CreateYeuCauChiTienChuyenKhoan(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpPost("api/YeuCauChiTien/TiemMat/Create")]
        public ActionResult<Response> CreateYeuCauChiTienMat([FromBody] YeuCauChiTienParams request)
        {
            var resp = _service.CreateYeuCauChiTienMat(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }

        [HttpPut("api/YeuCauChiTien/UpdateKetQuaHachToanNganHang")]
        public ActionResult<ResponseGetYeuCauChiTien> UpdateKetQuaHachToanNganHang([FromBody] YeuCauChiTienParams request)
        {
            // tạm trả tất cả status code 200 cho erp
            var resp = _service.UpdateKetQuaHachToanNganHang(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(200, resp);
        }
        [AllowAnonymous]
        [HttpPost("api/YeuCauChiTien/ConfirmAccount")]
        public async Task<bool> ConfirmAccount([FromBody]Epayment.Controllers.AuthenticateRequest model)
        {
            bool checkcomfirm = false;
            if (!string.IsNullOrEmpty(model.UserType)){
                if (Convert.ToInt32(model.UserType) == 0){
                    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                    checkcomfirm = result.Succeeded;
                }else if(Convert.ToInt32(model.UserType) == 1){
                    var resultEvnid = await _userService.AuthenticateID(model, ipAddress());
                    checkcomfirm = resultEvnid.Success;
                }
            }
            return checkcomfirm;
        }
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
