using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCXN.Repositories;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public class LichSuXacNhanService : ILichSuXacNhanService
    {
        private readonly ILichSuXacNhanRepository _repo;
        private readonly IMapper _mapper;

        public LichSuXacNhanService(ILichSuXacNhanRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public List<LichSuXacNhanBaoCaoViewModel> GetLichSuXacNhan(int ycbcId, int bcId)
        {
            var resp = _repo.GetLichSuXacNhan(ycbcId, bcId);
            return resp;
        }
    }
}
