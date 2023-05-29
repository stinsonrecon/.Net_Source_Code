using APIERP.ViewModels;

namespace APIERP.Repository
{
    public interface IPaymentRepository
    {
        public ResponsePostView Pc_Evn_Update_Payment_Status(PaymentView paymentView);
    }
}
