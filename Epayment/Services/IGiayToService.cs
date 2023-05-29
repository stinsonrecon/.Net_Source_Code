using BCXN.ViewModels;
using BCXN.Models;
using Epayment.Models;
using System.Collections.Generic;
using Epayment.ViewModels;
using System;
using Epayment.ModelRequest;

namespace Epayment.Services
{
    public interface IGiayToService
    {
        ResponsePostViewModel CreateGiayTo (CreateGiayTo request);
        ResponsePostViewModel UpdateGiayTo(CreateGiayTo request);
        ResponsePostViewModel DeleteGiayTo(Guid id);
        ResponseGiayToViewModel GetGiayTo(SearchGiayTo request);
        List<GiayToViewModel> GetGiayToById(Guid id);
        List<GiayToViewModel> GetGiayToByIdLoaiHoSo(Guid LoaiHoSoId);
        List<GiayToViewModel> GetAllGiayTo();
    }
}