using APIERP.Repository;
using APIERP.Service;
using APIERP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIERP.Controllers
{
    [Route("api")]
    [ApiController]
    public class BankController : Controller
    {
        private readonly INganHangService _service;
        public BankController(INganHangService service)
        {
            _service = service;
        }
        [HttpPost("Insert_Tai_Khoan_Ngan_Hang")]
        public Response Insert_Tai_Khoan_Ngan_Hang([FromBody] TaiKhoanNganHangParams tk)
        {
            var res = _service.Insert_Tai_Khoan_Ngan_Hang(tk);
            return res;
        }
        [HttpPost("Update_Tai_Khoan_Ngan_Hang_Nhan")]
        public Response Update_Tai_Khoan_Ngan_Hang_Nhan([FromBody] TaiKhoanNganHangParams tk)
        {
            var res = _service.Update_Tai_Khoan_Ngan_Hang_Nhan(tk);
            return res;
        }
        [HttpPost("Insert_Chi_Nhanh_Ngan_Hang")]
        public Response Insert_Chi_Nhanh_Ngan_Hang([FromBody] ChiNhanhNganHangParams cn)
        {
            var res = _service.Insert_Chi_Nhanh_Ngan_Hang(cn);
            return res;
        }
        [HttpPost("Insert_Ngan_Hang")]
        public Response Insert_Ngan_Hang([FromBody] NganHangParams nh)
        {
            var res = _service.Insert_Ngan_Hang(nh);
            return res;
        }
    }
}
