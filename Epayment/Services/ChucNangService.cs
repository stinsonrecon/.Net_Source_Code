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
    public class ChucNangService : IChucNangService
    {
        private readonly IChucNangRepository _repo;
        private readonly IMapper _mapper;

        public ChucNangService(IChucNangRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public ResponsePostViewModel CreateChucNang(ChucNangGetViewModel request)
        {
            var ret = _repo.Add(request);
            return ret;
        }

        public ResponsePostViewModel DeleteChucNang(int id)
        {
            var ret = _repo.Delete(id);
            return ret;
        }

        public List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId)
        {
            var chucNang = _repo.GetAllChucNang(NhomQuyenId);
           
            return chucNang;
        }

        public ResponseWithPaginationViewModel GetChucNang(ChucNangSearchViewModel request)
        {
            var chucNang = _repo.GetChucNang(request);
            return chucNang;
        }

        public ResponsePostViewModel UpdateChucNang(ChucNangGetViewModel request)
        {
            var ret = _repo.Edit(request);
            return ret;
        }
    }
}
