using BCXN.ViewModels;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Epayment.Controllers
{
    [Route("api/CauHinhBuocPheDuyet")]
    [Authorize]
    public class CauHinhBuocPheDuyetController : ControllerBase
    {
        private readonly QuyTrinhPheDuyetService _service;

        public CauHinhBuocPheDuyetController(QuyTrinhPheDuyetService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public ActionResult CreateCauHinhBuocPheDuyet([FromBody] CreateBuocPheDuyetViewModel request)
        {
            var resp = _service.CreateCauHinhBuocPheDuyet(request);
            return Ok(resp);
        }

        [HttpGet("get-by-id/{id}")]
        public ActionResult<BuocPheDuyetChiTietViewModel> GetCauHinhBuocPheDuyet(Guid id)
        {
            var resp = _service.GetCauHinhBuocPheDuyet(id);
            return resp;
        }

        [HttpPut("update")]
        public ActionResult CreateCauHinhBuocPheDuyet([FromBody] UpdateBuocPheDuyetViewModel request)
        {
            var resp = _service.UpdateCauHinhBuocPheDuyet(request);
            return Ok(resp);
        }

        [HttpGet("get-ds-phe-duyet")]
        public ActionResult GetDsBuocPheDuyet([FromQuery] Guid quyTrinhPheDuyetId)
        {
            var resp = _service.GetDsBuocPheDuyet(quyTrinhPheDuyetId);
            return Ok(resp);
        }
    }
}
