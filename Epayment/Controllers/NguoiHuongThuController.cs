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
    [Route("api/nguoi-huong-thu")]
    [Authorize]
    public class NguoiHuongThuController : Controller
    {
      
    
        private readonly INguoiHuongThuService _service;

        public NguoiHuongThuController(INguoiHuongThuService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public List<NguoiHuongThuViewModel> GetListNguoiHuongThu(){
            var resPost = _service.GetListNguoiHuongThu();
            return resPost;
        }
    }
}