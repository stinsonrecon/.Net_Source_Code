using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIERP.Service;
using APIERP.ViewModels;

namespace APIERP.Controllers
{
    [Route("api")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPut("Pc_Evn_Update_Payment_Status")]
        public ResponsePostView Pc_Evn_Update_Payment_Status([FromBody] PaymentView paymentView)
        {
            var res = _service.Pc_Evn_Update_Payment_Status(paymentView);
            return res;
        }
    }
}
