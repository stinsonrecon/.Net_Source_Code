using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public interface IYeuCauBaoCaoService
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
