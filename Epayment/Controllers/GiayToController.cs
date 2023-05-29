
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
using System.Security.Claims;

namespace Epayment.Controllers
{
    [Route("api/giayto")]
    [Authorize]
    public class GiayToController : Controller
    {
        private readonly IGiayToService _service;

        public GiayToController(IGiayToService service)
        {
            _service = service;
        }
        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateGiayTo([FromBody] CreateGiayTo request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CreateGiayTo parm= new CreateGiayTo();
            parm = request;
            parm.NguoiTaoId = userId.ToString();
            var resPost = _service.CreateGiayTo(parm);
            return resPost;
        }
        [HttpPut("{id}")]
        public ActionResult<ResponsePostViewModel> UpdateGiayTo([FromBody] CreateGiayTo request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CreateGiayTo parm= new CreateGiayTo();
            parm = request;
            parm.NguoiTaoId = userId.ToString();
            var resPost = _service.UpdateGiayTo(parm);
            return resPost;
        }
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteGiayTo(Guid id)
        {
            var resPost = _service.DeleteGiayTo(id);
            return resPost;
        }
        [HttpPost("paging")]
        public ResponseGiayToViewModel GetGiayTo([FromBody] SearchGiayTo request){
            var resPost = _service.GetGiayTo(request);
            return resPost;
        }
        [HttpGet("all")]
        public List<GiayToViewModel> GetAllGiayTo(){
            var resPost = _service.GetAllGiayTo();
            return resPost;
        }
        [HttpGet("{id}")]
        public List<GiayToViewModel> GetGiayToById(Guid id){
            var resPost = _service.GetGiayToById(id);
            return resPost;
        }
        [HttpGet("getbyloaihoso")]
        public List<GiayToViewModel> GetGiayToByIdLoaiHoSo([FromQuery] Guid id){
            var resPost = _service.GetGiayToByIdLoaiHoSo(id);
            return resPost;
        }
    }
}