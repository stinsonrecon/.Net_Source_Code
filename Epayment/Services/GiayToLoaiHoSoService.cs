using System;
using System.Collections.Generic;
using AutoMapper;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.Repositories;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public class GiayToLoaiHoSoService: IGiayToLoaiHoSoService
    {
        private readonly IGiayToLoaiHoSoRepository _repo;
        private readonly IMapper _mapper;

        public GiayToLoaiHoSoService(IGiayToLoaiHoSoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public ResponsePostViewModel CreateGiayToLoaiHoSo(CreateGiayToLoaiHS request)
        {
            var ret = _repo.CreateGiayToLoaiHoSo(request);
            return ret;
        }

        public ResponsePostViewModel DeleteGiayToLoaiHoSo(Guid id)
        {
            var ret = _repo.DeleteGiayToLoaiHoSo(id);
            return ret;
        }

        public List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSo()
        {
            var ret = _repo.GetGiayToLoaiHoSo();
            return ret;
        }

        public List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSoById(Guid id)
        {
            var ret = _repo.GetGiayToLoaiHoSoById(id);
            return ret;
        }

        public ResponsePostViewModel UpdateGiayToLoaiHoSo(Guid id, UpdateGiayToLoaiHS request)
        {
            var ret = _repo.UpdateGiayToLoaiHoSo(id,request);
            return ret;
        }
    }
}