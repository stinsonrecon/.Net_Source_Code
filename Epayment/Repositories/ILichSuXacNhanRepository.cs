using BCXN.Models;
using BCXN.ViewModels;
using System.Collections.Generic;

namespace BCXN.Repositories
{
    public interface ILichSuXacNhanRepository
    {
        List<LichSuXacNhanBaoCaoViewModel> GetLichSuXacNhan(int ycbcId, int bcId);
    }
}
