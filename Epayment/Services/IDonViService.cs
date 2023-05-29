using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCXN.Models;
using BCXN.ViewModels;
namespace BCXN.Services
{
    public interface IDonViService
    {
        List<DonViViewModel> GetDonVi();
        ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request);
        ResponsePostViewModel CreateDonVi(DonVi request);
        ResponsePostViewModel UpdateDonVi(DonVi request);
        ResponsePostViewModel DeleteDonVi(int id);
        List<DonViViewModel> GetBoPhanByIdDonVi(int id);
        DonVi GetByIdDonVi(int id);
    }
}
