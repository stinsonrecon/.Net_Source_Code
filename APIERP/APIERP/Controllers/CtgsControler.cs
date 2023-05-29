using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIERP.Service;
using APIERP.ViewModels;

namespace APIERP.Controllers
{
    [Route("api")]
    [ApiController]
    public class CtgsController : ControllerBase
    {
        private readonly ICtgsService _service;

        public CtgsController(ICtgsService service)
        {
            _service = service;
        }

        [HttpPost("Pc_Evn_Update_Ap_Status")]
        public ResponsePostView Pc_Evn_Update_Ap_Status([FromBody] CtgsView ctgsView)
        {
            var res = _service.Pc_Evn_Update_Ap_Status(ctgsView);
            return res;
        }
    }
}
