using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Epayment.Controllers
{
    [Route("api/tinhthanhpho")]
    [Authorize]
    public class TinhThanhPhoController : Controller
    {
        private readonly ITinhThanhPhoService _service;
        public TinhThanhPhoController(ITinhThanhPhoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<TinhThanhPhoViewModel>> GetTinhThanhPho()
        {
            var resp = _service.GetTinhThanhPho();
            return Ok(resp);
        }

        [HttpGet("GetAll")]
        public ActionResult<ResponseTinhThanhPhoViewModel> GetTinhThanhPhoWithPagination([FromQuery] ParamGetTinhThanhPhoViewModel request)
        {
            var resp = _service.GetTinhThanhPhoWithPagination(request);
            return Ok(resp);
        }

        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateTinhThanhPho([FromBody] TinhThanhPhoViewModel request)
        {
            var resp = _service.CreateTinhThanhPho(request);
            return Ok(resp);
        }

        [HttpPut("update")]
        public ActionResult<ResponsePostViewModel> UpdateTinhThanhPho([FromBody] TinhThanhPhoViewModel request)
        {
            var resp = _service.UpdateTinhThanhPho(request);
            return Ok(resp);
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteTinhThanhPho(Guid id)
        {
            var resp = _service.DeleteTinhThanhPho(id);
            return resp;
        }
    }
}