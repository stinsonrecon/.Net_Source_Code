using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface IChiTietGiayToHSTTService
    {
        Task<ResponsePostHSTTViewModel> CreateChiTietGiayToHSTT (ParmChiTietGiayToHSTTViewModel request , List<string> url);
        Task<ResponsePostViewModel> UpdateChiTietGiayToHSTT( ParmChiTietGiayToHSTTViewModel request , List<string> url);
        ResponsePostViewModel DeleteChiTietGiayToHSTT(Guid id);  
        ResponseChiTietHSTTViewModel GetChiTietGiayToHSTT(ChiTietHSTTSearchViewModel request);
        ResponsePostViewModel DeleteChiTietGiayToByIdHSTT(Guid HoSoTTId);
        Task<Response> CreatePhieuThamTraHSTT (ParmPhieuThamTraHSTTViewModel request , string url);
    }
}