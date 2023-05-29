using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;

namespace Epayment.Repositories
{
    public interface ILichSuHoSoTTRepository
    {
        Task<ResponsePostViewModel> CreateLichSuHoSoTT ( ParmHoSoThanhToanViewModel request);
        ResponseLichSuViewModel GetLichSuTTByHoSoId(string hoSoId);
    }
}