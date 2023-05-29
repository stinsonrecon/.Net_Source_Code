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
    [Route("api/lich-su-ho-so-tt")]
    [Authorize]
    public class LichSuHoSoController : Controller
    {
        private readonly ILichSuHoSoTTService _service;

        public LichSuHoSoController(ILichSuHoSoTTService service)
        {
            _service = service;
        }

        [HttpGet("{hoSoId}")]
        public ActionResult<ResponseLichSuViewModel> getLichSuHoSoTT(string hoSoId)
        {
            var resp = _service.GetLichSuHoSoTT(hoSoId);
            return Ok(resp);
        }
        
    }
}