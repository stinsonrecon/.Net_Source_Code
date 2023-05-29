using BCXN.Models;
using BCXN.ViewModels;
using System.Collections.Generic;

namespace BCXN.Repositories
{
    public interface IDonViRepository
    {
        List<DonViViewModel> GetDonVi();
        ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request);
        ResponsePostViewModel CreateDonVi(DonVi donVi);
        ResponsePostViewModel UpdateDonVi(DonVi donVi);
        ResponsePostViewModel DeleteDonVi(int id);
        DonVi GetDonViById(int id);
        List<DonViViewModel> GetBoPhanByIdDonVi(int id);
    }
}
