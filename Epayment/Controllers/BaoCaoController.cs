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
    [Route("api/baocao")]
    [Authorize]
    public class BaoCaoController : Controller
    {
        private readonly IBaoCaoService _service;

        public BaoCaoController(IBaoCaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ResponseBaoCaoViewModel> GetBaoCao([FromQuery] ParamsGetBaoCaoViewModel request)
        {
            var resp = _service.GetBaoCao(request);
            return Ok(resp);
        }

        [HttpGet("linhvuc")]
        public ActionResult<List<GetBaoCaoByLinhVuc>> GetBaoCaoByLinhVuc()
        {
            var resp = _service.GetBaoCaoByLinhVuc();
            return Ok(resp);
        }

        [HttpGet("bcxn/{donViId}/{ycbcId}/{trangThai}")]
        public ActionResult<List<BaoCaoXNViewModel>> GetBaoCaoXacNhan(int donViId, int ycbcId, int trangThai)
        {
            var resp = _service.GetBaoCaoXacNhan(donViId, ycbcId, trangThai);
            return Ok(resp);
        }

        [HttpPut("bcxn/sapxep")]
        public ActionResult<ResponsePostViewModel> SapXepBaoCaoXacNhan([FromBody] List<XacNhanBaoCaoViewModel> request)
        {
            var resp = _service.SapXepBaoCaoXacNhan(request);
            return Ok(resp);
        }
        [HttpPut("sapxep")]
        public ActionResult<ResponsePostViewModel> SapXepDanhMucBaoCao([FromBody] List<BaoCaoViewModel> request)
        {
            var resp = _service.SapXepDanhMucBaoCao(request);
            return Ok(resp);
        }
        [HttpGet("{ycbcId}/{typeId}")]
        public ActionResult<List<BaoCaoViewModel>> GetBaoCaoByYeuCauBC(int ycbcId, int typeId)
        {
            var resp = _service.GetBaoCaoByYeuCauBC(ycbcId, typeId);
            return Ok(resp);
        }
        [HttpGet("thongke/{ycbcId}")]
        public ActionResult<List<ThongKeBCViewModel>> ThongKeBC(int ycbcId)
        {
            var resp = _service.ThongKeBC(ycbcId);
            return Ok(resp);
        }

        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateBaoCao([FromBody] BaoCao request)
        {
            var resPost = _service.CreateBaoCao(request);
            return resPost;
        }

        [HttpPut("{id}")]
        public ActionResult<ResponsePostViewModel> UpdateBaoCao(int id, [FromBody] BaoCao request)
        {
            var resPost = _service.UpdateBaoCao(id, request);
            return resPost;
        }
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteBaoCao(int id)
        {
            var resPost = _service.DeleteBaoCao(id);
            return resPost;
        }
        [HttpGet("GetBaoCaoTableau")]
        public ActionResult<ResponseBaoCaoViewModel> GetBaoCaoTableau()
        {
            var resp = _service.GetBaoCaoTableau();
            return Ok(resp);
        }
        
    }
}