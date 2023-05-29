using System;
using System.Collections.Generic;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.ModelRequest;
using Epayment.Models;
using Epayment.ViewModels;

namespace BCXN.Repositories
{
    public interface IGiayToRepository
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