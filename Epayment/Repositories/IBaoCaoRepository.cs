using BCXN.Models;
using BCXN.ViewModels;
using System.Collections.Generic;

namespace BCXN.Repositories
{
    public interface IBaoCaoRepository
    {
        ResponseBaoCaoViewModel GetBaoCao(ParamsGetBaoCaoViewModel request);
        List<GetBaoCaoByLinhVuc> GetBaoCaoByLinhVuc();
        ResponseBaoCaoViewModel GetBaoCaoTableau();
        List<BaoCaoXNViewModel> GetBaoCaoXacNhan(int donViId, int ycbcId, int trangThai);
        List<ThongKeBCViewModel> ThongKeBC(int ycbcId);
        ResponsePostViewModel SapXepBaoCaoXacNhan(List<XacNhanBaoCaoViewModel> request);
        ResponsePostViewModel SapXepDanhMucBaoCao(List<BaoCaoViewModel> request);
        List<BaoCaoViewModel> GetBaoCaoByYeuCauBC(int ycbcId, int typeId);
        ResponsePostViewModel CreateBaoCao(BaoCao request);
        ResponsePostViewModel UpdateBaoCao(int id, BaoCao request);
        ResponsePostViewModel DeleteBaoCao(int id);
        ResponsePostViewModel UpdateBaoCaoAuto(int BaoCaoId, string filename);
    }
}
