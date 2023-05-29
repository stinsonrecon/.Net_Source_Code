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
    [Route("api/lichsuxacnhan")]
    [Authorize]
    public class LichSuXacNhanController : Controller
    {
        private readonly ILichSuXacNhanService _service;

        public LichSuXacNhanController(ILichSuXacNhanService service)
        {
            _service = service;
        }

        [HttpGet("{ycbcId}/{bcId}")]
        public ActionResult<List<LichSuXacNhanBaoCaoViewModel>> GetLichSuXacNhan(int ycbcId, int bcId)
        {
            var resp = _service.GetLichSuXacNhan(ycbcId, bcId);
            return Ok(resp);
        }
    }
}