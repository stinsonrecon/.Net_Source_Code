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
    [Route("api/donvi")]
    [Authorize]
    public class DonViController : Controller
    {
        private readonly IDonViService _service;

        public DonViController(IDonViService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ResponseWithPaginationViewModel> GetDonVi()
        {
            var listDonVi = _service.GetDonVi();
            return Ok(listDonVi);
        }

        [HttpGet("getAll")]
        public ActionResult<ResponseWithPaginationViewModel> GetDonViWithPagination([FromQuery] ParamsGetDonViViewModel request)
        {
            var listDonVi = _service.GetDonViWithPagination(request);
            return Ok(listDonVi);
        }

        // [Authorize(Policy = Policies.ADMIN)]
        [HttpPost]
        public ActionResult<ResponsePostViewModel> CreateDonVi([FromBody]DonVi request)
        {
            var resPost = _service.CreateDonVi(request);
            return resPost;
        }

        // [Authorize(Policy = Policies.ADMIN)]
        [HttpPut("update")]
        public ActionResult<ResponsePostViewModel> UpdateDonVi([FromBody]DonVi request)
        {
            var resPost = _service.UpdateDonVi(request);
            return resPost;
        }


        [HttpDelete("{id}")]
        // [Authorize(Policy = Policies.ADMIN)]
        public ActionResult<ResponsePostViewModel> DeleteDonVi(int id)
        {
            var ret = _service.DeleteDonVi(id);
            return ret;
        }
        [HttpGet("getBoPhanByIdDv/{DonViId}")]
        public ActionResult<ResponseWithPaginationViewModel> GetBoPhanByIdDonVi(int DonViId)
        {
            var listDonVi = _service.GetBoPhanByIdDonVi(DonViId);
            return Ok(listDonVi);
        }
        [HttpGet("getByIdDv/{DonViId}")]
        public ActionResult<DonVi> GetByIdDonVi(int DonViId)
        {
            var listDonVi = _service.GetByIdDonVi(DonViId);
            return Ok(listDonVi);
        }
    }
}