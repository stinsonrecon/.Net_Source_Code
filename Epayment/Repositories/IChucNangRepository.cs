using BCXN.Models;
using BCXN.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCXN.Repositories
{
    public interface IChucNangRepository
    {
        List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId);
        ResponseWithPaginationViewModel GetChucNang(ChucNangSearchViewModel request);
        ResponsePostViewModel Add(ChucNangGetViewModel chucNang);
        ResponsePostViewModel Edit(ChucNangGetViewModel chucNang);
        ResponsePostViewModel Delete(int id);
    }
}
