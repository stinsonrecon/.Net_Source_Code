using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public interface IBaoCaoService
    {
        ResponseBaoCaoViewModel GetBaoCao(ParamsGetBaoCaoViewModel request);
        List<GetBaoCaoByLinhVuc> GetBaoCaoByLinhVuc();
        List<BaoCaoXNViewModel> GetBaoCaoXacNhan(int donViId, int ycbcId, int trangThai);
        List<ThongKeBCViewModel> ThongKeBC(int ycbcId);
        ResponsePostViewModel SapXepDanhMucBaoCao(List<BaoCaoViewModel> request);
        ResponsePostViewModel SapXepBaoCaoXacNhan(List<XacNhanBaoCaoViewModel> request);
        List<BaoCaoViewModel> GetBaoCaoByYeuCauBC(int ycbcId, int typeId);
        ResponsePostViewModel CreateBaoCao(BaoCao request);
        ResponsePostViewModel UpdateBaoCao(int id, BaoCao request);
        ResponsePostViewModel DeleteBaoCao(int id);
        ResponseBaoCaoViewModel GetBaoCaoTableau();
    }
}
