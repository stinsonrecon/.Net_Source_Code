using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class LuongPheDuyetService : ILuongPheDuyetService
    {
        private readonly ILuongPheDuyetRepository _repo;
        public LuongPheDuyetService(ILuongPheDuyetRepository repo){
            _repo = repo;
        }

        public ResponsePostViewModel DeleteLuongPheDuyet(string loaiHoSoId, string luongPheDuyetId)
        {
            var deleteLPD = _repo.deleteLuongPheDuyet(loaiHoSoId, luongPheDuyetId);
            return deleteLPD;
        }

        public ResponseLuongPheDuyetViewModel GetLuongPheDuyet(string LoaiHoSoId)
        {
            var lpd = _repo.getLuongPheDuyet(LoaiHoSoId);
            return lpd;
        }
        public ResponsePostViewModel CreateLuongPheDuyet(List<CreateLuongPheDuyet> request)
        {
            var lpd = _repo.CreateLuongPheDuyet(request);
            return lpd;
        }

        public ResponsePostViewModel UpdateLuongPheDuyet( List<CreateLuongPheDuyet> request)
        {
            var lpd = _repo.UpdateLuongPheDuyet(request);
            return lpd;
        }

        // public ResponsePostViewModel UpdateLuongPheDuyet(UpdateLuongPheDuyet request)
        // {
        //     var updateLPD = _repo.updateLuongPheDuyet(request);
        //     return updateLPD;
        // }
    }
}