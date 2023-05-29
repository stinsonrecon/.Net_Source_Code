using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCXN.ViewModels;
using Epayment.Models;
using Epayment.ViewModels;

namespace Epayment.Services
{
    public interface ILichSuHoSoTTService
    {
      Task<ResponsePostViewModel> CreateLichSuHoSoTT (ParmHoSoThanhToanViewModel request);
        ResponseLichSuViewModel GetLichSuHoSoTT(string hoSoId);
    }
}