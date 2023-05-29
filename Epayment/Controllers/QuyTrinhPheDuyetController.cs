using BCXN.ViewModels;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Epayment.Controllers
{
    [Route("api/QuyTrinhPheDuyet")]
    [Authorize]
    public class QuyTrinhPheDuyetController : ControllerBase
    {
        private readonly QuyTrinhPheDuyetService _service;

        public QuyTrinhPheDuyetController(QuyTrinhPheDuyetService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public ActionResult CreateQuyTrinh([FromBody] CreateQuyTrinhPheDuyetViewModel request)
        {
            var resp = _service.CreateQuyTrinh(request);
            return Ok(resp);
        }

        [HttpGet("getAll")]
        public ActionResult GetPagingQuyTrinh([FromQuery] QuyTrinhPheDuyetSearchViewModel request)
        {
            var resp = _service.GetPagingQuyTrinh(request);
            return Ok(resp);
        }

        [HttpGet("get-by-loai-hs-id/{loaiHoSoId}")]
        public ActionResult GetPagingQuyTrinh(Guid loaiHoSoId)
        {
            var resp = _service.GetQuyTrinhByLoaiHoSoId(loaiHoSoId);
            return Ok(resp);
        }

        [HttpPut("update")]
        public ActionResult UpdateLinhVucBaoCao([FromBody] UpdateQuyTrinhPheDuyetViewModel request)
        {
            var resp = _service.UpdateQuyTrinh(request);
            return Ok(resp);
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteQuyTrinh(Guid id)
        {
            var resp = _service.DeleteQuyTrinh(id);
            return resp;
        }
    }
}
