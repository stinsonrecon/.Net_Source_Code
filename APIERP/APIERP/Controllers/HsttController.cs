using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIERP.Service;
using APIERP.ViewModels;

namespace APIERP.Controllers
{
    [Route("api")]
    [ApiController]
    public class HsttController : ControllerBase
    {
        private readonly IHsttService _service;

        public HsttController(IHsttService service)
        {
            _service = service;
        }

        [HttpPost("Pc_Evn_Insert_Hstt")]
        public ResponsePostView Pc_Evn_Insert_Hstt([FromBody] HsttView hsttView)
        {
            var res = _service.Pc_Evn_Insert_Hstt(hsttView);
            return res;
        }
    }
}
