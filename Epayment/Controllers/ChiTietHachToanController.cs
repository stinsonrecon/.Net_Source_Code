using BCXN.ViewModels;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Controllers
{
    public class ChiTietHachToanController : Controller
    {
        private readonly IChiTietHachToanService _service;

        public ChiTietHachToanController(IChiTietHachToanService service)
        {
            _service = service;
        }

        [HttpPost("api/ChiTietHachToan/Create")]
        public ActionResult<Response> CreateChiTietHachToan([FromBody] ChiTietHachToanParams request)
        {
            var resPost = _service.CreateChiTietHachToan(request);
            if (resPost.Success == true) return StatusCode(201, resPost);
            else return StatusCode(500, resPost);
        }

        [HttpPost("api/ChiTietHachToan/Update/{id}")]
        public ActionResult<Response> UpdateChiTietHachToan(Guid id, [FromBody] ChiTietHachToanParams request)
        {
            var resPost = _service.UpdateChiTietHachToan(id, request);
            if (resPost.Success == true) return StatusCode(200, resPost);
            else
            {
                if (resPost.ErrorCode == "001") return StatusCode(404, resPost);
                else return StatusCode(500, resPost);
            }
        }

        [HttpGet("api/ChiTietHachToan/Get")]
        public ActionResult<ResponseGetChiTietHachToan> GetChiTietHachToan([FromQuery] ChiTietHachToanParams request)
        {
            var resp = _service.GetChiTietHachToan(request);
            if (resp.Success == true) return StatusCode(200, resp);
            else return StatusCode(500, resp);
        }
    }
}
