using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Services;
using BCXN.Statics;
using BCXN.ViewModels;
using BCXN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCXN.Data;

namespace BCXN.Controllers
{
    [Route("api/linhvucbaocao")]
    [Authorize]
    public class LinhVucBaoCaoController : Controller
    {
        private readonly ILinhVucBaoCaoService _service;

        public LinhVucBaoCaoController(ILinhVucBaoCaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<LinhVucBaoCaoViewModel>> GetLinhVucBaoCao()
        {
            var resp = _service.GetLinhVucBaoCao();
            return Ok(resp);
        }

        [HttpGet("getAll")]
        public ActionResult<ResponseLinhVucBaoCaoViewModel> GetLinhVucBaoCaoWithPagination([FromQuery]ParamGetLinhVucBaoCaoViewModel request)
        {
            var resp = _service.GetLinhVucBaoCaoWithPagination(request);
            return Ok(resp);
        }

        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateLinhVucBaoCao([FromBody]LinhVucBaoCaoViewModel request)
        {
            var resp = _service.CreateLinhVucBaoCao(request);
            return Ok(resp);
        }

        [HttpPut("update")]
        public ActionResult<ResponsePostViewModel> UpdateLinhVucBaoCao([FromBody]LinhVucBaoCaoViewModel request)
        {
            var resp = _service.UpdateLinhVucBaoCao(request);
            return Ok(resp);
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteLinhVucBaoCao(int id)
        {
            var resp = _service.DeleteLinhVucBaoCao(id);
            return resp;
        }
    }
}