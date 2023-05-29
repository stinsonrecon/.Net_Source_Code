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
    public class YeuCauBaoCaoService : IYeuCauBaoCaoService
    {
        private readonly IYeuCauBaoCaoRepository _repo;
        private readonly IMapper _mapper;

        public YeuCauBaoCaoService(IYeuCauBaoCaoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public ResponseYeuCauBaoCaoViewModel GetYeuCauBaoCao(ParamsGetYeuCauBaoCaoViewModel request)
        {
            var resp = _repo.GetYeuCauBaoCao(request);
            return resp;
        }
        public YeuCauBaoCaoViewModel GetYeuCauBaoCaoById(int donViId, int ycbcId)
        {
            var resp = _repo.GetYeuCauBaoCaoById(donViId, ycbcId);
            return resp;
        }

        public ResponsePostViewModel CreateYeuCauBaoCao(YeuCauBaoCaoCreateViewModel request)
        {
            var resp = _repo.CreateYeuCauBaoCao(request);
            return resp;
        }
        public ResponsePostViewModel UpdateYeuCauBaoCao(int ycbcId, YeuCauBaoCaoCreateViewModel request)
        {
            var resp = _repo.UpdateYeuCauBaoCao(ycbcId, request);
            return resp;
        }
        public ResponsePostViewModel DeleteYeuCauBaoCao(int ycbcId)
        {
            var resp = _repo.DeleteYeuCauBaoCao(ycbcId);
            return resp;
        }
        public ResponsePostViewModel XacNhanYeuCauBaoCao(int xnbcId, XacNhanBaoCaoViewModel request)
        {
            var resp = _repo.XacNhanYeuCauBaoCao(xnbcId, request);
            return resp;
        }
        public ResponsePostViewModel UpdateTrangThai(int ycbcId, int trangThai)
        {
            var resp = _repo.UpdateTrangThai(ycbcId, trangThai);
            return resp;
        }
        public ResponsePostViewModel UpdateXNBC(int xnbcId, XacNhanBaoCaoViewModel request)
        {
            var resp = _repo.UpdateXNBC(xnbcId, request);
            return resp;
        }

        public Response UpdateFileBaoCaoTongHopHieuChinh(int ycbcID, string path)
        {
            var resp = _repo.UpdateFileBaoCaoTongHopHieuChinh(ycbcID, path);
            return resp;
        }

        public Response GetFileBaoCaoTongHopHieuChinh(int ycbcID)
        {
            var resp = _repo.GetFileBaoCaoTongHopHieuChinh(ycbcID);
            return resp;
        }
    }
}
