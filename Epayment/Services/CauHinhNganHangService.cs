using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.ViewModels;
using AutoMapper;
using Epayment.ViewModels;
using Epayment.Repositories;

namespace Epayment.Services
{
    public interface ICauHinhNganHangService
    {
        List<CauHinhNganHangViewModel> GetCauHinhNganHang();
        Response CreateCauHinhNganHang(CauHinhNganHangParams cauHinh);
        ResponseGetCauHinhNganHang GetAllCauHinhNganHang(CauHinhNganHangPagination cauHinhPagination);
        Response UpdateCauHinhNganHang(CauHinhNganHangViewModel cauHinh);
        Response DeleteCauHinhNganHang(Guid cauHinhId);
    }
    public class CauHinhNganHangService : ICauHinhNganHangService
    {
        private readonly ICauHinhNganHangRepository _repo;
        private readonly IMapper _mapper;
        public CauHinhNganHangService(ICauHinhNganHangRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public Response CreateCauHinhNganHang(CauHinhNganHangParams cauHinh)
        {
            var resp = _repo.CreateCauHinhNganHang(cauHinh);
            return resp;
        }

        public Response DeleteCauHinhNganHang(Guid cauHinhId)
        {
            var resp = _repo.DeleteCauHinhNganHang(cauHinhId);
            return resp;
        }

        public ResponseGetCauHinhNganHang GetAllCauHinhNganHang(CauHinhNganHangPagination cauHinhPagination)
        {
            var resp = _repo.GetAllCauHinhNganHang(cauHinhPagination);
            return resp;
        }

        public List<CauHinhNganHangViewModel> GetCauHinhNganHang()
        {
            var resp = _repo.GetCauHinhNganHang();
            return resp;
        }

        public Response UpdateCauHinhNganHang(CauHinhNganHangViewModel cauHinh)
        {
            var resp = _repo.UpdateCauHinhNganHang(cauHinh);
            return resp;
        }
    }
}