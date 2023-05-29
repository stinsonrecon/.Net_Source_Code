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
    public class LoaiHoSoService :ILoaiHoSoService
    {
        private readonly ILoaiHoSoRepository _repo;
        private readonly IMapper _mapper;

        public LoaiHoSoService(ILoaiHoSoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public ResponsePostViewModel CreateLoaiHoSo(CreateLoaiHoSo request)
        {
            var ret = _repo.CreateLoaiHoSo(request);
            return ret;
        }

         public ResponsePostViewModel UpdateLoaiHoSo(CreateLoaiHoSo request)
        {
            var ret = _repo.UpdateLoaiHoSo(request);
            return ret;
        }

        public ResponsePostViewModel DeleteLoaiHoSo(Guid id)
        {
            var ret = _repo.DeleteLoaiHoSo(id);
            return ret;
        }
        public List<LoaiHoSoViewModel> GetLoaiHoSo( int trangThai)
        {
            var ret = _repo.GetLoaiHoSo(trangThai);
            return ret;
        }
         public List<LoaiHoSoViewModel> GetLoaiHoSoById(Guid id)
        {
            var ret = _repo.GetLoaiHoSoById(id);
            return ret;
        }

        public ResponseLoaiHoSoViewModel GetPagingLoaiHoSo(LoaiHoSoSearchViewModel request)
        {
            var lhs = _repo.GetPagingLoaiHoSo(request);
            return lhs;
        }

        public ResponsePostViewModel UpdateTrangThaiLoaiHoSo(string loaiHoSoId)
        {
            var updateTrangThai = _repo.UpdateTrangThaiLoaiHoSo(loaiHoSoId);
            return updateTrangThai;
        }
    }
}