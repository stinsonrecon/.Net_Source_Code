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

namespace Epayment.Controllers
{
    [Route("api/LichSuHoSoTT")]
    [Authorize]
    public class LichSuThanhToanController: Controller
    {
        private readonly ILichSuHoSoTTService _service;

        public LichSuThanhToanController(ILichSuHoSoTTService service)
        {
            _service = service;
        }
        // [HttpPost]
        // public ActionResult<ResponsePostViewModel> CreateLichSuHoSoTT([FromBody] ParmHoSoThanhToanViewModel request)
        // {
        //     var resPost = _service.CreateLichSuHoSoTT(request);
        //     return resPost;
        // }
    }
}