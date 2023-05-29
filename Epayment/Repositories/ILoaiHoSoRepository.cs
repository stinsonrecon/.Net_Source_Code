using System;
using System.Collections.Generic;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ModelRequest;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface ILoaiHoSoRepository
    {
        ResponsePostViewModel CreateLoaiHoSo (CreateLoaiHoSo request);
        ResponsePostViewModel UpdateLoaiHoSo(CreateLoaiHoSo request);
        ResponsePostViewModel DeleteLoaiHoSo(Guid id);
        List<LoaiHoSoViewModel> GetLoaiHoSo( int trangThai);
        List<LoaiHoSoViewModel> GetLoaiHoSoById(Guid id);
        ResponseLoaiHoSoViewModel GetPagingLoaiHoSo(LoaiHoSoSearchViewModel request);
        ResponsePostViewModel UpdateTrangThaiLoaiHoSo(string loaiHoSoId);
    }
}