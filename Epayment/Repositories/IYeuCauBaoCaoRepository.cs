using BCXN.Models;
using BCXN.ViewModels;
using System.Collections.Generic;

namespace BCXN.Repositories
{
    public interface IYeuCauBaoCaoRepository
    {
        ResponseYeuCauBaoCaoViewModel GetYeuCauBaoCao(ParamsGetYeuCauBaoCaoViewModel request);
        YeuCauBaoCaoViewModel GetYeuCauBaoCaoById(int donViId, int ycbcId);
        ResponsePostViewModel CreateYeuCauBaoCao(YeuCauBaoCaoCreateViewModel request);
        ResponsePostViewModel UpdateYeuCauBaoCao(int ycbcId, YeuCauBaoCaoCreateViewModel request);
        ResponsePostViewModel DeleteYeuCauBaoCao(int ycbcId);
        ResponsePostViewModel XacNhanYeuCauBaoCao(int xnbcId, XacNhanBaoCaoViewModel request);
        ResponsePostViewModel UpdateTrangThai(int ycbcId, int trangThai);
        ResponsePostViewModel UpdateXNBC(int xnbcId, XacNhanBaoCaoViewModel request);
        Response UpdateFileBaoCaoTongHopHieuChinh(int ycbcID, string path);
        Response GetFileBaoCaoTongHopHieuChinh(int ycbcID);
    }
}
