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
    public class LinhVucBaoCaoService : ILinhVucBaoCaoService
    {
        private readonly ILinhVucBaoCaoRepository _repo;
        private readonly IMapper _mapper;

        public LinhVucBaoCaoService(ILinhVucBaoCaoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public ResponsePostViewModel CreateLinhVucBaoCao(LinhVucBaoCaoViewModel request)
        {
            var resp = _repo.CreateLinhVucBaocao(request);
            return resp;
        }

        public ResponsePostViewModel DeleteLinhVucBaoCao(int id)
        {
            var resp = _repo.DeleteLinhVucBaoCao(id);
            return resp;
        }
        public List<LinhVucBaoCaoViewModel> GetLinhVucBaoCao()
        {
            var resp = _repo.GetLinhVucBaoCao();
            return resp;
        }

        public ResponseLinhVucBaoCaoViewModel GetLinhVucBaoCaoWithPagination(ParamGetLinhVucBaoCaoViewModel request)
        {
            var resp = _repo.GetLinhVucWithPagination(request);
            return resp;
        }

        public ResponsePostViewModel UpdateLinhVucBaoCao(LinhVucBaoCaoViewModel request)
        {
            var resp = _repo.UpdateLinhVucBaoCao(request);
            return resp;
        }
    }
}
