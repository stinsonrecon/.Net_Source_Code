using APIERP.Repository;
using APIERP.ViewModels;

namespace APIERP.Service
{
    public class HsttService : IHsttService
    {
        private readonly IHsttRepository _repo;

        public HsttService(IHsttRepository repo)
        {
            _repo = repo;
        }

        public ResponsePostView Pc_Evn_Insert_Hstt(HsttView hsttView)
        {
            var res = _repo.Pc_Evn_Insert_Hstt(hsttView);
            return res;
        }
    }
}
