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
    public class DonViService : IDonViService
    {
        private readonly IDonViRepository _repo;
        private readonly IMapper _mapper;

        public DonViService(IDonViRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public List<DonViViewModel> GetDonVi()
        {
            var donViList = _repo.GetDonVi();
            return donViList;
        }
        public ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request)
        {
            var donViList = _repo.GetDonViWithPagination(request);
            return donViList;
        }
        public ResponsePostViewModel CreateDonVi(DonVi request)
        {
            var ret = _repo.CreateDonVi(request);
            return ret;
        }
        public ResponsePostViewModel UpdateDonVi(DonVi request)
        {
            var ret = _repo.UpdateDonVi(request);
            return ret;
        }

        public ResponsePostViewModel DeleteDonVi(int id)
        {
            var ret = _repo.DeleteDonVi(id);
            return ret;
        }

        public List<DonViViewModel> GetBoPhanByIdDonVi(int id)
        {
            var ret = _repo.GetBoPhanByIdDonVi(id);
            return ret;
        }
         public DonVi GetByIdDonVi(int id)
        {
            var ret = _repo.GetDonViById(id);
            return ret;
        }
    }
}
