using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public interface ILichSuXacNhanService
    {
        List<LichSuXacNhanBaoCaoViewModel> GetLichSuXacNhan(int ycbcId, int bcId);
    }
}
