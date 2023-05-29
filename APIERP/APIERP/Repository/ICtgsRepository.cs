using APIERP.ViewModels;

namespace APIERP.Repository
{
    public interface ICtgsRepository
    {
        public ResponsePostView Pc_Evn_Update_Ap_Status(CtgsView ctgsView);
    }
}
