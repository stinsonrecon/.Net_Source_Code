using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;
using AutoMapper;

namespace Epayment.Services
{
    public class TinhThanhPhoService : ITinhThanhPhoService
    {
        private readonly ITinhThanhPhoRepository _repo;
        private readonly IMapper _mapper;
        public TinhThanhPhoService(ITinhThanhPhoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public ResponsePostViewModel CreateTinhThanhPho(TinhThanhPhoViewModel tinhTP)
        {
            var resp = _repo.CreateTinhThanhPho(tinhTP);
            return resp;
        }

        public ResponsePostViewModel DeleteTinhThanhPho(Guid id)
        {
            var resp = _repo.DeleteTinhThanhPho(id);
            return resp;
        }

        public List<TinhThanhPhoViewModel> GetTinhThanhPho()
        {
            var resp = _repo.GetTinhThanhPho();
            return resp;
        }

        public ResponseTinhThanhPhoViewModel GetTinhThanhPhoWithPagination(ParamGetTinhThanhPhoViewModel request)
        {
            var resp = _repo.GetTinhThanhPhoWithPagination(request);
            return resp;
        }

        public ResponsePostViewModel UpdateTinhThanhPho(TinhThanhPhoViewModel tinhTP)
        {
            var resp = _repo.UpdateTinhThanhPho(tinhTP);
            return resp;
        }
    }
}