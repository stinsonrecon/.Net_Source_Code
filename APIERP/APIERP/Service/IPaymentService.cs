using APIERP.ViewModels;

namespace APIERP.Service
{
    public interface IPaymentService
    {
        public ResponsePostView Pc_Evn_Update_Payment_Status(PaymentView paymentView);
    }
}
