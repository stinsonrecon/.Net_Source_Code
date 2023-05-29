using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class LichSuChiTietGiayToService :ILichSuChiTietGiayToService
    {
        private readonly ILichSuChiTietGiayToRepository _repo;
        private readonly IMapper _mapper;

        public LichSuChiTietGiayToService(ILichSuChiTietGiayToRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResponsePostViewModel> CreateLichSuChiTietGiayToHSTT(ParmChiTietGiayToHSTTViewModel request, List<string> url)
        {
            var ret = await _repo.CreateLichSuChiTietGiayToHSTT(request, url);
            return ret;

        }
        public ResponseLichSuChiTietGiayToHSTTViewModel GetLichSuChiTietGiayHSTTByHoSoId(string hoSoId,string giayToId)
        {
            var ret =  _repo.GetLichSuChiTietGiayHSTTByHoSoId(hoSoId,giayToId);
            return ret;
        }
    }
}