using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface IChiTietGiayToHSTTRepository
    {
        ResponsePostHSTTViewModel CreateChiTietGiayToHSTT (ParmChiTietGiayToHSTTViewModel request , List<string> url);
        ResponsePostViewModel UpdateChiTietGiayToHSTT( ParmChiTietGiayToHSTTViewModel request , List<string> url);
        ResponsePostViewModel DeleteChiTietGiayToHSTT(Guid id);
        ResponseChiTietHSTTViewModel GetChiTietGiayToHSTT(ChiTietHSTTSearchViewModel request);
        ResponsePostViewModel DeleteChiTietGiayToByIdHSTT(Guid HoSoTTId);
        Response UpdateTaiLieuToTrinh(HoSoThanhToan hsttItem, string filepath, Guid thaoTacBuocPheDuyetId, string nguoiThucHienId);
        ResponsePostHSTTViewModel CreateToTrinhChiTietGiay (ParmChiTietGiayToHSTTViewModel request );
        Response CreatePhieuThamTraHSTT (ParmPhieuThamTraHSTTViewModel request , string url);
    }
}