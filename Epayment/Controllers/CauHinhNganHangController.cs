using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Epayment.Controllers
{
    [Route("api/cau_hinh_ngan_hang")]
    [Authorize]
    public class CauHinhNganHangController : Controller
    {
        private readonly ICauHinhNganHangService _service;

        public CauHinhNganHangController(ICauHinhNganHangService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<CauHinhNganHangViewModel>> GetCauHinhNganHang()
        {
            var resp = _service.GetCauHinhNganHang();
            return Ok(resp);
        }

        [HttpGet("GetAll")]
        public ActionResult<ResponseGetCauHinhNganHang> GetAllCauHinhNganHang([FromQuery] CauHinhNganHangPagination cauHinhPagination)
        {
            var resp = _service.GetAllCauHinhNganHang(cauHinhPagination);
            return Ok(resp);
        }

        [HttpPost("Create")]
        public ActionResult<Response> CreateCauHinhNganHang([FromBody] CauHinhNganHangParams cauHinh)
        {
            var resp = _service.CreateCauHinhNganHang(cauHinh);
            return Ok(resp);
        }

        [HttpPut("Update")]
        public ActionResult<Response> UpdateCauHinhNganHang([FromBody] CauHinhNganHangViewModel cauHinh)
        {
            var resp = _service.UpdateCauHinhNganHang(cauHinh);
            return Ok(resp);
        }

        [HttpDelete("{cauHinhId}")]
        public ActionResult<Response> DeleteCauHinhNganHang(Guid cauHinhId)
        {
            var resp = _service.DeleteCauHinhNganHang(cauHinhId);
            return Ok(resp);
        }
    }
}