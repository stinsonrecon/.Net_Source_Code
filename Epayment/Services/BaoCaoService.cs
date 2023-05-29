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
    public class BaoCaoService : IBaoCaoService
    {
        private readonly IBaoCaoRepository _repo;
        private readonly IMapper _mapper;

        public BaoCaoService(IBaoCaoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public ResponseBaoCaoViewModel GetBaoCao(ParamsGetBaoCaoViewModel request)
        {
            var resp = _repo.GetBaoCao(request);
            return resp;
        }

        public List<GetBaoCaoByLinhVuc> GetBaoCaoByLinhVuc()
        {
            var resp = _repo.GetBaoCaoByLinhVuc();
            return resp;
        }
        public List<BaoCaoXNViewModel> GetBaoCaoXacNhan(int donViId, int ycbcId, int trangThai)
        {
            var resp = _repo.GetBaoCaoXacNhan(donViId, ycbcId, trangThai);
            return resp;
        }
        public List<ThongKeBCViewModel> ThongKeBC(int ycbcId)
        {
            var resp = _repo.ThongKeBC(ycbcId);
            return resp;
        }
        public ResponsePostViewModel SapXepDanhMucBaoCao(List<BaoCaoViewModel> request)
        {
            var resp = _repo.SapXepDanhMucBaoCao(request);
            return resp;
        }
        public ResponsePostViewModel SapXepBaoCaoXacNhan(List<XacNhanBaoCaoViewModel> request)
        {
            var resp = _repo.SapXepBaoCaoXacNhan(request);
            return resp;
        }
        public List<BaoCaoViewModel> GetBaoCaoByYeuCauBC(int ycbcId, int typeId)
        {
            var resp = _repo.GetBaoCaoByYeuCauBC(ycbcId, typeId);
            return resp;
        }

        public ResponsePostViewModel CreateBaoCao(BaoCao request)
        {
            var ret = _repo.CreateBaoCao(request);
            return ret;
        }
        public ResponsePostViewModel UpdateBaoCao(int id, BaoCao request)
        {
            var ret = _repo.UpdateBaoCao(id, request);
            return ret;
        }
        public ResponsePostViewModel DeleteBaoCao(int id)
        {
            var ret = _repo.DeleteBaoCao(id);
            return ret;
        }
        public ResponseBaoCaoViewModel GetBaoCaoTableau()
        {
            var ret = _repo.GetBaoCaoTableau();
            return ret;
        }
        public ResponsePostViewModel UpdateBaoCaoAuto(int BaoCaoId, string filename)
        {
            var ret = _repo.UpdateBaoCaoAuto(BaoCaoId, filename);
            return ret;
        }

    }
}
