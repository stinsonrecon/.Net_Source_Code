using BCXN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Services;
using BCXN.Statics;
using BCXN.Models;
using Microsoft.AspNetCore.Authorization;
using Epayment.Services;
using Epayment.Models;
using Epayment.ViewModels;
using Epayment.ModelRequest;

namespace Epayment.Controllers
{
    [Route("api/LoaiHoSo")]
    [Authorize]
    public class LoaiHoSoController : Controller
    {
      
    
        private readonly ILoaiHoSoService _service;

        public LoaiHoSoController(ILoaiHoSoService service)
        {
            _service = service;
        }
        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateLoaiHoSo([FromBody] CreateLoaiHoSo request)
        {
            var resPost = _service.CreateLoaiHoSo(request);
            return resPost;
        }
        [HttpPut]
        public ActionResult<ResponsePostViewModel> UpdateLoaiHoSo([FromBody] CreateLoaiHoSo request)
        {
            var resPost = _service.UpdateLoaiHoSo( request);
            return resPost;
        }
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteLoaiHoSo(Guid id)
        {
            var resPost = _service.DeleteLoaiHoSo(id);
            return resPost;
        }
        [HttpGet]
        public List<LoaiHoSoViewModel> GetLoaiHoSo( [FromQuery] int trangThai){
            var resPost = _service.GetLoaiHoSo(trangThai);
            return resPost;
        }
        [HttpGet("{id}")]
        public List<LoaiHoSoViewModel> GetLoaiHoSoById(Guid id){
            var resPost = _service.GetLoaiHoSoById(id);
            return resPost;
        }

        [HttpGet("paging")]
        public ResponseLoaiHoSoViewModel GetPagingLoaiHoSo([FromQuery] LoaiHoSoSearchViewModel request){
            var resPost = _service.GetPagingLoaiHoSo(request);
            return resPost;
        }

        [HttpPut("update-trang-thai/{loaiHoSoId}")]
        public ResponsePostViewModel UpdateTrangThaiLoaiHoSo(string loaiHoSoId)
        {
            var updateTrangThai = _service.UpdateTrangThaiLoaiHoSo(loaiHoSoId);
            return updateTrangThai;
        }
    }
}