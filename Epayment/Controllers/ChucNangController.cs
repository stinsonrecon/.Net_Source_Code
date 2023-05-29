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

namespace BCXN.Controllers
{
    [Route("api/chucnang")]
    [Authorize]
    public class ChucNangController : Controller
    {
        private readonly IChucNangService _service;
        public ChucNangController(IChucNangService service)
        {
            _service = service;
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet("{NhomQuyenId}")]
        public ActionResult<List<ChucNangViewModel>> ChucNang(string NhomQuyenId)
        { 
            try
            {
                var listChucNang = _service.GetAllChucNang(NhomQuyenId);                
                return Ok(listChucNang);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        // [Authorize(Roles = Policies.TCT + "," + Policies.NCC + "," + Policies.DVTD + "," + Policies.ADMIN)]
        [HttpGet("getAll")]
        public ActionResult<ResponseWithPaginationViewModel> GetChucNang([FromQuery] ChucNangSearchViewModel request)
        {
            var listChucNang = _service.GetChucNang(request);
            return Ok(listChucNang);
        }

        // [Authorize(Policy = Policies.ADMIN)]
        [HttpPost("create")]
        public ActionResult<ResponsePostViewModel> CreateChucNang([FromBody] ChucNangGetViewModel request)
        {
            var ret = _service.CreateChucNang(request);
            return ret;
        }

        // [Authorize(Policy = Policies.ADMIN)]
        [HttpPut("update")]
        public ActionResult<ResponsePostViewModel> UpdateChucNang([FromBody] ChucNangGetViewModel request)
        {
            var ret = _service.UpdateChucNang(request);
            return ret;
        }

        // [Authorize(Policy = Policies.ADMIN)]
        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteChucNang(int id)
        {
            var ret = _service.DeleteChucNang(id);
            return ret;
        }

        
       
    }
}