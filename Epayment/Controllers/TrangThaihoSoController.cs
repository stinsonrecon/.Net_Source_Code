using Epayment.Services;
using Epayment.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Epayment.Controllers
{
    [Route("api/trang-thai-ho-so")]
    public class TrangThaiHoSoController : Controller
    {
        private readonly ITrangThaiHoSoService _service;
        public TrangThaiHoSoController(ITrangThaiHoSoService service)
        {
            _service = service;
        }
        // api lấy trạng thái hồ sơ, sử dụng trong việc lấy thông tin 
        // "vai trò của node" trong giao diện định nghĩa quá trình phê duyệt hồ sơ
        [HttpGet]
        public ResponseTrangThaiHoSoViewModel GetTrangThaiHoSo()
        {
            var getTTHS = _service.GetTrangThaiHoSo();
            return getTTHS;
        }

    }
}