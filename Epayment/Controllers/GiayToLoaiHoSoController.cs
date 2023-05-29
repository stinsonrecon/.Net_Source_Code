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
    [Route("api/giaytoloaiHS")]
    [Authorize]

    public class GiayToLoaiHoSoController :Controller 
    {
        private readonly IGiayToLoaiHoSoService _service;

        public GiayToLoaiHoSoController(IGiayToLoaiHoSoService service)
        {
            _service = service;
        }  

        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateGiayToLoaiHoSo([FromBody] CreateGiayToLoaiHS request)
        {
            var resPost = _service.CreateGiayToLoaiHoSo(request);
            return resPost;
        }
        [HttpPut("{id}")]
        public ActionResult<ResponsePostViewModel> UpdateGiayToLoaiHoSo(Guid id, [FromBody] UpdateGiayToLoaiHS request)
        {
            var resPost = _service.UpdateGiayToLoaiHoSo(id, request);
            return resPost;
        }
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteGiayToLoaiHoSo(Guid id)
        {
            var resPost = _service.DeleteGiayToLoaiHoSo(id);
            return resPost;
        }
        [HttpGet]
        public List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSo(){
            var resPost = _service.GetGiayToLoaiHoSo();
            return resPost;
        }
        [HttpGet("{id}")]
        public List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSoById(Guid id){
            var resPost = _service.GetGiayToLoaiHoSoById(id);
            return resPost;
        }
    }
}