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
    [Authorize]
    public class QuocGiaController : Controller
    {
        private readonly IQuocGiaService _service;

        public QuocGiaController(IQuocGiaService service)
        {
            _service = service;
        }
        [HttpGet("api/quocGia/getAll")]
        public List<QuocGiaViewModel> GetQuocGia()
        {
            var resp = _service.GetQuocGia();
            return resp;
        }
        [HttpGet("api/tinhTp/{quocGiaId}")]
        public List<TinhThanhPhoViewModel> GetTinhTpByQuocGiaId(Guid quocGiaId)
        {
            var resp = _service.GetTinhTPByQuocGiaId(quocGiaId);
            return resp;
        }
        [HttpGet("api/quocGia/getPages")]
        public ActionResult<ResponseGetQuocGia> GetAllQuocGia([FromQuery] QuocGiaPagination quocGiaPagination)
        {
            var resp = _service.GetAllQuocGia(quocGiaPagination);
            return Ok(resp);
        }
        [HttpPost("api/quocGia/create")]
        public ActionResult<Response> CreateQuocGia([FromBody] QuocGiaParam quocGia)
        {
            var resp = _service.CreateQuocGia(quocGia);
            return Ok(resp);
        }
        [HttpPut("api/quocGia/update")]
        public ActionResult<Response> UpdateQuocGia([FromBody] QuocGiaViewModel quocGia)
        {
            var resp = _service.UpdateQuocGia(quocGia);
            return Ok(resp);
        }
        [HttpDelete("api/quocGia/delete")]
        public ActionResult<Response> DeleteQuocGia([FromQuery] Guid quocGiaId)
        {
            var resp = _service.DeleteQuocGia(quocGiaId);
            return Ok(resp);
        }
    }
}