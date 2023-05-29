using AutoMapper;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Services
{
    public interface IChiTietHachToanService
    {
        ResponseGetChiTietHachToan GetChiTietHachToan(ChiTietHachToanParams ctht);
        Response UpdateChiTietHachToan(Guid id, ChiTietHachToanParams ctht);
        Response CreateChiTietHachToan(ChiTietHachToanParams ctht);
    }

    public class ChiTietHachToanService : IChiTietHachToanService
    {
        private readonly IChiTietHachToanRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ChiTietHachToanService(IChiTietHachToanRepository repo, IMapper mapper, IConfiguration configuration)
        {
            _repo = repo;
            _mapper = mapper;
            _configuration = configuration;
        }

        public ResponseGetChiTietHachToan GetChiTietHachToan(ChiTietHachToanParams ctht)
        {
            var resp = _repo.GetChiTietHachToan(ctht);
            return resp;
        }

        public Response UpdateChiTietHachToan(Guid id, ChiTietHachToanParams ctht)
        {
            var resp = _repo.UpdateChiTietHachToan(id, ctht);
            return resp;
        }

        public Response CreateChiTietHachToan(ChiTietHachToanParams ctht)
        {
            var resp = _repo.CreateChiTietHachToan(ctht);
            return resp;
        }
    }
}
