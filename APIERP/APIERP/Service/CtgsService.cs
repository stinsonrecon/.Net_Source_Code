using APIERP.Repository;
using APIERP.ViewModels;

namespace APIERP.Service
{
    public class CtgsService : ICtgsService
    {
        private readonly ICtgsRepository _repo;

        public CtgsService(ICtgsRepository repo)
        {
            _repo = repo;
        }

        public ResponsePostView Pc_Evn_Update_Ap_Status(CtgsView reqCtgs)
        {
            var res = _repo.Pc_Evn_Update_Ap_Status(reqCtgs);
            return res;
        }
    }
}
