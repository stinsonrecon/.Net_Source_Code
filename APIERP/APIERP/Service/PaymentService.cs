using APIERP.Repository;
using APIERP.ViewModels;

namespace APIERP.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;

        public PaymentService(IPaymentRepository repo)
        {
            _repo = repo;
        }

        public ResponsePostView Pc_Evn_Update_Payment_Status(PaymentView paymentView)
        {
            var res = _repo.Pc_Evn_Update_Payment_Status(paymentView);
            return res;
        }
    }
}
