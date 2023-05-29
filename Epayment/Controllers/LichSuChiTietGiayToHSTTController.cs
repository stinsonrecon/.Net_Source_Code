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
    [Route("api/lich-chi-tiet-giay-to-su-ho-so-tt")]
    [Authorize]
    public class LichSuChiTietGiayToHSTTController : Controller
    {
        private readonly ILichSuChiTietGiayToService _service;

        public LichSuChiTietGiayToHSTTController(ILichSuChiTietGiayToService service)
        {
            _service = service;
        }
        [HttpGet()]
        public ActionResult<ResponseLichSuViewModel> getLichSuHoSoTT([FromQuery] string hoSoId, string giayToId)
        {
            var resp = _service.GetLichSuChiTietGiayHSTTByHoSoId(hoSoId, giayToId);
            return Ok(resp);
        }
    }
}