using AutoMapper;
using BCXN.ViewModels;
using BCXN.Models;
using BCXN.Repositories;
using Epayment.Models;
using System.Collections.Generic;
using Epayment.ViewModels;
using System;
using Epayment.ModelRequest;

namespace Epayment.Services
{
    public class GiayToService: IGiayToService
    {
        private readonly IGiayToRepository _repo;
        private readonly IMapper _mapper;

        public GiayToService(IGiayToRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public ResponsePostViewModel CreateGiayTo(CreateGiayTo request)
        {
            var ret = _repo.CreateGiayTo(request);
            return ret;
        }

         public ResponsePostViewModel UpdateGiayTo( CreateGiayTo request)
        {
            var ret = _repo.UpdateGiayTo( request);
            return ret;
        }

        public ResponsePostViewModel DeleteGiayTo(Guid id)
        {
            var ret = _repo.DeleteGiayTo(id);
            return ret;
        }
        public ResponseGiayToViewModel GetGiayTo(SearchGiayTo request)
        {
            var ret = _repo.GetGiayTo(request);
            return ret;
        }
         public List<GiayToViewModel> GetGiayToById(Guid id)
        {
            var ret = _repo.GetGiayToById(id);
            return ret;
        }

        public List<GiayToViewModel> GetGiayToByIdLoaiHoSo(Guid LoaiHoSoId)
        {
            var ret = _repo.GetGiayToByIdLoaiHoSo(LoaiHoSoId);
            return ret;
        }

        public List<GiayToViewModel> GetAllGiayTo()
        {
            var getAll = _repo.GetAllGiayTo();
            return getAll;
        }
    }
}