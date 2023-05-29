

using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Epayment.Controllers
{
    [Route("api/luong-phe-duyet")]
    public class LuongPheDuyetController : Controller
    {
        private readonly ILuongPheDuyetService _service;
        public LuongPheDuyetController(ILuongPheDuyetService service)
        {
            _service = service;
        }
        [HttpGet("{loaiHoSoId}")]
        public ActionResult<ResponseLuongPheDuyetViewModel> GetLuongPheDuyet(string loaiHoSoId)
        {
            var resLuong = _service.GetLuongPheDuyet(loaiHoSoId);
            return resLuong;
        }

        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateLuongPheDuyet([FromBody] List<CreateLuongPheDuyet> request)
        {
            var resPost = _service.CreateLuongPheDuyet(request);
            return resPost;
        }
        [HttpPut("{id}")]
        public ActionResult<ResponsePostViewModel> UpdateLuongPheDuyet( [FromBody] List<CreateLuongPheDuyet> request)
        {
            var resPost = _service.UpdateLuongPheDuyet( request);
            return resPost;
        }

        [HttpDelete("{loaiHoSoId}/{luongPheDuyetId}")]
        public ActionResult<ResponsePostViewModel> DeleteLuongPheDUye(string loaiHoSoId, string luongPheDuyetId)
        {
            var deleteLPD = _service.DeleteLuongPheDuyet(loaiHoSoId, luongPheDuyetId);
            return deleteLPD;
        }

        // [HttpPut]
        // public ActionResult<ResponsePostViewModel> UpdateLuongPheDUye([FromBody] UpdateLuongPheDuyet request)
        // {
        //     var updateLPD = _service.UpdateLuongPheDuyet(request);
        //     return updateLPD;
        // }

    }
}