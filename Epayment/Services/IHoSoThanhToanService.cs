using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface IHoSoThanhToanService
    {
        Task<ResponsePostHSTTViewModel> CreateHoSoTT (ParmHoSoThanhToanViewModel request);
        Task<ResponsePostViewModel> UpdateHoSoTT(ParmHoSoThanhToanViewModel request);
        ResponsePostViewModel DeleteHoSoTT(Guid id, string userID);
        List<HoSoThanhToanViewModel> GetHoSoTT();
        ResponseHoSoViewModel GetHoSoTTById(string id);
        ResponseHoSoViewModel GetHoSoPaging(HoSoSearchViewModel request);
        ResponsePostViewModel ApproveHoSo(ApproveHoSoTT request);
        ResponsePheDuyetHoSoTTViewModel GetPheDuyetHoSoTTById(string id);
        Task<ResponsePostViewModel> UpdateFileHoSoTT(CreateFileHoSoTT request);
        ResponsePaging GetAllHoSoThamChieu(HoSoThamChieuSearchViewModel request );
        ResponsePostViewModel PhanCongXuLy(ModelRequest.PhanCongXuLy request);
    }
}