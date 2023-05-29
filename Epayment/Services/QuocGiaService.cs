using AutoMapper;
using BCXN.ViewModels;
using Epayment.Repositories;
using Epayment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epayment.Services
{
    public interface IQuocGiaService
    {
        List<QuocGiaViewModel> GetQuocGia();
        Response CreateQuocGia(QuocGiaParam quocGia);
        ResponseGetQuocGia GetAllQuocGia(QuocGiaPagination quocGiaPagination);
        Response UpdateQuocGia(QuocGiaViewModel quocGia);
        Response DeleteQuocGia(Guid quocGiaId);
        List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId);
    }
    public class QuocGiaService : IQuocGiaService
    {
        private readonly IQuocGiaRepository _repo;
        private readonly IMapper _mapper;
        public QuocGiaService(IQuocGiaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public Response CreateQuocGia(QuocGiaParam quocGia)
        {
            return _repo.CreateQuocGia(quocGia);
        }

        public Response DeleteQuocGia(Guid quocGiaId)
        {
            return _repo.DeleteQuocGia(quocGiaId);
        }

        public ResponseGetQuocGia GetAllQuocGia(QuocGiaPagination quocGiaPagination)
        {
            return _repo.GetAllQuocGia(quocGiaPagination);
        }

        public List<QuocGiaViewModel> GetQuocGia()
        {
            return _repo.GetQuocGia();
        }

        public List<TinhThanhPhoViewModel> GetTinhTPByQuocGiaId(Guid quocGiaId)
        {
            return _repo.GetTinhTPByQuocGiaId(quocGiaId);
        }

        public Response UpdateQuocGia(QuocGiaViewModel quocGia)
        {
            return _repo.UpdateQuocGia(quocGia);
        }
    }
}