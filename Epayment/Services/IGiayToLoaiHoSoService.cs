using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface IGiayToLoaiHoSoService
    {
        ResponsePostViewModel CreateGiayToLoaiHoSo (CreateGiayToLoaiHS request);
        ResponsePostViewModel UpdateGiayToLoaiHoSo(Guid id, UpdateGiayToLoaiHS request);
        ResponsePostViewModel DeleteGiayToLoaiHoSo(Guid id);
        List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSo();
        List<GiayToLoaiHoSoViewModel> GetGiayToLoaiHoSoById(Guid id);
    }
}