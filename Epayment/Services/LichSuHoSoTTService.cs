using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class LichSuHoSoTTService :ILichSuHoSoTTService
    {
        private readonly ILichSuHoSoTTRepository _repo;
        private readonly IMapper _mapper;

        public LichSuHoSoTTService(ILichSuHoSoTTRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResponsePostViewModel> CreateLichSuHoSoTT(ParmHoSoThanhToanViewModel request)
        {
            var ret = await _repo.CreateLichSuHoSoTT(request);
            return ret;
        }
        public ResponseLichSuViewModel GetLichSuHoSoTT(string hoSoId)
        {
            var ret = _repo.GetLichSuTTByHoSoId(hoSoId);
            return ret;
        }
    }
}